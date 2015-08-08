using System;
using System.Diagnostics;
using System.Net.Http;

namespace BandPowerpointRemote
{
	public class ProxyClient
	{
		public static async void NextSlide(int pairId)
		{
			Debug.WriteLine("Moving to next Slide");

			var httpClient = new HttpClient();
			var response = await httpClient.PostAsync("http://powerpointremoteproxy.azurewebsites.net/powerpoint/nextslide/" + pairId, new StringContent(""));
			//var response = await httpClient.PostAsync("http://localhost:3283/powerpoint/nextslide/1234", new StringContent(""));

			Debug.WriteLine("Send signal to move to next Slide. Response: " + response.StatusCode);
		}

		public static async void PrevSlide(int pairId)
		{
			Debug.WriteLine("Moving to prev Slide");

			var httpClient = new HttpClient();
			var response = await httpClient.PostAsync("http://powerpointremoteproxy.azurewebsites.net/powerpoint/prevslide/" + pairId, new StringContent(""));
			//var response = await httpClient.PostAsync("http://localhost:3283/powerpoint/prevslide/1234", new StringContent(""));

			Debug.WriteLine("Send signal to move to prev Slide. Response: " + response.StatusCode);
		}
	}
}

