using Microsoft.Band.Portable.Sensors;
using System;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Subjects;
using System.Reactive.Linq;
using System.Collections.Generic;
using System.Text;

namespace BandPowerpointRemote
{
    public class WaveGestureDetector
    {
        public event EventHandler WaveDetected;
        public event EventHandler ReverseWaveDetected;
        Subject<BandAccelerometerReading> rx = new Subject<BandAccelerometerReading>();
		Subject<BandGyroscopeReading> rxGyro = new Subject<BandGyroscopeReading>();

        private const int MaxQueueBufferSize = 500;
        Queue<BandAccelerometerReading> _readings = new Queue<BandAccelerometerReading>();

        private const double GForceThreshold = 1.9;

        //Track the last event so we can avoid duplicates
        private DateTime _lastEvent = DateTime.UtcNow;

        public WaveGestureDetector()
        {
            var wave = rx.Where(x => x.AccelerationZ > GForceThreshold);
            wave.Subscribe(x =>
            {
                if (DateTime.UtcNow.Subtract(_lastEvent) > TimeSpan.FromSeconds(1.0))
                {
					Debug.WriteLine("Detected Z-accel of " + x.AccelerationZ);

#if DEBUG
                    var csv = GetReadingsQueueCsv();
#endif

                    _lastEvent = DateTime.UtcNow;
         
                    var eh2 = WaveDetected;
                    if (eh2 != null)
                    {
                        eh2(this, EventArgs.Empty);
                    }
                }
            });

            var reverseWave = rx.Where(x => x.AccelerationZ < -GForceThreshold);
            reverseWave.Subscribe(x =>
            {
                if (DateTime.UtcNow.Subtract(_lastEvent) > TimeSpan.FromSeconds(1.0))
                {
					Debug.WriteLine("Detected Z-accel of " + x.AccelerationZ);

#if DEBUG
                    var csv = GetReadingsQueueCsv();
#endif

                    _lastEvent = DateTime.UtcNow;

                    var eh = ReverseWaveDetected;
                    if (eh != null)
                    {
                        eh(this, EventArgs.Empty);
                    }
                }
            });

            rx.Buffer(TimeSpan.FromSeconds(1)).Subscribe(x =>
            {
                if (x.Count > 0)
                    Debug.WriteLine(
                        "Accel Reading: " +

                        x.Average(y => y.AccelerationX) + "," +
                        x.Average(y => y.AccelerationY) + "," +
                        x.Average(y => y.AccelerationZ));
            });

            rxGyro.Buffer(TimeSpan.FromSeconds(1)).Subscribe(x =>
            {
                if (x.Count > 0)
                    Debug.WriteLine(
                        "Gyro Reading: " +
                        x.Average(y => y.AngularVelocityX) + "," +
                        x.Average(y => y.AngularVelocityY) + "," +
                        x.Average(y => y.AngularVelocityZ));
            });
        }

        public void AddAccelerometerReading(BandAccelerometerReading accelerometerReading)
        {
            _readings.Enqueue(accelerometerReading);
            if(_readings.Count > MaxQueueBufferSize)
            {
                _readings.Dequeue();
            }

            rx.OnNext(accelerometerReading);
        }

		public void AddGyroscopeReading(BandGyroscopeReading gyroReading)
		{
			rxGyro.OnNext (gyroReading);
		}

        private string GetReadingsQueueCsv()
        {
            var csv = new StringBuilder();

            foreach(var reading in _readings)
            {
                csv.Append(reading.AccelerationX + "," + reading.AccelerationY + "," + reading.AccelerationZ + "\n");
            }

            return csv.ToString();
        }
    }
}
