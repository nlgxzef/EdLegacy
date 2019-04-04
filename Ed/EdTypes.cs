using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ed
{
    public partial class EdTypes
    {
        public enum Game : int
        {
            MostWanted = 9,
            Carbon = 10,
            ProStreet = 11,
            Undercover = 12
        };

        public enum CarUsageType : int
        {
            Racer = 0,
            Cop = 1,
            Traffic = 2,
            Wheels = 3,
            Universal = 4
        };
    }
}
