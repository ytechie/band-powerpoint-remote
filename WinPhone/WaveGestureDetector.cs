using Microsoft.Band.Sensors;
using System;
using System.Linq;
using System.Reactive.Subjects;
using System.Reactive.Linq;

namespace WinPhone
{
    public class WaveGestureDetector
    {
        public event EventHandler WaveDetected;
        public event EventHandler ReverseWaveDetected;
        Subject<IBandAccelerometerReading> rx = new Subject<IBandAccelerometerReading>();

        //Track the last event so we can avoid duplicates
        private DateTime _lastEvent = DateTime.UtcNow;

        public WaveGestureDetector()
        {
            var wave = rx.Where(x => x.AccelerationY > 3);
            wave.Subscribe(x =>
            {
                if (DateTime.UtcNow.Subtract(_lastEvent) > TimeSpan.FromSeconds(1.0))
                {
                    _lastEvent = DateTime.UtcNow;
         
                    var eh = WaveDetected;
                    if (eh != null)
                    {
                        eh(this, EventArgs.Empty);
                    }
                }
            });

            var reverseWave = rx.Where(x => x.AccelerationY < -3);
            reverseWave.Subscribe(x =>
            {
                if (DateTime.UtcNow.Subtract(_lastEvent) > TimeSpan.FromSeconds(1.0))
                {
                    _lastEvent = DateTime.UtcNow;

                    var eh = ReverseWaveDetected;
                    if (eh != null)
                    {
                        eh(this, EventArgs.Empty);
                    }
                }
            });
        }

        public void AddAccelerometerReading(IBandAccelerometerReading accelerometerReading)
        {
            rx.OnNext(accelerometerReading);
        }
    }
}
