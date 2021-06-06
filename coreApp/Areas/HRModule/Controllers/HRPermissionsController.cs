using System.Linq;
using System.Web.Mvc;
using System;
using coreApp.Controllers;
using coreApp.DAL;
using coreApp.Interfaces;
using coreApp.Filters;
using Module.DB;
using Module.Core;

namespace coreApp.Areas.HRModule.Controllers
{
    [EmployeeInfoFilter]
    public class HRPermissionsController : Base_NoCoreHRStaffController, IEmployeeController
    {
        public tblEmployee employee { get; set; }

        public ActionResult Edit()
        {
            
            ViewBag.Title = "Permissions";
            

            using (dalDataContext context = new dalDataContext())
            {
                _HRPermission model = context._HRPermissions.Where(x => x.EmployeeId == employee.EmployeeId).SingleOrDefault();
                if (model == null)
                {
                    model = new _HRPermission
                    {
                        EmployeeId = employee.EmployeeId
                    };
                }

                return PartialView("_Permission", model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(string[] Permissions, _HRPermission model)
        {
            queryResult res = new queryResult { IsSuccessful = true, Data = null, Err = "", Remarks = "" };

            try
            {
                using (dalDataContext context = new dalDataContext())
                {
                    if (!Cache.Get().userAccess.HasAccess("hr_permissions"))
                    {
                        AddError(ModuleConstants_Authorization.USERACCESS_UNAUTHORIZED_ACTION);
                    }

                    if (ModelState.IsValid)
                    {
                        _HRPermission p = context._HRPermissions.Where(x => x.Id == model.Id).SingleOrDefault();

                        string[] orig = new string[] { };

                        if (Permissions == null)
                        {
                            if (p != null)
                            {
                                context._HRPermissions.DeleteOnSubmit(p);
                            }
                        }
                        else
                        {                           
                            if (p == null)
                            {
                                p = new _HRPermission
                                {
                                    EmployeeId = employee.EmployeeId
                                };
                                context._HRPermissions.InsertOnSubmit(p);
                            }

                            if (!string.IsNullOrEmpty(p.Controllers))
                            {
                                orig = p.Controllers.Split(',');
                            }

                            p.Controllers = string.Join(",", Permissions);
                        }
                        
                        context.SubmitChanges();

                        Cache.Update_Tables(_employee_permissions: true);

                        res.Remarks = "Permissions were successfully updated";

                        if (Permissions == null)
                        {
                            MvcApplication.HLLog.WriteLog("Employee Profile", "Remove Permissions",
                             string.Format("Remove records of [{0}] {1}", employee.EmployeeId, employee.FullName));
                        }
                        else
                        {
                            string[] current = p.Controllers.Split(',');
                            var data = new
                            {
                                Added = current.Where(x => !orig.Contains(x)).ToArray(),
                                Removed = orig.Where(x => !current.Contains(x)).ToArray()
                            };

                            MvcApplication.HLLog.WriteLog("Employee Profile", "Update Permissions",
                              string.Format("Updated records of [{0}] {1}", employee.EmployeeId, employee.FullName),
                               data);
                        }

                        Cache.Update();
                    }
                    else
                    {
                        throw new Exception(coreProcs.ShowErrors(ModelState));
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