using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DO
{
    public class LineTrip
    {
        public int ID { get; set; }
        public int LineId { get; set; }
        public TimeSpan StartAt { get; set; }
        public override string ToString() => $"ID:{ID}, Line:{LineId} --- Leaves at: {StartAt}";
    }
}
