using System;

namespace BO
{
    public class LineStation
    {
        public int StationId { get; set; }
        public string Name { get; set; }
        public double Distance { get; set; }
        public TimeSpan AverageTime { get; set; } 
        public override string ToString() => $"Station:{StationId}, Dist:{Distance}, Time:{AverageTime}";

    }
}