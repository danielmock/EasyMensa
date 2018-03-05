using Test.Models;

namespace EasyMensa
{
	public class ViewModel
	{
		public Canteen MensaAcademica { get; private set; }

		public ViewModel()
		{
			MensaAcademica = OpenMensaFetcher.GetCanteenAysnc(187).Result;
		}
	}
}
