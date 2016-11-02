using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace EasyMensa.Models
{
	internal static class OpenMensaFetcher
	{
		private const string OpenmensaUrl = "http://openmensa.org/api/v2/canteens/";

		private static async Task<Canteen> FetchCanteenAsync(int id)
		{
			Canteen result;

			using (var httpClient = new HttpClient())
			{
				var response = httpClient.GetStringAsync($"{OpenmensaUrl}{id}");
				var json = await response.ConfigureAwait(false);
				result = JsonConvert.DeserializeObject<Canteen>(json);
			}

			return result;
		}

		private static async Task<List<MensaDay>> FetchOpenDaysAync(int mensaId)
		{
			List<MensaDay> result;

			using (var httpClient = new HttpClient())
			{
				var response = httpClient.GetStringAsync($"{OpenmensaUrl}{mensaId}/days");
				var json = await response.ConfigureAwait(false);
				result = JsonConvert.DeserializeObject<List<MensaDay>>(json);
			}

			foreach (MensaDay mensaDay in result)
			{
				mensaDay.MensaId = mensaId;
			}

			return result;
		}

		private static async Task<List<Meal>> FetchMealsAsync(int mensaId, DateTime date)
		{
			List<Meal> result;

			using (var httpClient = new HttpClient())
			{
				var response = httpClient.GetStringAsync($"{OpenmensaUrl}{mensaId}/days/{date:yyyy-MM-dd}/meals");
				var json = await response.ConfigureAwait(false);
				result = JsonConvert.DeserializeObject<List<Meal>>(json);
			}

			return result;
		}

		public static async Task<Canteen> GetCanteenAysnc(int mensaId)
		{
			Canteen canteen = await FetchCanteenAsync(mensaId).ConfigureAwait(false);

			canteen.Days = await FetchOpenDaysAync(mensaId).ConfigureAwait(false);

			foreach (var mensaDay in canteen.Days)
			{
				mensaDay.Meals = await FetchMealsAsync(mensaId, mensaDay.Date).ConfigureAwait(false);
			}

			return canteen;
		}
	}
}
