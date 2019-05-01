using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ARCPSGUI.Model
{
    class EESZoneData
    {
        public int EESId { get; set; }
        public string EESName { get; set; }
        public int BaseZoneStart { get; set; }
        public int BaseZoneEnd { get; set; }
        public int NonBaseZoneStart { get; set; }
        public int NonBaseZoneEnd { get; set; }
        public int BaseRefAisle { get; set; }
        public int NonBaseRefAisle { get; set; }
    }
}
