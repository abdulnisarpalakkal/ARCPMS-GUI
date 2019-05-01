using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace ARCPSGUI.Model
{
   public  class SlotData
    {
        public int SlotPkId { get; set; }
        public int SlotFloor { get; set; }
        public int SlotAisle { get; set; }
        public int SlotRow { get; set; }

        public string slotValue { get; set; }
        public int SlotType { get; set; }
        public int SlotStatus { get; set; }
        public int PrevSlotStatus { get; set; }

        public bool Disable { get; set; }
        public bool Rehandle { get; set; }

        public CarData ObjCarData { get; set; }

    }
}
