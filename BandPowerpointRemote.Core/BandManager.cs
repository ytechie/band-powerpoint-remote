using System;
using System.Linq;
using System.Threading.Tasks;
using System.Diagnostics;
using Microsoft.Band.Portable;

namespace BandPowerpointRemote
{
    public class BandManager
    {
		private MainPage _parentPage;
        private BandClient _bandClient;

		public event EventHandler MovePrevGesture;
		public event EventHandler MoveNextGesture;


        private WaveGestureDetector _waveGesture = new WaveGestureDetector();

		public BandManager(MainPage parentPage)
		{
			this._parentPage = parentPage;
		}

        public async Task<int> StartBandMonitor()
        {
			_waveGesture.WaveDetected += (sender, e) => {
				var eh = MoveNextGesture;
				if(eh != null)
					eh(sender, e);
			};
			_waveGesture.ReverseWaveDetected += (sender, e) => {
				var eh = MovePrevGesture;
				if(eh != null)
					eh(sender, e);
			};

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
                //EventHubsInterface.SendAccelerometerReading(args.SensorReading);
            };

			sensors.Gyroscope.ReadingChanged += (sender, e) => 
			{
				_waveGesture.AddGyroscopeReading(e.SensorReading);
			};

            //SensorStatusTextBlock.Text = "Initialized!";
            //Background = new Windows.UI.Xaml.Media.SolidColorBrush(Windows.UI.Colors.Green);
            await sensors.Accelerometer.StartReadingsAsync(Microsoft.Band.Portable.Sensors.BandSensorSampleRate.Ms16);
			//await sensors.Gyroscope.StartReadingsAsync (Microsoft.Band.Portable.Sensors.BandSensorSampleRate.Ms16);

            Debug.WriteLine("Sensor reads requested");

            return 0;
        }


    }
}
