using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ARCPSGUI.Model
{
    public class EESData
    {
        public int eesPkId { get; set; }
        public string eesName { get; set; }
        public int aisle { get; set; }
        public int row { get; set; }
       
        public string machineCode { get; set; }
        public string machineChannel { get; set; }
       
        public int status { get; set; }

        public int eesState { get; set; }
        public bool vehicleDetectorStatus { get; set; }
        public bool innerDoorBlockStatus { get; set; }
      
    }
}
