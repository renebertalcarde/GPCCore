using Module.DB;
using Module.Time;
using System.Collections.Generic;

namespace coreApp.Models
{

    public class AttendanceMonitoringModel
    {
        public tblEmployee employee { get; set; }
        public etPeriodModel periodModel { get; set; }
    }
}