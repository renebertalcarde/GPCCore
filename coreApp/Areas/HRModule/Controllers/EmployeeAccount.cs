using coreApp.Controllers;
using coreApp.DAL;
using coreApp.Filters;
using coreApp.Interfaces;
using coreApp.Models;
using Microsoft.AspNet.Identity;
using Module.Core;
using Module.DB;
using System;
using System.Linq;
using System.Web.Mvc;

namespace coreApp.Areas.HRModule.Controllers
{
    [EmployeeInfoFilter]
    [EmployeeAuthorize]
    public class EmployeeAccountController : Base_NoCoreHRStaffController, IEmployeeController
    {
        public tblEmployee employee { get; set; }

        public ActionResult CreateAccount()
        {
            ViewBag.Title = "Employee Account";
            

            ViewBag.uiIsReadOnly = false;
            ViewBag.uiIncludeId = false;

            return PartialView("_Account", new EmployeeAccountModel
            {
                EmployeeId = employee.EmployeeId,
                Email = employee.Email
            });
        }

        [HttpPost]
        public ActionResult CreateAccount(EmployeeAccountModel model)
        {
            queryResult res = new queryResult { IsSuccessful = true, Data = null, Remarks = "", Err = "" };

            try
            {
                using (dalDataContext context = new dalDataContext())
                {

                    ApplicationUser u = UserManager.FindByEmail(employee.Email);
                    if (u != null)
                    {
                        AddError("Employee login account already exists");
                    }

                    if (ModelState.IsValid)
                    {
                        var result = Account.CreateUser(UserManager, new NewUserModel
                        {
                            UserName = model.Username,
                            Email = employee.Email,
                            Password = FixedSettings.DefaultPassword,
                            Roles = new string[] { "employee" }
                        });

                        if (result.Succeeded)
                        {
                            tblEmployee employee = context.tblEmployees.Where(x => x.EmployeeId == model.EmployeeId).Single();
                            employee.UserId = UserManager.FindByEmail(model.Email).Id;

                            context.SubmitChanges();

                            res.Remarks = "Employee login account was successfully created";

                            var data = new
                            {
                                Username = model.Username
                            };

                            MvcApplication.HLLog.WriteLog("Employee Profile", "Create Account",
                               string.Format("Created record for [{0}] {1}", employee.EmployeeId, employee.FullName),
                               data);
                        }
                        else
                        {
                            AddErrors(result);
                            throw new Exception(coreProcs.ShowErrors(ModelState));
                        }
                        
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
        
        [HttpPost]
        public ActionResult EnableAccount(string userId)
        {
            queryResult res = new queryResult { IsSuccessful = true, Data = null, Remarks = "", Err = "" };

            try
            {
                if (!Cache.Get().userAccess.HasAccess("hr_emp_login_account"))
                {
                    AddError(ModuleConstants_Authorization.USERACCESS_UNAUTHORIZED_ACTION);
                }

                ApplicationUser u = UserManager.FindById(userId);
                if (u == null)
                {
                    AddError("Employee login account does not exists");
                }
                else if (!Module.DB.Procs.Account.IsAccountDisabled(u.Id))
                {
                    AddError("Employee login account is already enabled");
                }

                if (ModelState.IsValid)
                {

                    Module.DB.Procs.Account.DisableAccount(userId, false);

                    res.Remarks = "Employee login account was successfully enabled";

                    MvcApplication.HLLog.WriteLog("Employee Profile", "Enable Account",
                               string.Format("Enabled account of [{0}] {1}", employee.EmployeeId, employee.FullName));
                }
                else
                {
                    throw new Exception(coreProcs.ShowErrors(ModelState));
                }

            }
            catch (Exception e)
            {
                res.IsSuccessful = false;
                res.Err = coreProcs.ShowErrors(e);
            }

            return Json(res);
            
        }

        [HttpPost]
        public ActionResult DisableAccount(string userId)
        {
            queryResult res = new queryResult { IsSuccessful = true, Data = null, Remarks = "", Err = "" };

            try
            {
                if (!Cache.Get().userAccess.HasAccess("hr_emp_login_account"))
                {
                    AddError(ModuleConstants_Authorization.USERACCESS_UNAUTHORIZED_ACTION);
                }

                ApplicationUser u = UserManager.FindById(userId);
                if (u == null)
                {
                    AddError("Employee login account does not exists");
                }
                else if (Module.DB.Procs.Account.IsAccountDisabled(u.Id))
                {
                    AddError("Employee login account is already disabled");
                }

                if (ModelState.IsValid)
                {

                    Module.DB.Procs.Account.DisableAccount(userId, true);

                    res.Remarks = "Employee login account was successfully disabled";

                    MvcApplication.HLLog.WriteLog("Employee Profile", "Disable Account",
                           string.Format("Disabled account of [{0}] {1}", employee.EmployeeId, employee.FullName));
                }
                else
                {
                    throw new Exception(coreProcs.ShowErrors(ModelState));
                }

            }
            catch (Exception e)
            {
                res.IsSuccessful = false;
                res.Err = coreProcs.ShowErrors(e);
            }

            return Json(res);

        }

        [HttpPost]
        public ActionResult ResetPassword(string userId)
        {
            queryResult res = new queryResult { IsSuccessful = true, Data = null, Remarks = "", Err = "" };

            try
            {
                UserAccess access = Cache.Get().userAccess;

                if (!access.HasAccess("hr_emp_login_account"))
                {
                    AddError(ModuleConstants_Authorization.USERACCESS_UNAUTHORIZED_ACTION);
                }

                ApplicationUser u = UserManager.FindById(userId);
                if (u == null)
                {
                    AddError("Employee login account does not exists");
                }
                else if (Module.DB.Procs.Account.IsAccountDisabled(u.Id))
                {
                    AddError("Employee login account is disabled");
                }

                if (ModelState.IsValid)
                {
                    var code = UserManager.GeneratePasswordResetToken(userId);
                    var result = UserManager.ResetPassword(userId, code, FixedSettings.DefaultPassword);

                    if (result.Succeeded)
                    {
                        using (dalDataContext context = new dalDataContext())
                        {
                            DateTime dt = DateTime.Now;
                            tblEmployee employee = context.tblEmployees.Where(x => x.UserId == userId).SingleOrDefault();
                            if (employee != null)
                            {
                                employee.LastPasswordChange = null;
                                employee.LastPasswordReset = dt;
                                employee.LastPasswordResetBy_UserId = access.user.Id;
                                context.SubmitChanges();
                            }
                        }

                        res.Remarks = "Employee login password was successfully reset";

                        MvcApplication.HLLog.WriteLog("Employee Profile", "Reset Password",
                            string.Format("Reset password of [{0}] {1}", employee.EmployeeId, employee.FullName));
                    }
                    else
                    {
                        AddErrors(result);
                        throw new Exception(coreProcs.ShowErrors(ModelState));
                    }                    
                }
                else
                {
                    throw new Exception(coreProcs.ShowErrors(ModelState));
                }

            }
            catch (Exception e)
            {
                res.IsSuccessful = false;
                res.Err = coreProcs.ShowErrors(e);
            }

            return Json(res);

        }

        [HttpPost]
        public ActionResult DeleteAccount(string userId)
        {
            queryResult res = new queryResult { IsSuccessful = true, Data = null, Remarks = "", Err = "" };

            try
            {
                if (!Cache.Get().userAccess.HasAccess("hr_emp_login_account"))
                {
                    AddError(ModuleConstants_Authorization.USERACCESS_UNAUTHORIZED_ACTION);
                }

                ApplicationUser u = UserManager.FindById(userId);
                if (u == null)
                {
                    AddError("Employee login account does not exists");
                }

                if (ModelState.IsValid)
                {
                    var result = UserManager.Delete(u);

                    if (result.Succeeded)
                    {
                        res.Remarks = "Employee login account was successfully deleted";

                        MvcApplication.HLLog.WriteLog("Employee Profile", "Delete Account",
                            string.Format("Deleted record of [{0}] {1}", employee.EmployeeId, employee.FullName));
                    }
                    else
                    {
                        AddErrors(result);
                        throw new Exception(coreProcs.ShowErrors(ModelState));
                    }
                }
                else
                {
                    throw new Exception(coreProcs.ShowErrors(ModelState));
                }

            }
            catch (Exception e)
            {
                res.IsSuccessful = false;
                res.Err = coreProcs.ShowErrors(e);
            }

            return Json(res);

        }
    }
}
