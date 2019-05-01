using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ARCPSGUI.Model
{
    class MachineJobData
    {
        public string Machine { get; set; }
        public string Mode { get; set; }
        public int Floor { get; set; }
        public int Aisle { get; set; }
        public int Row { get; set; }
        public string Status { get; set; }
    }
}
