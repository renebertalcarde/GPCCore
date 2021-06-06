
using coreApp.Areas.FinanceModule.Controllers;
using coreApp.Controllers;
using coreApp.DAL;
using coreApp.Enums;
using coreApp.Filters;
using coreApp.Interfaces;
using coreLib.Objects;
using Module.Core;
using Module.DB;
using reports;
using reports.AsposeLib;
using System;
using System.Linq;
using System.Web.Mvc;

namespace coreApp.Areas.EmployeeModule.Controllers
{
    [MyFilter]
    public class MyPayslipsController : Base_NoCoreEmployeeController, IMyController
    {
        public tblEmployee employee { get; set; }

        public ActionResult Index()
        {
            using (hr2017DataContext context = new hr2017DataContext())
            {
                ViewBag.Employee = employee;

                var model = context.tblPayrollSummary_Employees.ToList()
                    .Where(x => x.PS.IsPosted && x.EmployeeId == employee.EmployeeId).ToList();

                return View(model);
            }
        }

        public ActionResult Details(int id)
        {
            FinancePayrollController fin = new FinancePayrollController();

            using (hr2017DataContext context = new hr2017DataContext())
            {
                tblPayrollSummary_Employee psEmployee = context.tblPayrollSummary_Employees.Where(x => x.PSEmployeeId == id).SingleOrDefault();
                if (psEmployee == null)
                {
                    throw new Exception(ModuleConstants.ID_NOT_FOUND);
                }

                if (!Cache.Get().userAccess.IsMe(psEmployee.EmployeeId))
                {

                    throw new Exception(ModuleConstants_Authorization.EMPLOYEEACCESS_UNAUTHORIZED_ACCESS_TO_RESOURCE);
                }

                tblPayrollSummary ps = context.tblPayrollSummaries.Where(x => x.PSId == psEmployee.PSId).Single();

                objData CustomData = new objData
                {
                    ApplicationName = MvcApplication.ApplicationLicense.Info.ApplicationName,
                    ModuleName = "HR"
                };

                string appPath = Server.MapPath("~/");
                ReportHeaderParams hdr = null;

                if (FixedSettings.PayslipType == "payslip")
                {
                    hdr = new ReportHeaderParams(appPath) { ReportLogo_Width = 40, ReportLogo_Height = 40 };
                }

                bool IsPayslip4 = FixedSettings.PayslipType == "payslip_4";

                if (ps.PayrollType == (int)PayrollType.ACA_PERA)
                {
                    string fn = string.Format("payslip-acapera-{0}-{1}-{2}", psEmployee.FullName.ToLower(), ps.DateFrom.ToString("MMddyyyy"), ps.DateTo.ToString("MMddyyyy"));
                    CustomData.ReportName = "payslip-acapera" + (IsPayslip4 ? "_4" : "");

                    return new asposeWordsTemplateReport(fin.CustomizeDoc_Payslip_Aspose_ACAPERA, hdr, "", "",
                        new ReportFooterParams(appPath), CustomData)
                        .Get(CustomData.ReportName, fn, new tblPayrollSummary_Employee[] { psEmployee }, false, psEmployee.OfficeId);

                }
                else if (ps.PayrollType == (int)PayrollType.Overtime)
                {
                    string fn = string.Format("payslip-ot-{0}-{1}-{2}", psEmployee.FullName.ToLower(), ps.DateFrom.ToString("MMddyyyy"), ps.DateTo.ToString("MMddyyyy"));
                    CustomData.ReportName = "payslip-ot" + (IsPayslip4 ? "_4" : "");

                    return new asposeWordsTemplateReport(fin.CustomizeDoc_Payslip_Aspose_OT, hdr, "", "",
                        new ReportFooterParams(appPath), CustomData)
                        .Get(CustomData.ReportName, fn, new tblPayrollSummary_Employee[] { psEmployee }, false, psEmployee.OfficeId);

                }
                else if (ps.PayrollType == (int)PayrollType.MidYear || ps.PayrollType == (int)PayrollType.YearEnd)
                {
                    string fn = string.Format("payslip-midyear-yearend-{0}-{1}-{2}", psEmployee.FullName.ToLower(), ps.DateFrom.ToString("MMddyyyy"), ps.DateTo.ToString("MMddyyyy"));
                    CustomData.ReportName = "payslip-midyear-yearend" + (IsPayslip4 ? "_4" : "");

                    return new asposeWordsTemplateReport(fin.CustomizeDoc_Payslip_Aspose_MidYearYearEnd, hdr, "", "",
                        new ReportFooterParams(appPath), CustomData)
                        .Get(CustomData.ReportName, fn, new tblPayrollSummary_Employee[] { psEmployee }, false, psEmployee.OfficeId);

                }
                else if (ps.PayrollType == (int)PayrollType._13thMP)
                {
                    string fn = string.Format("payslip-13thmp-{0}-{1}-{2}", psEmployee.FullName.ToLower(), ps.DateFrom.ToString("MMddyyyy"), ps.DateTo.ToString("MMddyyyy"));
                    CustomData.ReportName = "payslip-13thmp" + (IsPayslip4 ? "_4" : "");

                    return new asposeWordsTemplateReport(fin.CustomizeDoc_Payslip_Aspose_13thMP, hdr, "", "",
                        new ReportFooterParams(appPath), CustomData)
                        .Get(CustomData.ReportName, fn, new tblPayrollSummary_Employee[] { psEmployee }, false, psEmployee.OfficeId);
                }
                else
                {
                    string fn = string.Format("payslip-{0}-{1}-{2}", psEmployee.FullName.ToLower(), ps.DateFrom.ToString("MMddyyyy"), ps.DateTo.ToString("MMddyyyy"));
                    CustomData.ReportName = "payslip" + (IsPayslip4 ? "_4" : "");

                    return new asposeWordsTemplateReport(fin.CustomizeDoc_Payslip_Aspose, hdr, "", "",
                        new ReportFooterParams(appPath), CustomData)
                        .Get(CustomData.ReportName, fn, new tblPayrollSummary_Employee[] { psEmployee }, false, psEmployee.OfficeId);
                }

            }
        }
    }
}
