﻿using System;
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
    public class MyTravelsController : Base_NoCoreEmployeeController, IMyController
    {
        public tblEmployee employee { get; set; }
        
        public ActionResult Index()
        {
            using (dalDataContext context = new dalDataContext())
            {

                var model = context.tblEmployee_Travels.Where(x => x.EmployeeId == employee.EmployeeId).OrderByDescending(x => x.StartDate).ToList();
                return View(model);
            }
        }
        
    }
}
