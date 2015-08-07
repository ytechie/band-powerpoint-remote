using System;
using System.Linq;
using System.Threading.Tasks;
using System.Diagnostics;
using Microsoft.Band.Portable;
using System.Net.Http;

namespace BandPowerpointRemote
{
    public class BandManager
    {
		private MainPage _parentPage;
        private BandClient _bandClient;

        private WaveGestureDetector _waveGesture = new WaveGestureDetector();

		public BandManager(MainPage parentPage)
		{
			this._parentPage = parentPage;
		}

        public async Task<int> StartBandMonitor()
        {
            _waveGesture.WaveDetected += _waveGesture_WaveDetected;
            _waveGesture.ReverseWaveDetected += _waveGesture_ReverseWaveDetected;

            // Get the list of Microsoft Bands paired to the phone.
            var pairedBands = await BandClientManager.Instance.GetPairedBandsAsync();
            if (pairedBands.Count() < 1)
            {
				await _parentPage.DisplayAlert ("No Bands", "Unable to find a Band to connect to.", "Ok");
				return 0;
            }

            // Connect to Microsoft Band.
            _bandClient = await BandClientManager.Instance.ConnectAsync(pairedBands.First());
            Debug.WriteLine("Band found");

            var sensors = _bandClient.SensorManager;

            //sensors.Gyroscope.ReportingInterval = sensors.Gyroscope.SupportedReportingIntervals.Min();

            sensors.Accelerometer.ReadingChanged += (s, args) =>
            {
                _waveGesture.AddAccelerometerReading(args.SensorReading);
                EventHubsInterface.SendAccelerometerReading(args.SensorReading);
            };

            //SensorStatusTextBlock.Text = "Initialized!";
            //Background = new Windows.UI.Xaml.Media.SolidColorBrush(Windows.UI.Colors.Green);

            await sensors.Accelerometer.StartReadingsAsync(Microsoft.Band.Portable.Sensors.BandSensorSampleRate.Ms16);
            Debug.WriteLine("Sensor reads requested");

            return 0;
        }

        private async void _waveGesture_WaveDetected(object sender, EventArgs e)
        {
            Debug.WriteLine("Moving to next Slide");

            var httpClient = new HttpClient();
            var response = await httpClient.PostAsync("http://powerpointremoteproxy.azurewebsites.net/powerpoint/nextslide/1234", new StringContent(""));
            //var response = await httpClient.PostAsync("http://localhost:3283/powerpoint/nextslide/1234", new StringContent(""));

            Debug.WriteLine("Send signal to move to next Slide. Response: " + response.StatusCode);
        }

        private async void _waveGesture_ReverseWaveDetected(object sender, EventArgs e)
        {
            Debug.WriteLine("Moving to prev Slide");

            var httpClient = new HttpClient();
            var response = await httpClient.PostAsync("http://powerpointremoteproxy.azurewebsites.net/powerpoint/prevslide/1234", new StringContent(""));
            //var response = await httpClient.PostAsync("http://localhost:3283/powerpoint/prevslide/1234", new StringContent(""));

            Debug.WriteLine("Send signal to move to prev Slide. Response: " + response.StatusCode);
        }
    }
}
