using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BO;

namespace BLAPI
{
    public interface IBL
    {
        #region BusStation
        IEnumerable<BusStation> GetAllBusStations();
        void addBusStation(BusStation busStation);
        void deleteBusStation(BusStation busStation);
        void updateBusStation(BusStation busStation, string name);
        void updateBusStation(BusStation busStation, BusStation newbusStation);
        void updateBusStation(BusStation busStation, int cost, string minmax);
        
        #endregion

        #region BusLine
        void addBusLine(BusLine busLine);
        void deleteBusLine(BusLine busLine);
        void updateBusLine(BusLine busLine, string name);
        void updateBusLine(BusLine busLine, BusLine newbusLine);
        void updateBusLine(BusLine busLine, int cost, string minmax);
        IEnumerable<BusStation> GetBusStations(int busLineID);
        IEnumerable<int> GetBusLineNumbers(int stationCode);
        #endregion

        void StartSimulator(TimeSpan ts, int Rate, Action<TimeSpan> updateTime);
        void StopSimulator();
        void SetStationPanel(int station, Action<LineTiming> updateBus);
    }
}
