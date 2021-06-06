using System;
using System.Linq;
using System.Web.Mvc;
using coreApp.Filters;
using coreApp.Controllers;
using coreApp.DAL;
using coreApp.Interfaces;
using System.Web;
using System.Collections.Generic;
using Module.DB;

namespace coreApp.Areas.EmployeeModule.Controllers
{
    [MyFilter]
    public class MyPhotosController : Base_NoCoreEmployeeController, IMyController
    {
        public tblEmployee employee { get; set; }
        

        public ActionResult Details()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Upload(HttpPostedFileBase ProfilePhoto, string ProfilePhoto_Remove, HttpPostedFileBase PDSPhoto, string PDSPhoto_Remove)
        {
            List<string> msg = new List<string>();
            List<string> err = new List<string>();

            DBProcs.upload_EmployeePhotos(employee, ProfilePhoto, ProfilePhoto_Remove, PDSPhoto, PDSPhoto_Remove, ref msg, ref err);

            if (msg.Count == 1)
            {
                TempData["GlobalMessage"] = msg[0];
            }
            else if (msg.Count > 1)
            {
                string a = "<ul>";
                foreach (string b in msg)
                {
                    a += string.Format("<li>{0}</li>", b);
                }
                a += "</ul>";

                TempData["GlobalMessage"] = a;
            }

            if (err.Count == 1)
            {
                TempData["GlobalError"] = err[0];
            }
            else if (err.Count > 1)
            {
                string a = "<ul>";
                foreach (string b in err)
                {
                    a += string.Format("<li>{0}</li>", b);
                }
                a += "</ul>";

                TempData["GlobalError"] = a;
            }

            return RedirectToAction("Details", new { employeeId = employee.EmployeeId });
        }
    }
}
