using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace coreApp.Areas.AdminModule.Models
{
    public class objDayLog
    {
        public DateTime Date;
        public List<objLog> Logs = new List<objLog>();
    }

    public class objLog
    {
        public DateTime Time;
        public string Description;
    }
}