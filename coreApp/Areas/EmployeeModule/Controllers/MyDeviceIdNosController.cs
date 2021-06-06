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

namespace coreApp.Areas.EmployeeModule.Controllers
{
    [MyFilter]
    public class MyDeviceIdNosController : Base_NoCoreEmployeeController, IMyController
    {
        public tblEmployee employee { get; set; }

        public ActionResult Index()
        {
            using (dalDataContext context = new dalDataContext())
            {
                List<EmployeeDeviceIdNoModel> model = context.tblDevices.OrderBy(x => x.DeviceName).Select(x => new EmployeeDeviceIdNoModel
                {
                    employeeId = employee.EmployeeId,
                    device = x,
                    deviceIdNo = context.tblEmployee_DeviceIdNos.Where(y => y.EmployeeId == employee.EmployeeId && y.DeviceId == x.DeviceId).SingleOrDefault()
                }).ToList();

                return View(model);
            }
        }
    }
}