using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ARCPSGUI.Model
{
    public class DBLogData
    {
        public decimal TrackId {get; set;}
        public decimal QueueId {get; set;}
        public string Message { get; set;}
        public DateTime Time {get; set;}
        public string PackageName {get; set;}
        public string ProcedureName {get; set;}
   
    }
}
