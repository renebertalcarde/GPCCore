using System.Linq;
using System.Web.Mvc;
using System;
using coreApp.DAL;
using coreApp.Controllers;
using coreApp.Enums;
using Microsoft.AspNet.Identity;
using System.Collections.Generic;
using Module.DB;
using Module.Core;

namespace coreApp.Areas.HRModule.Controllers
{
    public class SetGeoAuthAreasController : Base_NoCoreHRStaffController
    {
        public ActionResult Index()
        {
            using (dalDataContext context = new dalDataContext())
            {
                ViewBag.Areas = context.tblGeoAuth_Areas.OrderBy(x => x.AreaName).ToList();

                return View();
            }
        }

        [HttpPost]
        public JsonResult SaveSelection(string employeeIds, string areas)
        {
            queryResult res = new queryResult { IsSuccessful = true, Data = null, Err = "", Remarks = "" };

            try
            {
                using (dalDataContext context = new dalDataContext())
                {
                    int[] ids = employeeIds.Split(',').Where(x => !string.IsNullOrEmpty(x)).Select(x => int.Parse(x)).ToArray();
                    
                    List<tblEmployee> emp = context.tblEmployees.Where(x => ids.Contains(x.EmployeeId)).ToList();

                    emp.ForEach(x =>
                    {
                        if (string.IsNullOrEmpty(areas))
                        {
                            x.GeoAuth_Areas = null;
                        }
                        else
                        {
                            x.GeoAuth_Areas = areas;
                        }
                    });

                    context.SubmitChanges();

                    res.Remarks = "Geo-Auth Areas were successfully set";

                    if (emp.Any())
                    {
                        string[] _employees = context.tblEmployees.Where(x => ids.Contains(x.EmployeeId)).Select(x => string.Format("[{0}] {1}", x.EmployeeId, x.FullName)).ToArray();
                        if (string.IsNullOrEmpty(areas))
                        {
                            var data = new
                            {
                                Areas = new string[] { },
                                Employees = _employees
                            };

                            MvcApplication.HLLog.WriteLog("Batch Processes", "Removed Employee Geo-Location Areas",
                                  string.Format("Removed records from {0} employees", _employees.Length),
                                  data);
                        }
                        else
                        {
                            var data = new
                            {
                                Areas = context.tblGeoAuth_Areas.Where(x => areas.Split(',').Contains(x.Id.ToString()))
                                        .Select(x => string.Format("[{0}] {1}", x.Id, x.AreaName))
                                        .ToArray(),
                                Employees = _employees
                            };
                            
                            MvcApplication.HLLog.WriteLog("Batch Processes", "Set Employee Geo-Location Areas",
                                   string.Format("Set records for {0} employees", _employees.Length),
                                   data);
                        }
                    }

                    Cache.Update();
                }
            }
            catch (Exception ex)
            {
                res.IsSuccessful = false;
                res.Err = coreProcs.ShowErrors(ex);
            }

            return Json(res);
        }
    }
}