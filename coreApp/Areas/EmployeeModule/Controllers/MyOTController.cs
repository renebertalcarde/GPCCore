using System;
using System.Linq;
using System.Web.Mvc;
using coreApp.Filters;
using coreApp.DAL;
using coreApp.Controllers;
using coreApp.Interfaces;
using Module.DB;

namespace coreApp.Areas.EmployeeModule.Controllers
{
    [MyFilter]
    public class MyOTController : Base_NoCoreEmployeeController, IMyController
    {
        public tblEmployee employee { get; set; }
        
        public ActionResult Index()
        {
            using (dalDataContext context = new dalDataContext())
            {

                var model = context.tblEmployee_OTs.Where(x => x.EmployeeId == employee.EmployeeId).ToList().OrderByDescending(x => x.DateFrom).ToList();
                return View(model);
            }
        }
        
    }
}
