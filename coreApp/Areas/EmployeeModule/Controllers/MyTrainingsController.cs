using System;
using System.Linq;
using System.Web.Mvc;
using coreApp.Filters;
using coreApp.DAL;
using coreApp.Controllers;
using coreApp.Interfaces;
using Module.DB;
using System.Collections.Generic;

namespace coreApp.Areas.EmployeeModule.Controllers
{
    [MyFilter]
    public class MyTrainingsController : Base_NoCoreEmployeeController, IMyController
    {
        public tblEmployee employee { get; set; }
        
        public ActionResult Index()
        {
            using (hr2017DataContext context = new hr2017DataContext())
            {

                List<tblEmployee_Training> model = Procs.GetEmployeeTrainings(employee.EmployeeId);
                return View(model);
            }
        }
        
    }
}
