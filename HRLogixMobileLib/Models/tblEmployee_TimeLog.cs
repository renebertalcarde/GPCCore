using System.ComponentModel.DataAnnotations;
using System;
using System.Linq;
using System.Runtime.CompilerServices;

namespace HRLogixMobileLib.Models
{
    public class tblEmployee_TimeLog
    {
        public int LogId { get; set; }
     
        public DateTime TimeLog { get; set; }
        public string EntryType { get; set; }

        public string Mode { get; set; }

        public string DeviceReference { get; set; }
    
        public bool IsSelected { get; set; }

        public bool IsOfflineLog
        {
            get => LogId == -1;
        }
    }
}