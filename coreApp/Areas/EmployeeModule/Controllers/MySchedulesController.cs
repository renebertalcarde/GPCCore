using coreApp.Areas.TimeModule.Time;
using coreApp.Controllers;
using coreApp.DAL;
using coreApp.Enums;
using coreApp.Filters;
using coreApp.Interfaces;
using coreLib.Enums;
using coreLib.Objects;
using Module.DB;
using Module.Time;
using System;
using System.Web.Mvc;

namespace coreApp.Areas.EmployeeModule.Controllers
{
    [MyFilter]
    public class MySchedulesController : Base_NoCoreEmployeeController, IMyController
    {
        public tblEmployee employee { get; set; }

        public ActionResult Index(string startDate, string endDate)
        {
            PeriodModel pm = new PeriodModel(PeriodModelInitType.ThisMonth);
            DateTime tmp;

            if (DateTime.TryParse(startDate, out tmp))
            {
                pm.StartDate = tmp;
            }

            if (DateTime.TryParse(endDate, out tmp))
            {
                pm.EndDate = tmp;
            }

            etPeriodModel model = new etPeriodModel(employee.EmployeeId, pm.StartDate, pm.EndDate, null);

            return View(model);
        }
    }
}
