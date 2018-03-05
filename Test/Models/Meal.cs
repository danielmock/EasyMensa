using System;
using System.Collections.Generic;
using System.Linq;

namespace Test.Models
{
	public class Meal
	{
		private string _name;
		
		public int Id { get; set; }

		public string Name
		{
			get { return _name; }
			set // Splits the name and description of the meal, filters out stuff
			{
				var substrings = value.Split(new[] {" mit ", " | "}, StringSplitOptions.RemoveEmptyEntries);
				_name = substrings[0];
					//.Trim(' ');

				// replace first bar with "mit", last with "und" and the others with ","
				if (substrings.Length == 2)
				{
					Description = $"mit {substrings[1]}";
				}
				else if (substrings.Length > 2)
				{
					Description =
						$"mit {string.Join(", ", substrings.Skip(1).Take(substrings.Length - 2))} und {substrings[substrings.Length - 1]}";
				}
				else
				{
					Description = string.Empty;
				}
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
