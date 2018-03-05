using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using Test.Models;

namespace EasyMensa.ViewModels
{
	public class CanteenCollectionViewModel : ViewModelBase
	{
		public ObservableCollection<NotifyTaskCompletion<Canteen>> canteenCollection;

		private NotifyTaskCompletion<Canteen> _selectedCanteen;
		public NotifyTaskCompletion<Canteen> SelectedCanteen
		{
			get { return _selectedCanteen; }
			set { _selectedCanteen = value; RaisePropertyChanged();}
		}

		public CanteenCollectionViewModel()
		{
			canteenCollection = new ObservableCollection<NotifyTaskCompletion<Canteen>>
			{
				new NotifyTaskCompletion<Canteen>(OpenMensaFetcher.GetCanteenAysnc(187)),  //  Academica
				//new NotifyTaskCompletion<Canteen>(OpenMensaFetcher.GetCanteenAysnc(95)),  //  Ahorn
				//new NotifyTaskCompletion<Canteen>(OpenMensaFetcher.GetCanteenAysnc(96)),  //  Vita
				//new NotifyTaskCompletion<Canteen>(OpenMensaFetcher.GetCanteenAysnc(94)),  //  Templergraben
				//new NotifyTaskCompletion<Canteen>(OpenMensaFetcher.GetCanteenAysnc(93)),  //  Cafete
				//new NotifyTaskCompletion<Canteen>(OpenMensaFetcher.GetCanteenAysnc(99)),  //  Goethe
				//new NotifyTaskCompletion<Canteen>(OpenMensaFetcher.GetCanteenAysnc(98)),  //  EUPS
			};

			SelectedCanteen = canteenCollection[0];
		}
	}
}
