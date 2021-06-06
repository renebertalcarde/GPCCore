using coreApp.Areas.LeaveModule;
using coreApp.Areas.LeaveModule.Controllers;
using coreApp.Areas.LeaveModule.Filters;
using coreApp.Areas.LeaveModule.Interfaces;
using coreApp.Areas.ReportsModule.Models;
using coreApp.Areas.TimeModule.Time;
using coreApp.Controllers;
using coreApp.DAL;
using coreApp.Enums;
using coreApp.Filters;
using coreApp.Interfaces;
using coreLib.Enums;
using coreLib.Objects;
using Module.DB;
using Module.Leave;
using reports;
using reports.AsposeLib;
using System;
using System.Web.Mvc;

namespace coreApp.Areas.EmployeeModule.Controllers
{
    [MyFilter]
    [LeaveTypeInfoFilter(true)]
    public class MyLeaveCreditsController : Base_NoCoreEmployeeController, IMyController, ILeaveTypeController
    {
        public tblEmployee employee { get; set; }
        public tblLeaveType leaveType { get; set; }

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

            using (hr2017DataContext context = new hr2017DataContext())
            {
                ViewBag.LeaveType = leaveType;

                employeeLeave employeeLeave = new employeeLeave(employee.EmployeeId, leaveType.Id);
                LeaveCreditsModel model = new LeaveCreditsModel(employeeLeave, pm);

                return View(model);
            }
        }

        public ActionResult PrintLeaveCard(bool dlWord = false, string startDate = "", string endDate = "")
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

            EmployeeLeaveCreditsController hrLC = new EmployeeLeaveCreditsController { employee = employee, leaveType = leaveType };

            using (hr2017DataContext context = new hr2017DataContext())
            {
                employeeLeave employeeLeave = new employeeLeave(employee.EmployeeId, leaveType.Id);
                LeaveCreditsModel data = new LeaveCreditsModel(employeeLeave, pm);

                string fn = string.Format("leavecard-{0}-{1}", employee.LastName.ToLower(), leaveType.Description.ToLower());

                objData CustomData = new objData
                {
                    ApplicationName = MvcApplication.ApplicationLicense.Info.ApplicationName,
                    ModuleName = "HR",
                    ReportName = "leavecard"
                };

                string appPath = Server.MapPath("~/");

                return new asposeWordsTemplateReport(hrLC.CustomizeDoc_Aspose, new reports.AsposeLib.ReportHeaderParams(Server.MapPath("~/")) { ReportLogo_Width = 40, ReportLogo_Height = 40 }, FixedSettings.ApplicationName, FixedSettings.Owner,
                    new ReportFooterParams(appPath), CustomData)
                    .Get(CustomData.ReportName, fn, data, dlWord);
            }
        }
    }
}
