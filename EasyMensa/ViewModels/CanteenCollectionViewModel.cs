using System.Collections.ObjectModel;
using GalaSoft.MvvmLight;
using Test.Models;

namespace EasyMensa.ViewModels
{
	public class CanteenCollectionViewModel : ViewModelBase
	{
		public readonly ObservableCollection<NotifyTaskCompletion<Canteen>> CanteenCollection;

		private NotifyTaskCompletion<Canteen> _selectedCanteen;
		public NotifyTaskCompletion<Canteen> SelectedCanteen
		{
			get { return _selectedCanteen; }
			set { _selectedCanteen = value; RaisePropertyChanged();}
		}

		public CanteenCollectionViewModel()
		{
			CanteenCollection = new ObservableCollection<NotifyTaskCompletion<Canteen>>
			{
				new NotifyTaskCompletion<Canteen>(OpenMensaFetcher.GetCanteenAysnc(187))  //  Academica
				//new NotifyTaskCompletion<Canteen>(OpenMensaFetcher.GetCanteenAysnc(95)),  //  Ahorn
				//new NotifyTaskCompletion<Canteen>(OpenMensaFetcher.GetCanteenAysnc(96)),  //  Vita
				//new NotifyTaskCompletion<Canteen>(OpenMensaFetcher.GetCanteenAysnc(94)),  //  Templergraben
				//new NotifyTaskCompletion<Canteen>(OpenMensaFetcher.GetCanteenAysnc(93)),  //  Cafete
				//new NotifyTaskCompletion<Canteen>(OpenMensaFetcher.GetCanteenAysnc(99)),  //  Goethe
				//new NotifyTaskCompletion<Canteen>(OpenMensaFetcher.GetCanteenAysnc(98)),  //  EUPS
			};

			SelectedCanteen = CanteenCollection[0];
		}
	}
}
