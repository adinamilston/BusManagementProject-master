using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DO
{
    public class BusLine
    {
        public int ID { get; set; }
        public int Code { get; set; }
        public int FirstStation { get; set; }
        public int LastStation { get; set; }
        public override string ToString() => $"ID:{ID}, Code:{Code}, First:{FirstStation}, Last:{LastStation}";
    }
}
