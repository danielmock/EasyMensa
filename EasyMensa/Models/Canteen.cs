using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

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
		
		//public static Canteen GetCanteen(int id)
		//{
		//	Canteen result = OpenMensaFetcher.FetchCanteenAsync(id).Result;

		//	result.Days = OpenMensaFetcher.FetchOpenDaysAync(id).Result;

		//	foreach (MensaDay mensaDay in result.Days)
		//	{
		//		mensaDay.Meals = OpenMensaFetcher.FetchMealsAsync(id, mensaDay.Date).Result;
		//	}

		//	return result;
		//}

		// First argument mensaID, second date
		private static string OPENMENSA_URL = "http://openmensa.org/api/v2/canteens/";
		private static string OPENMENSA_MEAL_URL = "http://openmensa.org/api/v2/canteens/{0:D}/days/{1:yyyy-MM-dd}/meals";
		private static string OPENMENSA_OPEN_DAYS_URL = "http://openmensa.org/api/v2/canteens/{0:D}/days";

		//TODO change this
		public static Canteen GetCanteen(int id)
		{
			Canteen result;

			using (var httpClient = new HttpClient())
			{
				var json = httpClient.GetStringAsync($"{OPENMENSA_URL}{id}").Result;
				result = JsonConvert.DeserializeObject<Canteen>(json);
			}
			result.Days = result.getDays();
			return result;
		}

		public List<MensaDay> getDays()
		{
			List<MensaDay> result = new List<MensaDay>();
			using (var httpClient = new HttpClient())
			{
				var json = httpClient.GetStringAsync(String.Format(OPENMENSA_OPEN_DAYS_URL, this.Id)).Result;
				result = JsonConvert.DeserializeObject<List<MensaDay>>(json);
			}

			foreach (var MensaDay in result)
			{
				MensaDay.Meals = getMeals(MensaDay);
			}
			return result;
		}

		public List<Meal> getMeals(MensaDay Day)
		{
			List<Meal> result;

			using (var httpClient = new HttpClient())
			{
				var json = httpClient.GetStringAsync(String.Format(OPENMENSA_MEAL_URL, Id, Day.Date)).Result;
				result = JsonConvert.DeserializeObject<List<Meal>>(json);
			}

			return result;
		}
	}
}
