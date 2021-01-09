using System.Collections.Generic;

namespace BO
{
    public class BusLine
    {
        public int ID { get; set; }
        public int Code { get; set; }
        public int FirstStation { get; set; }
        public int LastStation { get; set; }
        IEnumerable<LineStation> Stations { get; set; }
        public override string ToString()
        {
            string str = $"ID:{ID}, Code:{Code}, First:{FirstStation}, Last:{LastStation}\n";
            foreach (var s in Stations)
                str += $" {s.StationId}:{s.Name}";
            return str;
        }

    }
}