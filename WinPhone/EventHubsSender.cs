using Microsoft.Band.Sensors;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace WinPhone
{
    public class EventHubsInterface
    {
        public static async Task SendAccelerometerReading(IBandHeartRateReading reading)
        {
            try
            {
                var sas = "";

                // Namespace info.
                var serviceNamespace = "codecamphub2-ns";
                var hubName = "codecamphub2";
                var deviceName = "band1";

                // Create client.
                var httpClient = new HttpClient
                {
                    BaseAddress = new Uri(string.Format("https://{0}.servicebus.windows.net/", serviceNamespace))
                };

                var payload = JsonConvert.SerializeObject(reading);

                httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", sas);

                var content = new StringContent(payload, Encoding.UTF8, "application/json");
                content.Headers.Add("ContentType", "application/atom+xml;type=entry;charset=utf-8");

                var url = string.Format("{0}/publishers/{1}/messages", hubName, deviceName);

                var postResult = await httpClient.PostAsync(url, content);
                var resultContent = postResult.Content.ToString();
                var resultStatus = (int)postResult.StatusCode;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error sending telemetry to event hubs: " + ex.ToString());
            }
        }
    }
}
