using coreApp.Controllers;
using coreApp.DAL;
using coreApp.Filters;
using coreApp.Interfaces;
using Module.Core;
using Module.DB;
using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Web.Mvc;
using System.Reflection;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace coreApp.Areas.HRModule.Controllers
{
    [EmployeeInfoFilter]
    [EmployeeAuthorize]
    public class EmployeeAccessController : Base_NoCoreHRStaffController, IEmployeeController
    {
        public tblEmployee employee { get; set; }

        public ActionResult Edit()
        {

            ViewBag.Title = "Access List";
            

            using (dalDataContext context = new dalDataContext())
            {
                tblEmployee_Access model = context.tblEmployee_Accesses.Where(x => x.EmployeeId == employee.EmployeeId).SingleOrDefault();
                if (model == null)
                {
                    model = new tblEmployee_Access
                    {
                        EmployeeId = employee.EmployeeId
                    };
                }

                return PartialView("_Access", model);
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(string[] Access, tblEmployee_Access model)
        {
            queryResult res = new queryResult { IsSuccessful = true, Data = null, Err = "", Remarks = "" };

            if (Access == null)
            {
                Access = new string[] { };
            }

            try
            {
                using (dalDataContext context = new dalDataContext())
                {
                    if (!Cache.Get().userAccess.HasAccess("hr_emp_access"))
                    {
                        AddError(ModuleConstants_Authorization.USERACCESS_UNAUTHORIZED_ACTION);
                    }
                    
                    if (ModelState.IsValid)
                    {
                        tblEmployee_Access access = context.tblEmployee_Accesses.Where(x => x.AccessId == model.AccessId).SingleOrDefault();
                        if (access == null)
                        {
                            access = new tblEmployee_Access
                            {
                                EmployeeId = employee.EmployeeId
                            };
                            context.tblEmployee_Accesses.InsertOnSubmit(access);
                        }

                        string[] orig = getAccess(access);

                        if (Cache.Get().userAccess.IsAdmin)
                        {
                            access.system_admin = Access.Contains("system_admin");
                        }

                        access.hr_config = Access.Contains("hr_config");
                        access.hr_devices = Access.Contains("hr_devices");
                        access.hr_bulletinboard = Access.Contains("hr_bulletinboard");
                        access.hr_module = Access.Contains("hr_module");
                        access.hr_emp_login_account = Access.Contains("hr_emp_login_account");
                        access.hr_emp_access = Access.Contains("hr_emp_access");
                        access.hr_emp_confidential = Access.Contains("hr_emp_confidential");
                        access.hr_emp_timelogs_modify = Access.Contains("hr_emp_timelogs_modify");
                        access.hr_permissions = Access.Contains("hr_permissions");
                        access.hr_emp_leave = Access.Contains("hr_emp_leave");
                        access.hr_emp_leave_app = Access.Contains("hr_emp_leave_app");
                        access.hr_emp_leave_app_approve = Access.Contains("hr_emp_leave_app_approve");
                        access.hr_emp_travel_app = Access.Contains("hr_emp_travel_app");
                        access.hr_emp_ot_app = Access.Contains("hr_emp_ot_app");
                        access.hr_emp_career = Access.Contains("hr_emp_career");
                        access.hr_emp_sched = Access.Contains("hr_emp_sched");
                        access.hr_emp_info = Access.Contains("hr_emp_info");
                        access.hr_holidays = Access.Contains("hr_holidays");
                        access.hr_ps = Access.Contains("hr_ps");
                        access.finance_module = Access.Contains("finance_module");
                        access.leave_module = Access.Contains("leave_module");
                        access.leave_module_settings = Access.Contains("leave_module_settings");
                        access.finance_ps = Access.Contains("finance_ps");
                        access.finance_employee_loans = Access.Contains("finance_employee_loans");
                        access.finance_definitions = Access.Contains("finance_definitions");
                        
                        context.SubmitChanges();

                        Cache.Update_Tables(_employee_accesses: true);

                        res.Remarks = "Access list was successfully updated";

                        string[] current = getAccess(access);

                        var data = new
                        {
                            Added = current.Where(x => !orig.Contains(x)).ToArray(),
                            Removed = orig.Where(x => !current.Contains(x)).ToArray()
                        };

                        MvcApplication.HLLog.WriteLog("Employee Profile", "Update Access List",
                          string.Format("Updated records of [{0}] {1}", employee.EmployeeId, employee.FullName),
                           data);

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

        string[] getAccess(tblEmployee_Access access)
        {
            List<string> ret = new List<string>();

            foreach (PropertyInfo pi in typeof(tblEmployee_Access).GetProperties())
            {
                if (pi.PropertyType == typeof(Boolean))
                {
                    if ((bool)pi.GetValue(access))
                    {
                        ret.Add(pi.Name);
                    }
                }
            }

            return ret.ToArray();
        }


        public ActionResult EditDepartments()
        {
            UserAccess access = new UserAccess(employee);
            OfficeStructureParameters model = new OfficeStructureParameters
            {
                Mode = coreApp.Enums.OfficeStructureMode.ListView_Value,
                SelectedIds = access.departments.Select(x => x.DepartmentId).ToArray()
            };

            return PartialView("_OfficeStructure", model);
        }

        [HttpPost]
        public ActionResult EditDepartments(int[] DepartmentScopeSelection)
        {
            queryResult res = new queryResult { IsSuccessful = true, Data = null, Remarks = "", Err = "" };
            if (DepartmentScopeSelection == null)
            {
                DepartmentScopeSelection = new int[] { };
            }
            try
            {
                using (dalDataContext context = new dalDataContext())
                {
                    if (!Cache.Get().userAccess.HasAccess("hr_emp_access"))
                    {
                        AddError(ModuleConstants_Authorization.USERACCESS_UNAUTHORIZED_ACTION);
                    }

                    if (ModelState.IsValid)
                    {
                        tblEmployee_Access access = context.tblEmployee_Accesses.Where(x => x.EmployeeId == employee.EmployeeId).SingleOrDefault();
                        if (access == null)
                        {
                            access = new tblEmployee_Access
                            {
                                EmployeeId = employee.EmployeeId
                            };
                            context.tblEmployee_Accesses.InsertOnSubmit(access);
                        }

                        int[] origIds = access.getIds();
                        List<tblDepartment> orig = access.getDepartments();

                        if (DepartmentScopeSelection.Length == 0)
                        {
                            access.department_scope = "";
                        }
                        else
                        {
                            access.department_scope = string.Join(",", DepartmentScopeSelection);
                        }
                        context.SubmitChanges();

                        Cache.Update_Tables(_employee_accesses: true);

                        res.Remarks = "Office scope was successfully updated";

                        var data = new
                        {
                            Added = access.getDepartments().Where(x => !origIds.Contains(x.DepartmentId)).Select(x => string.Format("[{0}] {1}", x.DepartmentId, x.Department)).ToArray(),
                            Removed = orig.Where(x => !access.getIds().Contains(x.DepartmentId)).Select(x => string.Format("[{0}] {1}", x.DepartmentId, x.Department)).ToArray()
                        };

                        MvcApplication.HLLog.WriteLog("Employee Profile", "Update Office/Department Scope",
                         string.Format("Updated records of [{0}] {1}", employee.EmployeeId, employee.FullName),
                          data);

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
