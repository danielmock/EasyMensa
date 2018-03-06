using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Windows.ApplicationModel.AppService;
using Windows.ApplicationModel.Background;
using Windows.ApplicationModel.Resources.Core;
using Windows.ApplicationModel.VoiceCommands;
using Test.Models;

namespace EasyMensa.VoiceCommands
{

	public sealed class EasyMensaVoiceCommandService : IBackgroundTask
	{

		/// <summary>
		/// the service connection is maintained for the lifetime of a cortana session, once a voice command
		/// has been triggered via Cortana.
		/// </summary>
		VoiceCommandServiceConnection voiceServiceConnection;

		/// <summary>
		/// Lifetime of the background service is controlled via the BackgroundTaskDeferral object, including
		/// registering for cancellation events, signalling end of execution, etc. Cortana may terminate the 
		/// background service task if it loses focus, or the background task takes too long to provide.
		/// 
		/// Background tasks can run for a maximum of 30 seconds.
		/// </summary>
		BackgroundTaskDeferral serviceDeferral;

		/// <summary>
		/// ResourceMap containing localized strings for display in Cortana.
		/// </summary>
		ResourceMap cortanaResourceMap;

		/// <summary>
		/// The context for localized strings.
		/// </summary>
		ResourceContext cortanaContext;

		/// <summary>
		/// Get globalization-aware date formats.
		/// </summary>
		DateTimeFormatInfo dateFormatInfo;

		/// <summary>
		/// Background task entrypoint. Voice Commands using the <VoiceCommandService Target="...">
		/// tag will invoke this when they are recognized by Cortana, passing along details of the 
		/// invocation. 
		/// 
		/// Background tasks must respond to activation by Cortana within 0.5 seconds, and must 
		/// report progress to Cortana every 5 seconds (unless Cortana is waiting for user
		/// input). There is no execution time limit on the background task managed by Cortana,
		/// but developers should use plmdebug (https://msdn.microsoft.com/en-us/library/windows/hardware/jj680085%28v=vs.85%29.aspx)
		/// on the Cortana app package in order to prevent Cortana timing out the task during
		/// debugging.
		/// 
		/// Cortana dismisses its UI if it loses focus. This will cause it to terminate the background
		/// task, even if the background task is being debugged. Use of Remote Debugging is recommended
		/// in order to debug background task behaviors. In order to debug background tasks, open the
		/// project properties for the app package (not the background task project), and enable
		/// Debug -> "Do not launch, but debug my code when it starts". Alternatively, add a long
		/// initial progress screen, and attach to the background task process while it executes.
		/// </summary>
		/// <param name="taskInstance">Connection to the hosting background service process.</param>
		public async void Run(IBackgroundTaskInstance taskInstance)
		{
			serviceDeferral = taskInstance.GetDeferral();

			// Register to receive an event if Cortana dismisses the background task. This will
			// occur if the task takes too long to respond, or if Cortana's UI is dismissed.
			// Any pending operations should be cancelled or waited on to clean up where possible.
			taskInstance.Canceled += OnTaskCanceled;

			var triggerDetails = taskInstance.TriggerDetails as AppServiceTriggerDetails;

			// Load localized resources for strings sent to Cortana to be displayed to the user.
			cortanaResourceMap = ResourceManager.Current.MainResourceMap.GetSubtree("Resources");

			// Select the system language, which is what Cortana should be running as.
			cortanaContext = ResourceContext.GetForViewIndependentUse();

			// Get the currently used system date format
			dateFormatInfo = CultureInfo.CurrentCulture.DateTimeFormat;

			// This should match the uap:AppService and VoiceCommandService references from the 
			// package manifest and VCD files, respectively. Make sure we've been launched by
			// a Cortana Voice Command.
			if (triggerDetails != null && triggerDetails.Name == "EasyMensaVoiceCommandService")
			{
				try
				{
					voiceServiceConnection =
						VoiceCommandServiceConnection.FromAppServiceTriggerDetails(triggerDetails);

					voiceServiceConnection.VoiceCommandCompleted += OnVoiceCommandCompleted;

					// GetVoiceCommandAsync establishes initial connection to Cortana, and must be called prior to any 
					// messages sent to Cortana. Attempting to use ReportSuccessAsync, ReportProgressAsync, etc
					// prior to calling this will produce undefined behavior.
					VoiceCommand voiceCommand = await voiceServiceConnection.GetVoiceCommandAsync();

					// Depending on the operation (defined in AdventureWorks:AdventureWorksCommands.xml)
					// perform the appropriate command.
					switch (voiceCommand.CommandName)
					{
						case "showCategory":
							string spokenCategory = "";
							try
							{
								spokenCategory = voiceCommand.Properties["category"][0];
							}
							catch
							{
								// 
							}

							string spokenDayAdverb = "";
							try
							{
								spokenDayAdverb = voiceCommand.Properties["dayAdverb"][0];
							}
							catch
							{
								spokenDayAdverb = "heute";
							}
							await SendCategoryAsync(spokenDayAdverb, spokenCategory);
							break;
						case "showMenu":
						case "showMenuToday":
							string spokenDayAdverb1 = "";
							try
							{
								spokenDayAdverb1 = voiceCommand.Properties["dayAdverb"][0];
							}
							catch
							{
								spokenDayAdverb1 = "heute";
							}
							await SendMenuAsync(spokenDayAdverb1);
							break;
						default:
							// As with app activation VCDs, we need to handle the possibility that
							// an app update may remove a voice command that is still registered.
							// This can happen if the user hasn't run an app since an update.
							LaunchAppInForeground();
							break;
					}
				}
				catch (Exception ex)
				{
					System.Diagnostics.Debug.WriteLine("Handling Voice Command failed " + ex.ToString());
				}
			}
		}

		private async Task SendCategoryAsync(string spokenDayAdverb, string spokenCategory)
		{
			DateTime date = parseDate(spokenDayAdverb);
			var menu = await OpenMensaFetcher.FetchMealsAsync(187, date);
			var mealContentTiles = new List<VoiceCommandContentTile>();

			foreach (var meal in menu.
				FindAll(x => x.Category.ToLower().Contains(spokenCategory.ToLower())))
			{
				var mealTile = new VoiceCommandContentTile
				{
					ContentTileType = VoiceCommandContentTileType.TitleWithText,
					Title = meal.Name,
					TextLine1 = meal.Description,
					TextLine2 = meal.Category,
					TextLine3 = string.Format("{0:C2}", meal.Prices.Students)
				};
				mealContentTiles.Add(mealTile);
			}

			string displayMessage;
			string spokenMessage;

			if (mealContentTiles.Count == 0)
			{
				displayMessage =
					spokenMessage = $"Es konnten keine Einträge für {spokenCategory} und {spokenDayAdverb} gefunden werden";
			}
			else
			{
				displayMessage = $"Die Ergebnisse für {spokenCategory} und {spokenDayAdverb}:";
				spokenMessage = $"{spokenDayAdverb} gibt es {mealContentTiles[0].Title}!";
			}

			await SendContentToCortana(displayMessage, spokenMessage, mealContentTiles);
		}


		/// <summary>
		/// Sends the today's canteen menu (async) to Cortana.
		/// Cortana then displays the first four entries of the Menu: Tellergericht, Vegetarisch, Empfehlung, Klassiker
		/// The Klassiker is read out loud
		/// Only for today,de-DE, mensa academica (187)
		/// </summary>
		/// <returns></returns>
		private async Task SendMenuAsync(string spokenDayAdverb)
		{
			DateTime date = parseDate(spokenDayAdverb);
			var mealList = await OpenMensaFetcher.FetchMealsAsync(187, date);
			var mealContentTiles = new List<VoiceCommandContentTile>();

			// check for Tellergericht, Vegetarisch, Empfehlung, Klassiker
			// needs to be changed if the order imposed by the API changes
			for (var j = 0; j < 4; j++)
			{
				var meal = mealList[j];
				var mealTile = new VoiceCommandContentTile
				{
					ContentTileType = VoiceCommandContentTileType.TitleWithText,
					Title = meal.Name,
					TextLine1 = meal.Description,
					TextLine2 = meal.Category,
					TextLine3 = string.Format("{0:C2}", meal.Prices.Students)
				};
				mealContentTiles.Add(mealTile);
			}
			// check for Haupt- & Nebenbeilage
			// needs to be changed if the order imposed by the API changes
			for (int j = mealList.Count - 2; j < mealList.Count; j++)
			{
				var meal = mealList[j];
				var mealTile = new VoiceCommandContentTile
				{
					ContentTileType = VoiceCommandContentTileType.TitleWithText,
					Title = meal.Name,
					TextLine1 = meal.Category
				};
				mealContentTiles.Add(mealTile);
			}

			string displayMessage;
			string spokenMessage;

			if (mealList.Count == 0)
			{
				displayMessage = spokenMessage = "Ich konnte nichts finden.";
			}
			else
			{
				displayMessage = $"Das Menü für {spokenDayAdverb}";
				if (mealContentTiles.Count > 3)
				{
					spokenMessage = $"Der Klassiker ist {mealContentTiles[3].Title}";
				}
				else
				{
					spokenMessage = $"Es gibt {mealContentTiles[0].Title}";
				}
			}

			await SendContentToCortana(displayMessage, spokenMessage, mealContentTiles);
		}


		/// <summary>
		/// Sends the parameters to Cortana
		/// </summary>
		/// <param name="displayMessage"></param>
		/// <param name="spokenMessage"></param>
		/// <param name="contentTiles"></param>
		/// <returns></returns>
		private async Task SendContentToCortana(string displayMessage, string spokenMessage, IEnumerable<VoiceCommandContentTile> contentTiles)
		{
			var userMessage = new VoiceCommandUserMessage
			{
				DisplayMessage = displayMessage,
				SpokenMessage = spokenMessage
			};

			await SendContentToCortana(userMessage, contentTiles);
		}


		private async Task SendContentToCortana(VoiceCommandUserMessage userMessage, IEnumerable<VoiceCommandContentTile> contentTiles)
		{
			var response = VoiceCommandResponse.CreateResponse(userMessage, contentTiles);

			await voiceServiceConnection.ReportSuccessAsync(response);
		}


		private DateTime parseDate(string spokenDayAdverb)
		{
			int dayDifference;
			switch (spokenDayAdverb)
			{
				case "heute":
					dayDifference = 0;
					break;
				case "gestern":
					dayDifference = -1;
					break;
				case "vorgestern":
					dayDifference = -2;
					break;
				case "morgen":
					dayDifference = 1;
					break;
				case "übermorgen":
					dayDifference = 2;
					break;
				default:
					dayDifference = 0;
					break;
			}
			return DateTime.Today.AddDays(dayDifference);
		}


		private async Task RespondToUser(string text)
		{
			var userMessage = new VoiceCommandUserMessage();
			userMessage.DisplayMessage = userMessage.SpokenMessage = text;
			VoiceCommandResponse response = VoiceCommandResponse.CreateResponse(userMessage);
			await voiceServiceConnection.ReportSuccessAsync(response);
		}


		/// <summary>
		/// Provide a simple response that launches the app. Expected to be used in the
		/// case where the voice command could not be recognized (eg, a VCD/code mismatch.)
		/// </summary>
		private async void LaunchAppInForeground()
		{
			var userMessage = new VoiceCommandUserMessage
			{
				SpokenMessage = cortanaResourceMap.GetValue("LaunchingAdventureWorks", cortanaContext).ValueAsString
			};

			var response = VoiceCommandResponse.CreateResponse(userMessage);

			response.AppLaunchArgument = "";

			await voiceServiceConnection.RequestAppLaunchAsync(response);
		}

		/// <summary>
		/// Handle the completion of the voice command. Your app may be cancelled
		/// for a variety of reasons, such as user cancellation or not providing 
		/// progress to Cortana in a timely fashion. Clean up any pending long-running
		/// operations (eg, network requests).
		/// </summary>
		/// <param name="sender">The voice connection associated with the command.</param>
		/// <param name="args">Contains an Enumeration indicating why the command was terminated.</param>
		private void OnVoiceCommandCompleted(VoiceCommandServiceConnection sender, VoiceCommandCompletedEventArgs args)
		{
			serviceDeferral?.Complete();
		}

		/// <summary>
		/// When the background task is cancelled, clean up/cancel any ongoing long-running operations.
		/// This cancellation notice may not be due to Cortana directly. The voice command connection will
		/// typically already be destroyed by this point and should not be expected to be active.
		/// </summary>
		/// <param name="sender">This background task instance</param>
		/// <param name="reason">Contains an enumeration with the reason for task cancellation</param>
		private void OnTaskCanceled(IBackgroundTaskInstance sender, BackgroundTaskCancellationReason reason)
		{
			System.Diagnostics.Debug.WriteLine("Task cancelled, clean up");
			//Complete the service deferral
			serviceDeferral?.Complete();
		}
	}
}
