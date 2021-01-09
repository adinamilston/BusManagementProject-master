using BLAPI;
using BO;
using System.Collections.Generic;

using DALAPI;
using System.Linq;
using System;
using System.Threading;
using System.Diagnostics;

namespace BL
{
    internal sealed class BLImp : IBL
    {
        IDAL dal = DALFactory.GetDAL();

        #region singelton
        static readonly BLImp instance = new BLImp();
        static BLImp() { }// static ctor to ensure instance init is done just before first usage
        private BLImp() => TripSimulator.Instance.Initialize(dal);
        public static BLImp Instance { get => instance; }// The public Instance property to use
        #endregion


        public void addBusLine(BusLine busLine)
        {
            throw new System.NotImplementedException();
        }

        public void addBusStation(BusStation busStation)
        {
            throw new System.NotImplementedException();
        }

        public void deleteBusLine(BusLine busLine)
        {
            throw new System.NotImplementedException();
        }

        public void deleteBusStation(BusStation busStation)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<BusStation> GetAllBusStations()
        {
            return from item in dal.GetAllBusStatins()
                   select BusStationDoBoAdapter(item);
        }
        private BO.BusStation BusStationDoBoAdapter(DO.BusStation busStationDO)
        {
            BO.BusStation busStationBO = new BO.BusStation();
            //  busStationDO.CopyPropertiesTo(busStationBO);
            busStationBO.Code = busStationDO.Code;
            busStationBO.Name = busStationDO.Name;
            busStationBO.Latitude = busStationDO.Latitude;
            busStationBO.Longitude = busStationDO.Longitude;
            return busStationBO;
        }

        public IEnumerable<BusStation> GetBusStations(int busLineID)
        {
            throw new System.NotImplementedException();
        }

        #region Bus Station

        public void updateBusLine(BusLine busLine, string name)
        {
            throw new System.NotImplementedException();
        }

        public void updateBusLine(BusLine busLine, BusLine newbusLine)
        {
            throw new System.NotImplementedException();
        }

        public void updateBusLine(BusLine busLine, int cost, string minmax)
        {
            throw new System.NotImplementedException();
        }

        public void updateBusStation(BusStation busStation, string name)
        {
            throw new System.NotImplementedException();
        }

        public void updateBusStation(BusStation busStation, BusStation newbusStation)
        {
            throw new System.NotImplementedException();
        }

        public void updateBusStation(BusStation busStation, int cost, string minmax)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<int> GetBusLineNumbers(int stationCode)
        {
            return (from ls in dal.GetLineStations(ls => ls.StationId == stationCode)
                    select dal.GetBusLine(ls.LineId).Code).Distinct();
        }
        #endregion

        #region Simulator
        public void StartSimulator(TimeSpan startTime, int rate, Action<TimeSpan> updateTime)
        {
            ClockSimulator.Instance.ClockObserver += updateTime;
            ClockSimulator.Instance.Start(startTime, rate);
        }
        public void StopSimulator() => ClockSimulator.Instance.Stop();

        public void SetStationPanel(int station, Action<LineTiming> updateBus)
        {
            TripSimulator.Instance.StationId = station;
            TripSimulator.Instance.BusObserver += updateBus;
        }
        #endregion
    }
}