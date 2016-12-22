using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace EasyMensa.Models
{
	public class Meal
	{
		private string _name;
		
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

		public Meal(string name, int id, string category, Prices prices, List<string> notes)
		{
			Name = name;
			Id = id;
			Category = category;
			Prices = prices;
			Notes = notes;
		}
	}

	public class Prices
	{
		public float? Students { get; set; }
		public float? Employees { get; set; }
		public float? Pupils { get; set; }
		public float? Others { get; set; }
	}
}
