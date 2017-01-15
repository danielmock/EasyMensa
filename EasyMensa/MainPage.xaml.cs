using System.Security.Cryptography.X509Certificates;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using EasyMensa.Models;
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
    }
}
