using System;
using System.Linq;
using System.Web.Mvc;
using coreApp.Filters;
using coreApp.DAL;
using coreApp.Controllers;
using coreApp.Interfaces;
using Module.DB;
using coreLib.Objects;
using Module.Core;

namespace coreApp.Areas.EmployeeModule.Controllers
{
    [MyFilter]
    public class MySkillsController : Base_NoCoreEmployeeController, IMyController
    {
        public tblEmployee employee { get; set; }

        public ActionResult Index()
        {
            using (hr2017DataContext context = new hr2017DataContext())
            {

                var model = context.tblEmployee_OtherInfo_Skills.Where(x => x.EmployeeId == employee.EmployeeId).OrderBy(x => x.Description).ToList();
                return View(model);
            }
        }
    }
}
