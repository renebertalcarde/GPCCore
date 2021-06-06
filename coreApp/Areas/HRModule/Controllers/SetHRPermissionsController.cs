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
    [UserAccessAuthorize(allowedAccess: "hr_permissions")]
    public class SetHRPermissionsController : Base_NoCoreHRStaffController
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult SelectedEmployees(string selection)
        {
            List<tblEmployee> model = new List<tblEmployee>();

            if (!string.IsNullOrEmpty(selection))
            {
                using (dalDataContext context = new dalDataContext())
                {
                    model = context.tblEmployees.Where(x => selection.Split(',').Contains(x.EmployeeId.ToString())).ToList();
                    
                }
            }

            return PartialView(model);
        }

        public ActionResult SelectedPermissions(string selection)
        {
            List<string> model = new List<string>();

            if (!string.IsNullOrEmpty(selection))
            {
                foreach (string s in selection.Split(','))
                {
                    model.Add(ModuleConstants.HRPermissionModules[s].ToString());
                }
            }

            return PartialView(model);
        }

        [HttpPost]
        public JsonResult SaveSelection(string employeeIds, string permissions)
        {
            queryResult res = new queryResult { IsSuccessful = true, Data = null, Err = "", Remarks = "" };
            permissions = permissions ?? "";

            try
            {
                using (dalDataContext context = new dalDataContext())
                {
                    int[] ids = employeeIds.Split(',').Where(x => !string.IsNullOrEmpty(x)).Select(x => int.Parse(x)).ToArray();
                    
                    foreach (int id in ids)
                    {
                        _HRPermission p = context._HRPermissions.Where(x => x.EmployeeId == id).SingleOrDefault();
                        if (p == null)
                        {
                            p = new _HRPermission { EmployeeId = id };
                            context._HRPermissions.InsertOnSubmit(p);
                        }

                        p.Controllers = permissions;
                    }
                    
                    context.SubmitChanges();

                    res.Remarks = "Permissions were successfully set";

                    if (ids.Any())
                    {
                        string[] _employees = context.tblEmployees.Where(x => ids.Contains(x.EmployeeId)).Select(x => string.Format("[{0}] {1}", x.EmployeeId, x.FullName)).ToArray();
                        if (string.IsNullOrEmpty(permissions))
                        {
                            var data = new
                            {
                                Permissions = new string[] { },
                                Employees = _employees
                            };

                            MvcApplication.HLLog.WriteLog("Batch Processes", "Remove Employee Permissions",
                                  string.Format("Removed records from {0} employees", _employees.Length),
                                  data);
                        }
                        else
                        {
                            var data = new
                            {
                                Permissions = permissions.Split(','),
                                Employees = _employees
                            };

                            MvcApplication.HLLog.WriteLog("Batch Processes", "Set Employee Permissions",
                                  string.Format("Set records for {0} employees", _employees.Length),
                                  data);
                        }
                    }
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