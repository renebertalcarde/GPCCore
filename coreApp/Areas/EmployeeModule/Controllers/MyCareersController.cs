using System.Linq;
using System.Web.Mvc;
using System;
using coreApp.DAL;
using coreApp.Filters;
using coreApp.Controllers;
using coreApp.Interfaces;
using reports;
using coreApp.Areas.HRModule.Controllers;
using reports.AsposeLib;
using Aspose.Words;
using Module.DB;

namespace coreApp.Areas.EmployeeModule.Controllers
{
    [MyFilter]
    public class MyCareersController : Base_NoCoreEmployeeController, IMyController
    {
        public tblEmployee employee { get; set; }

        public ActionResult Index()
        {
            using (dalDataContext context = new dalDataContext())
            {
                var model = context.tblEmployee_Careers.Where(x => x.EmployeeId == employee.EmployeeId).OrderByDescending(x => x.DateEffective).ToList();
                return View(model);
            }
        }

        public ActionResult Print(bool dlWord = false)
        {
            EmployeeCareersController hrCareer = new EmployeeCareersController();

            using (hr2017DataContext context = new hr2017DataContext())
            {
                string fn = string.Format("service-record-{0}", employee.LastName.ToLower());

                return new asposeWordsTemplateReport(hrCareer.CustomizeDoc_Aspose, new reports.AsposeLib.ReportHeaderParams(Server.MapPath("~/")) { ReportLogo_Width = 40, ReportLogo_Height = 40 }, FixedSettings.ApplicationName, FixedSettings.Owner).Get("servicerecord", fn, employee, dlWord);
            }
        }
    }
}