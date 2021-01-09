using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace BL
{
    internal sealed class ClockSimulator
    {
        #region singelton
        static readonly ClockSimulator instance = new ClockSimulator();
        static ClockSimulator() { }// static ctor to ensure instance init is done just before first usage
        ClockSimulator() { } // default => private
        internal static ClockSimulator Instance { get => instance; }// The public Instance property to use
        #endregion

        internal class Clock // this class is build to avoid inter-thread issues reading/writing the timespan value
        {
            internal TimeSpan Time;
            internal Clock(TimeSpan timespan) => Time = timespan;
        }

        internal bool Cancel { get; set; }
        private volatile Clock simulatorClock = null;
        internal Clock SimulatorClock => simulatorClock;
        private volatile int simulatorRate;
        internal int Rate => simulatorRate;

        private Stopwatch stopwatch = new Stopwatch();
        private Action<TimeSpan> clockObserver = null;
        internal event Action<TimeSpan> ClockObserver
        {
            add => clockObserver = value;
            remove => clockObserver = null;
        }
        private TimeSpan simulatorStartTime;

        internal void Start(TimeSpan startTime, int rate)
        {
            simulatorStartTime = startTime;
            simulatorClock = new Clock(startTime);
            simulatorRate = rate;
            Cancel = false;
            stopwatch.Restart();
            new Thread(clockThread).Start();
            TripSimulator.Instance.Start();
        }

        internal void Stop() => Cancel = true;

        void clockThread()
        {
            while (!Cancel)
            {
                simulatorClock = new Clock(simulatorStartTime + new TimeSpan(stopwatch.ElapsedTicks * simulatorRate));
                clockObserver(new TimeSpan(simulatorClock.Time.Hours, simulatorClock.Time.Minutes, simulatorClock.Time.Seconds));
                Thread.Sleep(1000);
            }
            clockObserver = null;
        }
    }
}
