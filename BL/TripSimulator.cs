using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using BO;
using DALAPI;

namespace BL
{
    internal sealed class TripSimulator
    {
        #region singelton
        static readonly TripSimulator instance = new TripSimulator();
        static TripSimulator() { }// static ctor to ensure instance init is done just before first usage
        TripSimulator() { } // default => private
        internal static TripSimulator Instance { get => instance; }// The public Instance property to use
        #endregion

        private static Random rand = new Random();
        private IDAL dal = null;

        private int stationId = -1;
        internal int StationId { set => stationId = value; }

        private event Action<LineTiming> busObserver;
        internal event Action<LineTiming> BusObserver
        {
            add => busObserver = value;
            remove => busObserver = null;
        }

        internal void Initialize(IDAL dal) => this.dal = dal;

        internal void Start() => new Thread(runBusTripsThread).Start();

        void runBusTripsThread()
        {
            List<DO.LineTrip> scheds = (from sc in dal.GetLineSchedules(s => true)
                                            orderby sc.StartAt
                                            select sc).ToList();
            while (!ClockSimulator.Instance.Cancel)
            {
                foreach (var sc in scheds)
                {
                    if (ClockSimulator.Instance.Cancel) break;
                    TimeSpan clock = ClockSimulator.Instance.SimulatorClock.Time;
                    if (clock > sc.StartAt) continue;
                    Thread.Sleep((int)((sc.StartAt - clock).TotalMilliseconds / ClockSimulator.Instance.Rate));
                    if (ClockSimulator.Instance.Cancel) break;
                    new Thread(busTripThread).Start(sc);
                }
                Thread.Sleep(1000); // protect CPU
            }
        }

        void busTripThread(object lineSchedule)
        {
            DO.LineTrip sc = (DO.LineTrip)lineSchedule;
            int station = stationId;
            LineTiming lineTiming = new LineTiming {
                LineId = sc.LineId,
                LineCode = dal.GetBusLine(sc.LineId).Code,
                TripStart = sc.StartAt
            };
            Thread.CurrentThread.Name = $"{lineTiming.ID}:{lineTiming.LineId}/{lineTiming.LineCode}";
            var r1 = (from st in dal.GetLineStations(s => s.LineId == lineTiming.LineId)
                      orderby st.LineStationIndex
                      select st).ToList();
            if (r1.Count == 0) return;
            var route = (from st in r1
                         select st.CopyPropertiesToNew<DO.LineStation, BO.LineStation>()).ToList();
            route.ForEach(ls => ls.Name = dal.GetBusStation(ls.StationId).Name);
            for (int i = 1; i < route.Count; ++i)
                route[i].AverageTime = dal.GetAdjacentStations(route[i - 1].StationId, route[i].StationId).AverageTime;
            lineTiming.LastStation = route[route.Count - 1].Name;

            for (int i = 0; i < route.Count; ++i)
            {
                if (station != stationId)
                {
                    lineTiming.Timing = TimeSpan.Zero;
                    busObserver(lineTiming); // Reached the station...
                    lineTiming = new LineTiming {
                        ID = lineTiming.ID,
                        LineId = lineTiming.LineId,
                        LineCode = lineTiming.LineCode, 
                        LastStation = lineTiming.LastStation,
                        TripStart = lineTiming.TripStart
                    };
                    station = stationId;
                }
                if (ClockSimulator.Instance.Cancel) break;
                if (stationId == route[i].StationId)
                { // changed the monitored station
                    lineTiming.Timing = TimeSpan.Zero;
                    busObserver(lineTiming); // Reached the station...
                }
                TimeSpan total = TimeSpan.Zero;
                for (int j = i + 1; j < route.Count; ++j)
                {
                    total += route[j].AverageTime;
                    if (stationId == route[j].StationId)
                    {
                        lineTiming.Timing = total;
                        busObserver(lineTiming);
                        break;
                    }
                }
                if (i + 1 < route.Count)
                    Thread.Sleep((int)(route[i + 1].AverageTime.TotalMilliseconds * (0.9 + rand.NextDouble() / 2) / ClockSimulator.Instance.Rate));
            }
        }
    }
}
