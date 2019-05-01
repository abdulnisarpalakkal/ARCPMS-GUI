using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ARCPSGUI.Model
{
    public class CarData
    {
        public int CustomerId { get; set; }
        public bool IsMember { get; set; }
        public string Plate { get; set; }
        public DateTime EntryTime { get; set; }
         public int CarType { get; set; }

        public string CardId { get; set; }
        public string EntryGate { get; set; }
        public bool IsRotated { get; set; }
 

        public bool Wash { get; set; }
        public bool IsWashComplete { get; set; }
        
        public int EntryQueueId { get; set; }
        public DateTime PutToSlotTime { get; set; }
        public string EntryNorthImg { get; set; }
        public string EntrySouthImg { get; set; }

        public string PatronName { get; set; }
        public bool Disable { get; set; }


    }
}
