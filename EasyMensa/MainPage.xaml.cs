using System;
using System.Diagnostics;
using Windows.ApplicationModel.Background;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using EasyMensa.ViewModels;

// Die Vorlage "Leere Seite" ist unter http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409 dokumentiert.

namespace EasyMensa
{
    /// <summary>
    /// Eine leere Seite, die eigenständig verwendet oder zu der innerhalb eines Rahmens navigiert werden kann.
    /// </summary>
    public sealed partial class MainPage
    {
		public MainPage()
        {
            InitializeComponent();

	        DataContextChanged += (s, e) =>
	        {
		        ViewModel = DataContext as CanteenCollectionViewModel;
	        };
        }

		protected override void OnNavigatedTo(NavigationEventArgs e)
		{
			StartLiveTileUpdater();
		}

		//private async void RegisterBackgroundTast()
		//{
		//	var backgroundAccessStatus = await BackgroundExecutionManager.RequestAccessAsync();
		//	if (backgroundAccessStatus != BackgroundAccessStatus.AlwaysAllowed &&
		//	    backgroundAccessStatus != BackgroundAccessStatus.AllowedSubjectToSystemPolicy) return;
		//	foreach (var task in BackgroundTaskRegistration.AllTasks)
		//	{
		//		if (task.Value.Name == taskName)
		//		{
		//			task.Value.Unregister(true);
		//		}
		//	}

		//	BackgroundTaskBuilder taskBuilder = new BackgroundTaskBuilder();
		//	taskBuilder.Name = taskName;
		//	taskBuilder.TaskEntryPoint = taskEntryPoint;
		//	taskBuilder.SetTrigger(new TimeTrigger(15, false));
		//	var registration = taskBuilder.Register();
		//}

		public CanteenCollectionViewModel ViewModel { get; set; }

	    private void HamburgerButton_OnClick(object sender, RoutedEventArgs e)
	    {
		    MySplitView.IsPaneOpen = !MySplitView.IsPaneOpen;
	    }


		// https://stackoverflow.com/a/40035354
		private void MealList_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
	    {
		    ListView senderListView = sender as ListView;
		    if (senderListView == null)
			    return;

			Debug.Assert(e.AddedItems.Count == 1, "List View: Not exactly 1 added Item");

		    //Toggle DataTemplate for selected items
			foreach (var item in e.AddedItems)
			{
				ListViewItem lvi = senderListView.ContainerFromItem(item) as ListViewItem;
				if (lvi == null) continue;
				lvi.ContentTemplate = lvi.ContentTemplate == (DataTemplate)Resources["MealTemplate"]
					? (DataTemplate) Resources["MealTemplateExpanded"]
					: (DataTemplate) Resources["MealTemplate"];
			}

		    //Remove DataTemplate for unselected items
		    foreach (var item in e.RemovedItems)
		    {
			    ListViewItem lvi = senderListView.ContainerFromItem(item) as ListViewItem;
			    if (lvi == null) continue;
			    lvi.ContentTemplate = (DataTemplate) Resources["MealTemplate"];
		    }

	    }

		// Hintergrundaufgabe zeitlich planen
		// alle 12h
		private static async void StartLiveTileUpdater()
		{
			var requestStatus = await BackgroundExecutionManager.RequestAccessAsync();
			if (requestStatus != BackgroundAccessStatus.AlwaysAllowed &&
					requestStatus != BackgroundAccessStatus.AllowedSubjectToSystemPolicy)
				// Depending on the value of requestStatus, provide an appropriate response
				// such as notifying the user which functionality won't work as expected
				return;

			const string taskInitiliazerName = "EasyMensa Live Tile Initiliazer";
			const string taskUpdaterName = "EasyMensa Live Tile Updater";
			const string taskEntryPoint = "EasyMensa.BackgroundTasks.TileUpdater";
			SystemCondition internetCondition = new SystemCondition(SystemConditionType.InternetAvailable);

			foreach (var task in BackgroundTaskRegistration.AllTasks)
			{
				if (task.Value.Name == taskInitiliazerName || task.Value.Name == taskUpdaterName)
				{
					task.Value.Unregister(true);
				}
			}

			// installs a background task which initiliazes the live tile
			var taskBuilder = new BackgroundTaskBuilder
			{
				Name = taskInitiliazerName,
				TaskEntryPoint = taskEntryPoint
			};
			taskBuilder.SetTrigger(new TimeTrigger(15, true)); // 15min are minimum, 
			taskBuilder.AddCondition(internetCondition);
			taskBuilder.Register();

			// installs the recurring updater 
			var recurringTaskBuilder = new BackgroundTaskBuilder
			{
				Name = taskUpdaterName,
				TaskEntryPoint = taskEntryPoint
			};
			recurringTaskBuilder.SetTrigger(new TimeTrigger(60 * 12, false)); // every 12h
			recurringTaskBuilder.AddCondition(internetCondition);
			recurringTaskBuilder.Register();
		}
    }
}
