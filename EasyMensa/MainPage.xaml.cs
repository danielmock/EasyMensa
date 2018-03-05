using System.Security.Cryptography.X509Certificates;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using EasyMensa.ViewModels;

// Die Vorlage "Leere Seite" ist unter http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409 dokumentiert.

namespace EasyMensa
{
    /// <summary>
    /// Eine leere Seite, die eigenständig verwendet oder zu der innerhalb eines Rahmens navigiert werden kann.
    /// </summary>
    public sealed partial class MainPage : Page
    {
		public MainPage()
        {
            this.InitializeComponent();

	        DataContextChanged += (s, e) =>
	        {
		        ViewModel = DataContext as CanteenCollectionViewModel;
	        };
        }

	    public CanteenCollectionViewModel ViewModel { get; set; }

	    private void HamburgerButton_OnClick(object sender, RoutedEventArgs e)
	    {
		    MySplitView.IsPaneOpen = !MySplitView.IsPaneOpen;
	    }


		// https://stackoverflow.com/a/40035354
		private void MealList_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
	    {
		    if (sender == null) return;

			//Assign DataTemplate for selected items
			foreach (var item in e.AddedItems)
		    {
			    ListViewItem lvi = (sender as ListView).ContainerFromItem(item) as ListViewItem;
			    lvi.ContentTemplate = (DataTemplate) this.Resources["MealTemplateExpanded"];
		    }

		    //Remove DataTemplate for unselected items
		    foreach (var item in e.RemovedItems)
		    {
			    ListViewItem lvi = (sender as ListView).ContainerFromItem(item) as ListViewItem;
			    lvi.ContentTemplate = (DataTemplate) this.Resources["MealTemplate"];
		    }

	    }
    }
}
