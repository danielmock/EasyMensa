using Windows.UI.Xaml.Controls;
using EasyMensa.Models;

// Die Vorlage "Leere Seite" ist unter http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409 dokumentiert.

namespace EasyMensa
{
    /// <summary>
    /// Eine leere Seite, die eigenständig verwendet oder zu der innerhalb eines Rahmens navigiert werden kann.
    /// </summary>
    public sealed partial class MainPage : Page
    {
		public NotifyTaskCompletion<Canteen> MensaAcademica { get; private set; }

		public MainPage()
        {
			MensaAcademica = new NotifyTaskCompletion<Canteen>(OpenMensaFetcher.GetCanteenAysnc(187));

            this.InitializeComponent();
        }
    }
}
