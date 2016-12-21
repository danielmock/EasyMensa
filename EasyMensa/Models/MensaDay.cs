using System;
using System.Collections.Generic;

namespace EasyMensa.Models
{
	public class MensaDay
	{
		public string WeekDayName => Date.ToString("dddd");

		public DateTime Date { get; set; }

		public int MensaId { get; set; }

		public bool Closed { get; set; }

		public List<Meal> Meals { get; set; }

		public MensaDay(DateTime date)
		{
			Date = date;
		}
	}
}
