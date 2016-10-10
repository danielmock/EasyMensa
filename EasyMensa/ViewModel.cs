using EasyMensa.Models;

namespace EasyMensa
{
	public class ViewModel
	{
		public Canteen MensaAcademica { get; private set; }

		public ViewModel()
		{
			MensaAcademica = Canteen.GetCanteen(187);
		}
	}
}
