using Microsoft.Band.Portable.Sensors;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace BandPowerpointRemote
{
    public class EventHubsInterface
    {
        public static async Task SendAccelerometerReading(BandAccelerometerReading reading)
        {
            try
            {
                var sas = "SharedAccessSignature sr=https%3A%2F%2Fcodecamphub2-ns.servicebus.windows.net%2Fcodecamphub2%2Fpublishers%2F1%2Fmessages&sig=TYAQlgOd%2Fni60whSbFiOfsDk8lburuufu0yvrVyJzfc%3D&se=1440259779&skn=Devices";

                // Namespace info.
                var serviceNamespace = "codecamphub2-ns";
                var hubName = "codecamphub2";
                var deviceName = "band1";
                var url = string.Format("{0}/publishers/{1}/messages", hubName, deviceName);

                // Create client.
                var httpClient = new HttpClient
                {
                    BaseAddress = new Uri(string.Format("https://{0}.servicebus.windows.net/", serviceNamespace))
                };

                var payload = JsonConvert.SerializeObject(reading);

                httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", sas);

                var content = new StringContent(payload, Encoding.UTF8, "application/json");

                //content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/atom+xml;type=entry;charset=utf-8");
                content.Headers.TryAddWithoutValidation("Content-Type", "application/atom+xml;type=entry;charset=utf-8");
                
                var postResult = await httpClient.PostAsync(url, content);
                var resultContent = postResult.Content.ToString();
                var resultStatus = (int)postResult.StatusCode;
            }
            catch(Exception ex)
            {
                Debug.WriteLine("Error sending telemetry to event hubs: " + ex.ToString());
            }
        }
    }
}
