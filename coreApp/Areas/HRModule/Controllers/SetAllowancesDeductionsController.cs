using System.Linq;
using System.Web.Mvc;
using System;
using coreApp.DAL;
using coreApp.Controllers;
using coreApp.Enums;
using Microsoft.AspNet.Identity;
using System.Collections.Generic;
using Module.DB;

namespace coreApp.Areas.HRModule.Controllers
{
    public class SetAllowancesDeductionsController : Base_NoCoreHRStaffController
    {
        public ActionResult Index()
        {
            using (hr2017DataContext context = new hr2017DataContext())
            {
                ViewBag.Allowances = context.tblFin_AllowanceDefs.OrderBy(x => x.Description).ToList();
                ViewBag.Deductions = context.tblFin_DeductionDefs.OrderBy(x => x.Description).ToList();

                return View();
            }
        }
        
        [HttpPost]
        public JsonResult SaveSelection(string employeeIds, string allowances, string deductions)
        {
            queryResult res = new queryResult { IsSuccessful = true, Data = null, Err = "", Remarks = "" };

            try
            {
                using (hr2017DataContext context = new hr2017DataContext())
                {
                    int[] ids = employeeIds.Split(',').Where(x => !string.IsNullOrEmpty(x)).Select(x => int.Parse(x)).ToArray();

                    List<tblEmployee_Allowance> existingAllowances = context.tblEmployee_Allowances.Where(x => ids.Contains(x.EmployeeId)).ToList();
                    context.tblEmployee_Allowances.DeleteAllOnSubmit(existingAllowances);

                    List<tblEmployee_Deduction> existingDeductions = context.tblEmployee_Deductions.Where(x => ids.Contains(x.EmployeeId)).ToList();
                    context.tblEmployee_Deductions.DeleteAllOnSubmit(existingDeductions);
                    context.SubmitChanges();

                    List<tblEmployee_Allowance> newAllowances = new List<tblEmployee_Allowance>();
                    List<tblEmployee_Deduction> newDeductions = new List<tblEmployee_Deduction>();

                    foreach (int id in ids)
                    {
                        if (!string.IsNullOrEmpty(allowances))
                        {
                            foreach (string t in allowances.Split(','))
                            {
                                int allowanceId = int.Parse(t);

                                newAllowances.Add(new tblEmployee_Allowance
                                {
                                    EmployeeId = id,
                                    AllowanceId = allowanceId
                                });
                            }
                        }

                        if (!string.IsNullOrEmpty(deductions))
                        {
                            foreach (string t in deductions.Split(','))
                            {
                                int deductionId = int.Parse(t);

                                newDeductions.Add(new tblEmployee_Deduction
                                {
                                    EmployeeId = id,
                                    DeductionId = deductionId
                                });
                            }
                        }
                    }

                    context.tblEmployee_Allowances.InsertAllOnSubmit(newAllowances);
                    context.tblEmployee_Deductions.InsertAllOnSubmit(newDeductions);
                    context.SubmitChanges();

                    res.Remarks = "Allowances/Deductions were successfully set";

                    if (ids.Any())
                    {
                        using (dalDataContext _context = new dalDataContext())
                        {
                            string[] _employees = _context.tblEmployees.Where(x => ids.Contains(x.EmployeeId)).Select(x => string.Format("[{0}] {1}", x.EmployeeId, x.FullName)).ToArray();
                            
                            if (newAllowances.Any() || newDeductions.Any())
                            {
                                var data = new
                                {
                                    Allowances = newAllowances.Select(x => string.Format("[{0}] {1}", x.AllowanceId, x.Allowance.Description)).Distinct().ToArray(),
                                    Deductions = newDeductions.Select(x => string.Format("[{0}] {1}", x.DeductionId, x.Deduction.Description)).Distinct().ToArray(),
                                    Employees = _employees
                                };

                                MvcApplication.HLLog.WriteLog("Batch Processes", "Set Employee Allowances & Deductions",
                                         string.Format("Set records for {0} employees", _employees.Length),
                                         data);
                            }
                            else
                            {
                                var data = new
                                {
                                    Allowances = new string[] { },
                                    Deductions = new string[] { },
                                    Employees = _employees
                                };

                                MvcApplication.HLLog.WriteLog("Batch Processes", "Removed Employee Allowances & Deductions",
                                         string.Format("Removed records from {0} employees", _employees.Length),
                                         data);
                            }
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