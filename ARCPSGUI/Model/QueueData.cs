using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ARCPSGUI.Model
{
    public class QueueData
    {
        public enum CAR_TYPE
        {
            low = 1,
            high = 2,
            medium = 3
        };
        public int queuePkId { get; set; }
        public string eesCode { get; set; }
        public int eesNumber { get; set; }
        public string kioskId { get; set; }

        public string customerId { get; set; }
        public int customerPkId { get; set; }
        public int requestType { get; set; }
        public int priority { get; set; }
        public int status { get; set; }

        public DateTime procStartTime { get; set; }
        public DateTime procEndTime { get; set; }
        public string assignedThreadId { get; set; }
        public CAR_TYPE carType { get; set; }

        public string gate { get; set; }
        public int cancelReqType { get; set; }
        public bool isAborted { get; set; }
        public int retrievalType { get; set; }
        public bool needWash { get; set; }

        public int washStatus { get; set; }
        public string patronName { get; set; }
        public bool isMember { get; set; }
        public string plateNumber { get; set; }

        public bool isEntry { get; set; }
        public bool needSimulation { get; set; }
        public bool isRotate { get; set; }
    }
}
