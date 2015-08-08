using Microsoft.Band.Portable.Sensors;
using System;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Subjects;
using System.Reactive.Linq;

namespace BandPowerpointRemote
{
    public class WaveGestureDetector
    {
        public event EventHandler WaveDetected;
        public event EventHandler ReverseWaveDetected;
        Subject<BandAccelerometerReading> rx = new Subject<BandAccelerometerReading>();
		Subject<BandGyroscopeReading> rxGyro = new Subject<BandGyroscopeReading>();

        //Track the last event so we can avoid duplicates
        private DateTime _lastEvent = DateTime.UtcNow;

        public WaveGestureDetector()
        {
            var wave = rx.Where(x => x.AccelerationZ > 2);
            wave.Subscribe(x =>
            {
                if (DateTime.UtcNow.Subtract(_lastEvent) > TimeSpan.FromSeconds(1.0))
                {
					Debug.WriteLine("Detected Z-accel of " + x.AccelerationZ);

                    _lastEvent = DateTime.UtcNow;
         
                    var eh = WaveDetected;
                    if (eh != null)
                    {
                        eh(this, EventArgs.Empty);
                    }
                }
            });

            var reverseWave = rx.Where(x => x.AccelerationZ < -2);
            reverseWave.Subscribe(x =>
            {
                if (DateTime.UtcNow.Subtract(_lastEvent) > TimeSpan.FromSeconds(1.0))
                {
					Debug.WriteLine("Detected Z-accel of " + x.AccelerationZ);

                    _lastEvent = DateTime.UtcNow;

                    var eh = ReverseWaveDetected;
                    if (eh != null)
                    {
                        eh(this, EventArgs.Empty);
                    }
                }
            });

			rx.Buffer (TimeSpan.FromSeconds (1)).Subscribe (x => {
				if(x.Count > 0)
					Debug.WriteLine(
						"Accel Reading: " +

						x.Average(y => y.AccelerationX) + "," +
						x.Average(y => y.AccelerationY) + "," +
						x.Average(y => y.AccelerationZ));
			});

			rxGyro.Buffer (TimeSpan.FromSeconds (1)).Subscribe (x => {
				if(x.Count > 0)
					Debug.WriteLine(
						"Gyro Reading: " +
						x.Average(y => y.AngularVelocityX) + "," +
						x.Average(y => y.AngularVelocityY) + "," +
						x.Average(y => y.AngularVelocityZ));
			});
        }

        public void AddAccelerometerReading(BandAccelerometerReading accelerometerReading)
        {
            rx.OnNext(accelerometerReading);
        }

		public void AddGyroscopeReading(BandGyroscopeReading gyroReading)
		{
			rxGyro.OnNext (gyroReading);
		}
    }
}
