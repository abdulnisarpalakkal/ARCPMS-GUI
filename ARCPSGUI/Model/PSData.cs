using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ARCPSGUI.Model
{
    [Serializable]
    class PSData
    {
        public int psPkId { get; set; }
        public string psName { get; set; }
        public string machineCode { get; set; }
        public string machineChannel { get; set; }
        public int position { get; set; }

        public int status { get; set; }


    }
}
