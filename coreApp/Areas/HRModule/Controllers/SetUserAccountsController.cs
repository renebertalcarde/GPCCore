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
using coreApp.Models;

namespace coreApp.Areas.HRModule.Controllers
{
    public class SetUserAccountsController : Base_NoCoreHRStaffController
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

        public ActionResult UsernameFormat(string format, int field)
        {
            string model = format.Replace("{0}", "<" + System.Enum.GetName(typeof(AutoAccountFld), field) + ">");

            return PartialView("UsernameFormat", model);
        }

        [HttpPost]
        public JsonResult SaveSelection(string employees, string format, int field, bool deleteExisting = false)
        {
            queryResult res = new queryResult { IsSuccessful = true, Data = null, Err = "", Remarks = "" };

            try
            {
                if (string.IsNullOrEmpty(employees))
                {
                    throw new Exception("No employee selected");
                }

                using (dalDataContext context = new dalDataContext())
                {
                    List<string> errors = new List<string>();
                    int n = 0;
                    List<string> _employees = new List<string>();

                    foreach (string s in employees.Split(','))
                    {
                        int id = int.Parse(s);
                        tblEmployee employee = context.tblEmployees.Where(x => x.EmployeeId == id).SingleOrDefault();
                        if (employee == null) continue;
                        if (string.IsNullOrEmpty(employee.Email)) continue;
                        if (field == (int)AutoAccountFld.EmployeeNo && string.IsNullOrEmpty(employee.IdNo)) continue;

                        ApplicationUser u = UserManager.FindByEmail(employee.Email);
                        if (u != null)
                        {
                            if (deleteExisting)
                            {
                                var del_result = UserManager.Delete(u);

                                if (!del_result.Succeeded)
                                {
                                    errors.AddRange(del_result.Errors.Select(x => "Error in deleting existing account of " + employee.FullName + ": " + x));
                                }
                            }
                            else
                            {
                                continue;
                            }
                        }

                        string userName = "";
                        if (field == (int)AutoAccountFld.EmployeeId) userName = format.Replace("{0}", employee.EmployeeId.ToString());
                        else if (field == (int)AutoAccountFld.EmployeeNo) userName = format.Replace("{0}", employee.IdNo);
                        else if (field == (int)AutoAccountFld.Email) userName = format.Replace("{0}", employee.Email);

                        var result = Account.CreateUser(UserManager, new NewUserModel
                        {
                            UserName = userName,
                            Email = employee.Email,
                            Password = FixedSettings.DefaultPassword,
                            Roles = new string[] { "employee" }
                        });

                        if (result.Succeeded)
                        {
                            employee.UserId = UserManager.FindByEmail(employee.Email).Id;
                            context.SubmitChanges();

                            n++;
                            _employees.Add(string.Format("[{0}] {1} - {2}", employee.EmployeeId, employee.FullName, userName));
                        }
                        else
                        {
                            if (!result.Succeeded)
                            {
                                errors.AddRange(result.Errors.Select(x => "Error in creating account for " + employee.FullName + ": " + x));
                            }
                        }

                        res.Remarks = n + " accounts were successfully created";

                        if (errors.Any())
                        {
                            res.Remarks += "<br />- " + string.Join("<br />- ", errors);
                        }
                    }

                    if (n > 0)
                    {
                        var data = new
                        {
                            Format = format,
                            Employees = _employees
                        };

                        MvcApplication.HLLog.WriteLog("Batch Processes", "Create Employee Accounts",
                              string.Format("Created records for {0} employees", n),
                              data);
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