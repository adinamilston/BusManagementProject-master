using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BO
{
    /// <summary>
    /// The class represents a bus (of certain line) arriving soon to the bus station
    /// </summary>
    public class LineTiming
    {
        /// <summary>
        ///  for debugging
        /// </summary>
        private static int counter = 0;
        public int ID;
        public LineTiming() => ID = ++counter;
        public TimeSpan TripStart { get; set; }
        /// <summary>
        /// Line ID (unique)
        /// </summary>
        public int LineId { get; set; }
        /// <summary>
        /// Line Number as understood by the people
        /// </summary>
        public int LineCode { get; set; }
        /// <summary>
        /// Last station name - so the passengers will now better which direction it is
        /// </summary>
        public string LastStation { get; set; }
        /// <summary>
        /// Expected time of arrival
        /// </summary>
        public TimeSpan Timing { get; set; }
    }
}
