using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyMensa.Models
{
	public class Meal
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public string Category { get; set; }
		public Prices Prices { get; set; }
		public List<string> Notes { get; set; }
	}

	public class Prices
	{
		public decimal Students { get; set; }
		public object Employees { get; set; }
		public object Pupils { get; set; }
		public decimal Others { get; set; }
	}
}
