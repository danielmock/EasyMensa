using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace EasyMensa.Models
{
	internal class OpenMensaFetcher
	{
		private static string OPENMENSA_URL = "http://openmensa.org/api/v2/canteens/";

		public static async Task<Canteen> FetchCanteenAsync(int id)
		{
			Canteen result;

			using (var httpClient = new HttpClient())
			{
				var response = httpClient.GetStringAsync($"{OPENMENSA_URL}{id}");
				var json = await response;
				result = JsonConvert.DeserializeObject<Canteen>(json);
			}

			return result;
		}

		public static async Task<List<MensaDay>> FetchOpenDaysAync(int mensaId)
		{
			List<MensaDay> result;

			using (var httpClient = new HttpClient())
			{
				var response = httpClient.GetStringAsync($"{OPENMENSA_URL}{mensaId}/days");
				var json = await response;
				result = JsonConvert.DeserializeObject<List<MensaDay>>(json);
			}

			foreach (MensaDay mensaDay in result)
			{
				mensaDay.MensaId = mensaId;
			}

			return result;
		}

		public static async Task<List<Meal>> FetchMealsAsync(int mensaId, DateTime date)
		{
			List<Meal> result;

			using (var httpClient = new HttpClient())
			{
				var response = httpClient.GetStringAsync($"{OPENMENSA_URL}{mensaId}/days{date:yyyy-MMM-dd}");
				var json = await response;
				result = JsonConvert.DeserializeObject<List<Meal>>(json);
			}

			return result;
		}
	}
}
