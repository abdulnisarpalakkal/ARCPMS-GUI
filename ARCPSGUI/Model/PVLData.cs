using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ARCPSGUI.Model
{
    [Serializable]
    class PVLData
    {
        public int pvlPkId { get; set; }
        public string pvlName { get; set; }
        public string machineChannel { get; set; }
        public string machineCode { get; set; }

        public int aisle { get; set; }
        public int row { get; set; }

        public int status { get; set; }





    }
}
