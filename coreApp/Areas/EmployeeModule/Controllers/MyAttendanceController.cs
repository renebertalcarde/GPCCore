using System.Web.Mvc;
using System;
using coreApp.Controllers;
using coreApp.Interfaces;
using coreApp.Filters;
using Module.DB;
using Module.Time;
using coreLib.Objects;
using coreLib.Enums;
using coreApp.DAL;
using reports.AsposeLib;

namespace coreApp.Areas.EmployeeModule.Controllers
{
    [MyFilter]
    public class MyAttendanceController : Base_NoCoreEmployeeController, IMyController
    {
        public tblEmployee employee { get; set; }

        public ActionResult Index(string startDate, string endDate, string filters = "")
        {
            PeriodModel pm = new PeriodModel(PeriodModelInitType.ThisMonth);
            DateTime tmp;
            etPeriodFilter filter = null;

            if (DateTime.TryParse(startDate, out tmp))
            {
                pm.StartDate = tmp;
            }

            if (DateTime.TryParse(endDate, out tmp))
            {
                pm.EndDate = tmp;
            }

            if (!string.IsNullOrEmpty(filters))
            {
                filter = new etPeriodFilter(filters);
            }

            AttendanceModel model = new AttendanceModel { periodModel = pm };

            model.period = new etPeriod(employee.EmployeeId, pm.StartDate, pm.EndDate, filter: filter);

            return View(model);
        }

        public ActionResult Print(string startDate = "", string endDate = "", string filters = "")
        {
            PeriodModel pm = new PeriodModel(PeriodModelInitType.ThisMonth);
            DateTime tmp;
            etPeriodFilter filter = null;

            if (DateTime.TryParse(startDate, out tmp))
            {
                pm.StartDate = tmp;
            }

            if (DateTime.TryParse(endDate, out tmp))
            {
                pm.EndDate = tmp;
            }

            if (!string.IsNullOrEmpty(filters))
            {
                filter = new etPeriodFilter(filters);
            }

            using (hr2017DataContext context = new hr2017DataContext())
            {
                etPeriod data = new etPeriod(employee.EmployeeId, pm.StartDate, pm.EndDate, null, filter: filter);

                string templateName = FixedSettings.DTRType;

                objData CustomData = new objData
                {
                    ApplicationName = MvcApplication.ApplicationLicense.Info.ApplicationName,
                    ModuleName = "HR",
                    ReportName = templateName
                };

                string appPath = Server.MapPath("~/");

                return new asposeWordsTemplateReport(new HRModule.Controllers.EmployeeAttendanceController { employee = employee, serverPath = appPath }.CustomizeDoc_Aspose, 
                    (FixedSettings.DTRHeader && !DTR.IsDTRByCutOff ? new ReportHeaderParams(appPath) { ReportLogo_Width = 40, ReportLogo_Height = 40 } : null), FixedSettings.ApplicationName, FixedSettings.Owner,
                    new ReportFooterParams(appPath), CustomData)
                    .Get(CustomData.ReportName, employee.LastName, data, false);

            }
        }
    }
}