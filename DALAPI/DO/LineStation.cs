using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DO
{
    public class LineStation
    {
        public int LineId { get; set; }
        public int StationId { get; set; }
        public int LineStationIndex { get; set; }
        public override string ToString() => $"Line:{LineId}, Station:{StationId}, Index:{LineStationIndex}";
    }
}
