using Microsoft.Band;
using Microsoft.Band.Sensors;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using System.Linq;
using Windows.UI.Core;

namespace WinPhone
{
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();

            this.NavigationCacheMode = NavigationCacheMode.Required;
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            await StartBandMonitor();
        }

        private bool buffering = false;

        private List<IBandGyroscopeReading> _gyroscopeBuffer = new List<IBandGyroscopeReading>();

        IBandClient _bandClient;

        private BandStreamProcessor.WaveGestureDetector _waveGesture = new BandStreamProcessor.WaveGestureDetector();

        private async Task<int> StartBandMonitor()
        {
            _waveGesture.WaveDetected += _waveGesture_WaveDetected;
            _waveGesture.ReverseWaveDetected += _waveGesture_ReverseWaveDetected;

            // Get the list of Microsoft Bands paired to the phone.
            var pairedBands = await BandClientManager.Instance.GetBandsAsync();
            if (pairedBands.Length < 1)
            {
                throw new Exception("Can't find a band");
            }

            // Connect to Microsoft Band.
            _bandClient = await BandClientManager.Instance.ConnectAsync(pairedBands[0]);

            var sensors = _bandClient.SensorManager;

            sensors.Gyroscope.ReportingInterval = sensors.Gyroscope.SupportedReportingIntervals.Min();

            sensors.Gyroscope.ReadingChanged += (s, args) =>
            {
                _gyroscopeBuffer.Add(args.SensorReading);
                _waveGesture.AddAccelerometerReading(args.SensorReading);
            };

            SensorStatusTextBlock.Text = "Initialized!";
            Background = new Windows.UI.Xaml.Media.SolidColorBrush(Windows.UI.Colors.Green);

            await sensors.Gyroscope.StartReadingsAsync();

            return 0;
        }
        
        private async void _waveGesture_WaveDetected(object sender, EventArgs e)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                Background = new Windows.UI.Xaml.Media.SolidColorBrush(Windows.UI.Colors.White);
            });
        }

        private async void _waveGesture_ReverseWaveDetected(object sender, EventArgs e)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                Background = new Windows.UI.Xaml.Media.SolidColorBrush(Windows.UI.Colors.Blue);
            });
        }

        private void Button_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {

        }
        
        private void StartStopBufferButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            if(buffering)
            {
                StartStopBufferButton.Content = "Start Buffering";

                string s = "";
                foreach(var grec in _gyroscopeBuffer.ToArray())
                {
                    s += string.Format("{0},{1},{2}\n", grec.AccelerationX, grec.AccelerationY, grec.AccelerationZ);
                }
            }
            else
            {
                StartStopBufferButton.Content = "Stop Buffering";
                _gyroscopeBuffer.Clear();
            }

            buffering = !buffering;
        }
    }
}
