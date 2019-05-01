using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ARCPSGUI.Model
{
    public class NotificationData
    {
        public int Id { get; set; }
        public string MachineCode { get; set; }
        public string OPCTag { get; set; }
        public enum errorCategory { NA, ERROR, MANUAL, TRIGGER, DISABLE, WAITING };
        public errorCategory category { get; set; }
        public string ErrorCode { get; set; }
        public string ErrorDescription { get; set; }
        public bool IsCleared { get; set; }
        public DateTime NotifyTime { get; set; }
        public DateTime FilterNotifyStart { get; set; }
        public DateTime FilterNotifyEnd { get; set; }
    }
}
