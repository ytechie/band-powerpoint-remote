using Microsoft.Band;
using Microsoft.Band.Sensors;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using System.Linq;
using Windows.UI.Core;
using System.Net.Http;
using System.Diagnostics;

namespace WinPhone
{
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Required;

            this.Unloaded += MainPage_Unloaded;
        }

        private void MainPage_Unloaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            _bandClient.Dispose();
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            await StartBandMonitor();
        }

        private bool buffering = false;

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
            var connected = false;
            while (!connected)
            {
                try
                {
                    _bandClient = await BandClientManager.Instance.ConnectAsync(pairedBands[0]);
                    connected = true;
                }
                catch (Exception connectEx)
                {
                    await Task.Delay(2000);
                }
            }

            var sensors = _bandClient.SensorManager;

            sensors.Gyroscope.ReportingInterval = sensors.Gyroscope.SupportedReportingIntervals.Min();

            sensors.Gyroscope.ReadingChanged += (s, args) =>
            {
                _waveGesture.AddAccelerometerReading(args.SensorReading);
            };

            //if (await sensors.HeartRate.RequestUserConsentAsync())
            //{ 
            //    sensors.HeartRate.ReadingChanged += (s, args) =>
            //    {
            //        this.SendHeartRate(args.SensorReading);
            //    };
            //}

            SensorStatusTextBlock.Text = "Initialized!";
            Background = new Windows.UI.Xaml.Media.SolidColorBrush(Windows.UI.Colors.Green);

            connected = false;
            while (!connected)
            {
                try
                {
                    await sensors.Gyroscope.StartReadingsAsync();
                    await sensors.HeartRate.StartReadingsAsync();
                    connected = true;
                }
                catch(Exception)
                {
                    connected = false;
                }
            }

            return 0;
        }
        
        private async void _waveGesture_WaveDetected(object sender, EventArgs e)
        {
            Debug.WriteLine("Moving to next Slide");

            var httpClient = new HttpClient();
            var response = await httpClient.PostAsync("http://powerpointremoteproxy.azurewebsites.net/powerpoint/nextslide/1234", new StringContent(""));
            //var response = await httpClient.PostAsync("http://localhost:3283/powerpoint/nextslide/1234", new StringContent(""));

            Debug.WriteLine("Send signal to move to next Slide. Response: " + response.StatusCode);

            try
            {
                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    Background = new Windows.UI.Xaml.Media.SolidColorBrush(Windows.UI.Colors.White);
                });
            }
            catch (Exception ex)
            {

            }
        }

        private async void _waveGesture_ReverseWaveDetected(object sender, EventArgs e)
        {
            Debug.WriteLine("Moving to prev Slide");

            var httpClient = new HttpClient();
            var response = await httpClient.PostAsync("http://powerpointremoteproxy.azurewebsites.net/powerpoint/prevslide/1234", new StringContent(""));
            //var response = await httpClient.PostAsync("http://localhost:3283/powerpoint/prevslide/1234", new StringContent(""));

            Debug.WriteLine("Send signal to move to prev Slide. Response: " + response.StatusCode);

            try
            {
                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    Background = new Windows.UI.Xaml.Media.SolidColorBrush(Windows.UI.Colors.Blue);
                });
            }
            catch (Exception ex)
            {

            }
        }

        private void Button_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {

        }

        private async void SendHeartRate(IBandHeartRateReading reading)
        {
            await EventHubsInterface.SendAccelerometerReading(reading);
        }
    }
}
