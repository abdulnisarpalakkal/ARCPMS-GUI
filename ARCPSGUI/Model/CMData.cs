using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ARCPSGUI.Model
{
    [Serializable]
    class CMData
    {
        public int cmPkId { get; set; }
        public string cmName { get; set; }
        public int floor { get; set; }


        public string cmChannel { get; set; }
        public string machineCode { get; set; }

        public int status { get; set; }
        public int position { get; set; }



        public string remCode { get; set; }



    }
}
