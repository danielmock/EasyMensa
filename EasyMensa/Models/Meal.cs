using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace EasyMensa.Models
{
	public class Meal
	{
		private string _name;

		// Properties
		public int Id { get; set; }

		public string Name
		{
			get { return _name; }
			set // Filters out shitty spaces and splits the name and the description of a meal
			{
				value = Regex.Replace(value, @"  ", " ");
				value = Regex.Replace(value, @" ,", ",");
				var substrings = Regex.Split(value, "(mit|und)");
				_name = substrings[0];
				substrings[0] = "";
				Description = String.Join("", substrings);
			}
		}

		public string Description { get; set; }

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
