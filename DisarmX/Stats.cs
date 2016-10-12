using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;

namespace DisarmX
{
    [Serializable()]
    public class Stats
    {
        public int gameTime { get; set; }
        public int gameResult { get; set; } // 0 or 1 (loose or win)
        public int gameLevel { get; set; } // 1, 2, 3, 4 - custom
        public double fieldsDiscovered { get; set; } // percentage
        public int minesMarked { get; set; }
        public DateTime dateTime { get; set; }
    }
}
