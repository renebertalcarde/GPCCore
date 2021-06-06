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
using System.Collections.Generic;
using Module.Time;
using coreApp.Models;

namespace coreApp.Areas.EmployeeModule.Controllers
{
    [MyFilter]
    public class MyAllowancesDeductionsController : Base_NoCoreEmployeeController, IMyController
    {
        public tblEmployee employee { get; set; }

        public ActionResult Index()
        {
            using (hr2017DataContext context = new hr2017DataContext())
            {
                var model = new EmployeeAllowancesDeductionsModel
                {
                    Allowances = context.tblEmployee_Allowances.Where(x => x.EmployeeId == employee.EmployeeId).ToList().Where(x => x.Allowance != null).OrderBy(x => x.Allowance.Description).ToList(),
                    Deductions = context.tblEmployee_Deductions.Where(x => x.EmployeeId == employee.EmployeeId).ToList().Where(x => x.Deduction != null).OrderBy(x => x.Deduction.Description).ToList(),
                };

                return View(model);
            }
        }
    }
}