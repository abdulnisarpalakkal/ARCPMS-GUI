using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ARCPSGUI.Model
{
    public class EESWaitData
    {
        public decimal sno { get; set; }
        public decimal WaitHistID { get; set; }
        public string WaitGate { get; set; }
        public DateTime WaitUpdateTime { get; set; }
        public string WaitLocation { get; set; }
        public int WaitLocationId { get; set; }
        public string WaitTime { get; set; }
        
    }
}
