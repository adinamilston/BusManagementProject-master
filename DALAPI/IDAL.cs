using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DO;

namespace DALAPI
{
    public interface IDAL
    {
        IEnumerable<BusStation> GetAllBusStatins();
        IEnumerable<LineStation> GetAllLineStations();
        IEnumerable<BusLine> GetAllBusLines();

        IEnumerable<LineTrip> GetLineSchedules(Predicate<LineTrip> filter);
        IEnumerable<LineStation> GetLineStations(Predicate<LineStation> filter);
        IEnumerable<BusStation> GetBusStations(Predicate<BusStation> filter);
        AdjacentStations GetAdjacentStations(int station1, int station2);
        BusStation GetBusStation(int id);
        BusLine GetBusLine(int id);

    }
}
