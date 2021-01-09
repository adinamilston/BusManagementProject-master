using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DALAPI;
using DO;
//using DO;
using DS;

namespace DL
{
    sealed class DalObject : IDAL
    {
        #region singelton
        static readonly DalObject instance = new DalObject();
        static DalObject() { }// static ctor to ensure instance init is done just before first usage
        DalObject() { } // default => private
        public static DalObject Instance { get => instance; }// The public Instance property to use
        #endregion

        public IEnumerable<BusLine> GetAllBusLines()
        {
            return from busLine in DataSource.BusLines
                   select busLine.Clone();
        }
        public IEnumerable<DO.BusStation> GetAllBusStatins()
        {
            return from busStation in DataSource.BusStations
                   select busStation.Clone();
        }

        public IEnumerable<LineStation> GetAllLineStations()
        {
                return from lineStation in DataSource.LineStations
                       select lineStation.Clone();
        }

        public IEnumerable<LineTrip> GetLineSchedules(Predicate<LineTrip> filter)
        {
            return from ls in DataSource.LineTrips
                   where filter(ls)
                   select ls.Clone();
        }

        public IEnumerable<LineStation> GetLineStations(Predicate<LineStation> filter)
        {
            return from ls in DataSource.LineStations
                   where filter(ls)
                   select ls.Clone();
        }

        public AdjacentStations GetAdjacentStations(int station1, int station2) => DataSource.AdjacentStationsList.Find(ast => ast.Station1 == station1 && ast.Station2 == station2);

        public IEnumerable<BusStation> GetBusStations(Predicate<BusStation> filter)
        {
            return from bs in DataSource.BusStations
                   where filter(bs)
                   select bs.Clone();
        }

        public BusStation GetBusStation(int id)
        {
            return DataSource.BusStations.Find(bs => bs.Code == id).Clone();
        }

        public BusLine GetBusLine(int id)
        {
            return DataSource.BusLines.Find(bl => bl.ID == id).Clone();
        }
    }
}
