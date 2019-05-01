using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ARCPSGUI.Model
{
    [Serializable]
    class PSTData
    {

        public int pstPkId { get; set; }
        public string pstName { get; set; }
        public int aisle { get; set; }
        public int row { get; set; }

        public string machineChannel { get; set; }
        public string machineCode { get; set; }

        public int status { get; set; }


    }
}
