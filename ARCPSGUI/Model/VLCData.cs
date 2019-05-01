using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ARCPSGUI.Model
{
    [Serializable]
    class VLCData
    {
        public int vlcPkId { get; set; }
        public string vlcName { get; set; }
        public int row { get; set; }
        public int aisle { get; set; }

        public string machineCode { get; set; }
        public string vlcDeckCode { get; set; }
        public string machineChannel { get; set; }
        public int floor { get; set; }
        public int position { get; set; }

        public int status { get; set; }

    }
}
