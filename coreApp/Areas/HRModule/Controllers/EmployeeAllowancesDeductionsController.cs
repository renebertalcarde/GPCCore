using System;
using System.Linq;
using System.Web.Mvc;
using coreApp.Filters;
using coreApp.DAL;
using coreApp.Controllers;
using coreApp.Interfaces;
using System.Collections.Generic;
using coreApp.Models;
using Module.DB;

namespace coreApp.Areas.HRModule.Controllers
{
    [EmployeeInfoFilter]
    [EmployeeAuthorize]
    public class EmployeeAllowancesDeductionsController : HLBase_NoCoreAuthorizedController, IEmployeeController
    {
        public tblEmployee employee { get; set; }
        
        public ActionResult Index()
        {
            using (hr2017DataContext context = new hr2017DataContext())
            {
                var model = new EmployeeAllowancesDeductionsModel
                {
                    Allowances = context.tblEmployee_Allowances.Where(x => x.EmployeeId == employee.EmployeeId).ToList().Where(x => x.Allowance != null).OrderBy(x => x.Allowance.Description).ToList(),
                    Deductions = context.tblEmployee_Deductions.Where(x => x.EmployeeId == employee.EmployeeId).ToList().Where(x => x.Deduction != null).OrderBy(x => x.Deduction.Description).ToList(),
                };

                return View(model);
            }
        }

        public ActionResult EditAllowances()
        {
            ViewBag.Title = "Allowances";
            

            ViewBag.uiIsReadOnly = false;
            ViewBag.uiIncludeId = true;

            using (hr2017DataContext context = new hr2017DataContext())
            {
                ViewBag.Allowances = context.tblFin_AllowanceDefs.OrderBy(x => x.Description).ToList();

                var model = context.tblEmployee_Allowances.ToList().Where(x => x.EmployeeId == employee.EmployeeId && x.Allowance != null).OrderBy(x => x.Allowance.Description).ToList();
                return PartialView(model);
            }
        }

        public ActionResult EditDeductions()
        {
            ViewBag.Title = "Deductions";
            

            ViewBag.uiIsReadOnly = false;
            ViewBag.uiIncludeId = true;

            using (hr2017DataContext context = new hr2017DataContext())
            {
                ViewBag.Deductions = context.tblFin_DeductionDefs.OrderBy(x => x.Description).ToList();

                var model = context.tblEmployee_Deductions.ToList().Where(x => x.EmployeeId == employee.EmployeeId && x.Deduction != null).OrderBy(x => x.Deduction.Description).ToList();
                return PartialView(model);
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditAllowances(string[] Allowances)
        {
            queryResult res = new queryResult { IsSuccessful = true, Data = null, Err = "", Remarks = "" };

            try
            {
                using (hr2017DataContext context = new hr2017DataContext())
                {
                    var existing_allowances = context.tblEmployee_Allowances.Where(x => x.EmployeeId == employee.EmployeeId);
                    context.tblEmployee_Allowances.DeleteAllOnSubmit(existing_allowances);

                    context.SubmitChanges();

                    if (Allowances != null)
                    {
                        List<tblEmployee_Allowance> allowances = new List<tblEmployee_Allowance>();

                        foreach (string allowanceId in Allowances)
                        {
                            allowances.Add(new tblEmployee_Allowance
                            {
                                EmployeeId = employee.EmployeeId,
                                AllowanceId = int.Parse(allowanceId)
                            });
                        }

                        if (allowances.Any())
                        {
                            context.tblEmployee_Allowances.InsertAllOnSubmit(allowances);
                            context.SubmitChanges();
                        }

                    }

                    res.Remarks = "Allowances were successfully updated";
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
        [ValidateAntiForgeryToken]
        public ActionResult EditDeductions(string[] Deductions)
        {
            queryResult res = new queryResult { IsSuccessful = true, Data = null, Err = "", Remarks = "" };

            try
            {
                using (hr2017DataContext context = new hr2017DataContext())
                {
                    var existing_deductions = context.tblEmployee_Deductions.Where(x => x.EmployeeId == employee.EmployeeId);
                    context.tblEmployee_Deductions.DeleteAllOnSubmit(existing_deductions);

                    context.SubmitChanges();

                    if (Deductions != null)
                    {
                        List<tblEmployee_Deduction> deductions = new List<tblEmployee_Deduction>();

                        foreach (string deductionId in Deductions)
                        {
                            deductions.Add(new tblEmployee_Deduction
                            {
                                EmployeeId = employee.EmployeeId,
                                DeductionId = int.Parse(deductionId)
                            });
                        }


                        if (deductions.Any())
                        {
                            context.tblEmployee_Deductions.InsertAllOnSubmit(deductions);
                            context.SubmitChanges();
                        }
                    }
                    
                    res.Remarks = "Deductions were successfully updated";
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
