using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.VoiceCommands;
using Windows.Foundation.Metadata;
using Windows.Storage;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace EasyMensa
{
	/// <summary>
	/// Stellt das anwendungsspezifische Verhalten bereit, um die Standardanwendungsklasse zu ergänzen.
	/// </summary>
	sealed partial class App
	{
        /// <summary>
        /// Initialisiert das Singletonanwendungsobjekt. Dies ist die erste Zeile von erstelltem Code
        /// und daher das logische Äquivalent von main() bzw. WinMain().
        /// </summary>
        public App()
        {
            InitializeComponent();
            Suspending += OnSuspending;
        }

        /// <summary>
        /// Wird aufgerufen, wenn die Anwendung durch den Endbenutzer normal gestartet wird. Weitere Einstiegspunkte
        /// werden z. B. verwendet, wenn die Anwendung gestartet wird, um eine bestimmte Datei zu öffnen.
        /// </summary>
        /// <param name="e">Details über Startanforderung und -prozess.</param>
        protected override async void OnLaunched(LaunchActivatedEventArgs e)
		{
#if DEBUG
			if (Debugger.IsAttached)
			{
				DebugSettings.EnableFrameRateCounter = true;
			}
#endif
			Frame rootFrame = Window.Current.Content as Frame;

			// App-Initialisierung nicht wiederholen, wenn das Fenster bereits Inhalte enthält.
			// Nur sicherstellen, dass das Fenster aktiv ist.
			if (rootFrame == null)
			{
				// Frame erstellen, der als Navigationskontext fungiert und zum Parameter der ersten Seite navigieren
				rootFrame = new Frame();

				rootFrame.NavigationFailed += OnNavigationFailed;

				if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
				{
					//TODO: Zustand von zuvor angehaltener Anwendung laden
				}

				// Den Frame im aktuellen Fenster platzieren
				Window.Current.Content = rootFrame;
			}

			if (e.PrelaunchActivated == false)
			{
				if (rootFrame.Content == null)
				{
					// Wenn der Navigationsstapel nicht wiederhergestellt wird, zur ersten Seite navigieren
					// und die neue Seite konfigurieren, indem die erforderlichen Informationen als Navigationsparameter
					// übergeben werden
					rootFrame.Navigate(typeof(MainPage), e.Arguments);
				}
				// Sicherstellen, dass das aktuelle Fenster aktiv ist
				Window.Current.Activate();
			}

			// Verändert die Statusbar auf Mobilen Geräten
			if (ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar"))
			{
				var statusBar = StatusBar.GetForCurrentView();
				if (statusBar != null)
				{
					statusBar.BackgroundOpacity = 1;
					statusBar.BackgroundColor = ((SolidColorBrush)Current.Resources["MyTitleBarBackgroundBrush"]).Color;
					statusBar.ForegroundColor = Colors.White;
				}
			}

			// Installiere Voice Commands
			await InstallVCD();
			//await StartLiveTileUpdater();
		}



		/// <summary>
		/// Wird aufgerufen, wenn die Navigation auf eine bestimmte Seite fehlschlägt
		/// </summary>
		/// <param name="sender">Der Rahmen, bei dem die Navigation fehlgeschlagen ist</param>
		/// <param name="e">Details über den Navigationsfehler</param>
		void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        /// <summary>
        /// Wird aufgerufen, wenn die Ausführung der Anwendung angehalten wird.  Der Anwendungszustand wird gespeichert,
        /// ohne zu wissen, ob die Anwendung beendet oder fortgesetzt wird und die Speicherinhalte dabei
        /// unbeschädigt bleiben.
        /// </summary>
        /// <param name="sender">Die Quelle der Anhalteanforderung.</param>
        /// <param name="e">Details zur Anhalteanforderung.</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            //TODO: Anwendungszustand speichern und alle Hintergrundaktivitäten beenden
            deferral.Complete();
        }

	    private async Task InstallVCD()
		{
			try
			{
				StorageFile vcdStorageFile = await Package.Current.InstalledLocation.GetFileAsync(@"EasyMensaCommands.xml");
				await VoiceCommandDefinitionManager.InstallCommandDefinitionsFromStorageFileAsync(vcdStorageFile);
			}
			catch (Exception)
			{
				// ignored
			}
		}
	}
}
