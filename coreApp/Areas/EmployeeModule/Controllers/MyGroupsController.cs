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
    public class MyGroupsController : Base_NoCoreEmployeeController, IMyController
    {
        public tblEmployee employee { get; set; }

        public ActionResult Index()
        {
            using (dalDataContext context = new dalDataContext())
            {

                var model = context.tblEmployee_Groups.Where(x => x.EmployeeId == employee.EmployeeId)
                    .Join(context.tblGroups, a => a.GroupId, b => b.Id, (a, b) => b)
                    .OrderBy(x => x.GroupName)
                    .ToList();

                return View(model);
            }
        }
    }
}