using System.Collections.Generic;

namespace EasyMensa.Models
{
	public class Canteen
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public string City { get; set; }
		public string Address { get; set; }
		public List<double> Coordinates { get; set; }
		public List<MensaDay> Days { get; set; }
	}
}
