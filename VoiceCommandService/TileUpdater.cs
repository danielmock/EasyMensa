using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Windows.ApplicationModel.Background;
using Windows.UI.Notifications;
using Microsoft.Toolkit.Uwp.Notifications;
using Test.Models;

namespace EasyMensa.BackgroundTasks
{
	public sealed class TileUpdater : IBackgroundTask
	{
		public async void Run(IBackgroundTaskInstance taskInstance)
		{
			// Get a deferral, to prevent the task from closing prematurely
			// while asynchronous code is still running.
			var deferral = taskInstance.GetDeferral();


			var menu = await OpenMensaFetcher.FetchMealsAsync(187, DateTime.Today);

			// Update the live tile with the feed items.
			UpdateTile(menu);

			// Inform the system that the task is finished.
			deferral.Complete();
		}

		private void UpdateTile(List<Meal> menu)
		{
			// Create a tile update manager for the specified menu
			var updater = TileUpdateManager.CreateTileUpdaterForApplication();
			updater.EnableNotificationQueue(true);
			updater.Clear();

			var children = (from meal in menu.Take(4) select new AdaptiveText { Text = meal.Name }).ToList();
			for (int i = 0; i < 4 - children.Count; i++)
			{
				children.Add(new AdaptiveText());
			}

			// add for debugging
			var item = new AdaptiveText {Text = DateTime.Now.ToString(CultureInfo.CurrentCulture), HintStyle = AdaptiveTextStyle.Caption};
			children.Add(item);

			var tileBinding = new TileBinding
			{
				Content = new TileBindingContentAdaptive
				{
					Children = {children[0], children[1], children[2], children[3], children[4]}
					//Children = {new AdaptiveText { Text = "T E S T"} }
				}
			};

			TileContent content = new TileContent
			{
				Visual = new TileVisual
				{
					TileMedium = tileBinding,
					TileWide = tileBinding
				}
			};

			updater.Update(new TileNotification(content.GetXml()));

		}
	}
}
