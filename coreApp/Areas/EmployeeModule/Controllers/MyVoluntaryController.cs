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
    public class MyVoluntaryController : Base_NoCoreEmployeeController, IMyController
    {
        public tblEmployee employee { get; set; }
        
        public ActionResult Index()
        {
            using (hr2017DataContext context = new hr2017DataContext())
            {
                var model = context.tblEmployee_Voluntaries.Where(x => x.EmployeeId == employee.EmployeeId).OrderByDescending(x => x.DateFrom).ToList();
                return View(model);
            }
        }
        
    }
}
