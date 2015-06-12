﻿using Microsoft.Band;
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
            _bandClient = await BandClientManager.Instance.ConnectAsync(pairedBands[0]);

            var sensors = _bandClient.SensorManager;

            sensors.Gyroscope.ReportingInterval = sensors.Gyroscope.SupportedReportingIntervals.Min();

            sensors.Gyroscope.ReadingChanged += (s, args) =>
            {
                _waveGesture.AddAccelerometerReading(args.SensorReading);
            };

            SensorStatusTextBlock.Text = "Initialized!";
            Background = new Windows.UI.Xaml.Media.SolidColorBrush(Windows.UI.Colors.Green);

            await sensors.Gyroscope.StartReadingsAsync();

            return 0;
        }
        
        private async void _waveGesture_WaveDetected(object sender, EventArgs e)
        {
            Debug.WriteLine("Moving to next Slide");

            var httpClient = new HttpClient();
            var response = await httpClient.PostAsync("http://powerpointremoteproxy.azurewebsites.net/powerpoint/nextslide/1234", new StringContent(""));
            //var response = await httpClient.PostAsync("http://localhost:3283/powerpoint/nextslide/1234", new StringContent(""));

            Debug.WriteLine("Send signal to move to next Slide. Response: " + response.StatusCode);

            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                Background = new Windows.UI.Xaml.Media.SolidColorBrush(Windows.UI.Colors.White);
            });
        }

        private async void _waveGesture_ReverseWaveDetected(object sender, EventArgs e)
        {
            Debug.WriteLine("Moving to prev Slide");

            var httpClient = new HttpClient();
            var response = await httpClient.PostAsync("http://powerpointremoteproxy.azurewebsites.net/powerpoint/prevslide/1234", new StringContent(""));
            //var response = await httpClient.PostAsync("http://localhost:3283/powerpoint/prevslide/1234", new StringContent(""));

            Debug.WriteLine("Send signal to move to prev Slide. Response: " + response.StatusCode);

            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                Background = new Windows.UI.Xaml.Media.SolidColorBrush(Windows.UI.Colors.Blue);
            });
        }

        private void Button_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {

        }
    }
}
