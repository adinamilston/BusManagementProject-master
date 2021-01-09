using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DO;

namespace DS
{
    public static class DataSource
    {
        public static List<BusStation> BusStations;
        public static List<BusLine> BusLines;
        public static List<AdjacentStations> AdjacentStationsList;
        public static List<LineStation> LineStations;
        public static List<LineTrip> LineTrips;

        static int s_lineID = 0;

        static DataSource()
        {
            BusStations = new List<BusStation>();
            BusLines = new List<BusLine>();
            LineStations = new List<LineStation>();
            AdjacentStationsList = new List<AdjacentStations>();
            LineTrips = new List<LineTrip>();
            InitAllLists();
        }

        #region Create Random data
        static Random rand = new Random();

        private static void InitAllLists()
        {
            GenerateBusStations(50);
            GenerateAdjacentStations();
            GenerateBusLines(20);
            GenerateLineSchedules();
        }

        private static void GenerateAdjacentStations()
        {
            AdjacentStationsList = (from st1 in BusStations
                                    let station1 = st1.Code
                                    from st2 in BusStations
                                    let station2 = st2.Code
                                    where st1 != st2
                                    let distance = Distance(station1, station2) * (1 + rand.NextDouble() / 2)
                                    let seconds = (int)(distance / (rand.NextDouble() * 30 + 30) * 3600)
                                    orderby distance
                                    select new AdjacentStations
                                    {
                                        Station1 = station1,
                                        Station2 = station2,
                                        Distance = distance,
                                        AverageTime = new TimeSpan(seconds * TimeSpan.TicksPerSecond)
                                    }).ToList();
        }

        static int s_lineScheduleId = 0;
        private static void GenerateLineSchedules()
        {
            BusLines.ForEach(bl =>
            {
                int trips = rand.Next(20, 50);
                for (int i = 0; i < trips; ++i)
                {
                    LineTrips.Add(new LineTrip
                    {
                        ID = ++s_lineScheduleId,
                        LineId = bl.ID,
                        StartAt = new TimeSpan(6, 0, 0) + new TimeSpan(rand.Next(0, 1000) * TimeSpan.TicksPerMinute)
                    });
                }
            });
        }

        private static void GenerateBusStations(int amount)
        {
            for (int i = 0; i < amount; i++)
                BusStations.Add(getRandomBusStation());
        }

        private static void GenerateBusLines(int anount)
        {
            for (int i = 0; i < anount; i++)
                BusLines.Add(getRandomBusLine());
        }

        private static BusLine getRandomBusLine()
        {
            int id = ++s_lineID;
            int busLineNum = rand.Next(1, 1000);
            int routeLength = rand.Next(8, 20);
            List<int> stationIds = getRandomListStationIds(routeLength);
            routeLength = stationIds.Count();
            int first = stationIds[0];
            int last = stationIds[routeLength - 1];
            for (int i = 0; i < routeLength; i++)
            {
                LineStations.Add(
                    new LineStation
                    {
                        LineId = id,
                        StationId = stationIds[i],
                        LineStationIndex = i
                    });
            }
            return new BusLine
            {
                ID = id,
                Code = busLineNum,
                FirstStation = first,
                LastStation = last
            };
        }



        private static List<int> getRandomListStationIds(int length)
        {
           var nearestCouple = AdjacentStationsList.GetRange(0, length);
           nearestCouple = nearestCouple.OrderBy(i => Guid.NewGuid()).ToList();
            List<int> result = new List<int>();
            foreach (var adjacentStation in nearestCouple)
            {
                result.Add(adjacentStation.Station1);
                result.Add(adjacentStation.Station2);
            }
            return result.Distinct().ToList();
            //return BusStations.Select(s => s.Code).OrderBy(item => rand.Next()).
            //                   ToList().GetRange(0, length);
        }

        private static int getRandomBusStationCode()
        {
            List<int> StationIDs = BusStations.Select(s => s.Code).ToList();
            return StationIDs[rand.Next(StationIDs.Count)];
        }

        private static BusStation getRandomBusStation()
        {
            int stationCode = rand.Next(100000, 200000);
            Double latitude = rand.Next(31, 33) + rand.NextDouble();
            Double longitude = rand.Next(34, 35) + rand.NextDouble();
            int chars = rand.Next(6, 12);
            string name = "";
            for (int i = 0; i < chars; ++i)
            {
                int r = rand.Next(0, 26);
                name += r == 0 ? ' ' : (char)('a' - 1 + r);
            }
            return new BusStation
            {
                Code = stationCode,
                Name = name,
                Latitude = latitude,
                Longitude = longitude
            };
        }

        static double Distance(int from, int to)
        {
            BusStation station1 = BusStations.FirstOrDefault(station => station.Code == from);
            BusStation station2 = BusStations.FirstOrDefault(station => station.Code == to);
            int R = 6371 * 1000; // metres
            double phi1 = station1.Latitude * Math.PI / 180; // φ, λ in radians
            double phi2 = station2.Latitude * Math.PI / 180;
            double deltaPhi = (station2.Latitude - station1.Latitude) * Math.PI / 180;
            double deltaLambda = (station2.Longitude - station1.Longitude) * Math.PI / 180;

            double a = Math.Sin(deltaPhi / 2) * Math.Sin(deltaPhi / 2) +
                      Math.Cos(phi1) * Math.Cos(phi2) *
                      Math.Sin(deltaLambda / 2) * Math.Sin(deltaLambda / 2);
            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            double d = R * c / 1000; // in kilometres
            return d;
        }


        #endregion
    }
}
