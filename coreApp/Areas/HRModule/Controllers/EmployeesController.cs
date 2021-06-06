using System.Linq;
using System.Web.Mvc;
using System;
using System.Collections.Generic;
using coreApp.Controllers;
using coreApp.DAL;
using Microsoft.AspNet.Identity;
using coreLib.Objects;
using coreLib.Extensions;
using Module.DB;
using Module.Core;
using System.Data.Linq;

namespace coreApp.Areas.HRModule.Controllers
{
    public class EmployeesController : Base_NoCoreHRStaffController
    {
        public ActionResult Index()
        {
            return View();
        }
        
        [EmployeeAuthorize("id")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new Raw_ActionResult(ModuleConstants.BAD_REQUEST);
            }

            using (dalDataContext context = new dalDataContext())
            {
                tblEmployee employee = context.tblEmployees.Where(x => x.EmployeeId == id).SingleOrDefault();
                if (employee == null)
                {
                    return new Raw_ActionResult(ModuleConstants.ID_NOT_FOUND);
                }
                else
                {
                    ViewBag.Employee = employee;

                    return View(employee);
                }
            }
        }
        
        public ActionResult Create()
        {
            ViewBag.Title = "Employee";
            

            ViewBag.uiIsReadOnly = false;
            ViewBag.uiIncludeId = false;

            

            return PartialView("_Employee", new tblEmployee());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Create(tblEmployee model)
        {
            queryResult res = new queryResult { IsSuccessful = true, Data = null, Err = "", Remarks = "" };

            try
            {
                using (dalDataContext context = new dalDataContext())
                {
                    if (ModelState.IsValid)
                    {
                        if (context.AspNetUsers.Where(x => x.Email == model.Email.ToLower().Trim()).Any() ||
                        context.tblEmployees.Where(x => x.Email == model.Email.ToLower().Trim()).Any())
                        {
                            ModelState.AddModelError("EmailExists", "Email already exists");
                        }
                    }

                    if (ModelState.IsValid)
                    {

                        tblEmployee employee = new tblEmployee
                        {
                            LastName = model.LastName,
                            FirstName = model.FirstName,
                            MiddleName = model.MiddleName,
                            NameExt = model.NameExt,
                            Email = model.Email.ToLower().Trim(),
                            IdNo = model.IdNo
                        };

                        context.tblEmployees.InsertOnSubmit(employee);
                        context.SubmitChanges();

                        res.Data = employee.EmployeeId;
                        res.Remarks = "Record was successfully created";

                        MvcApplication.HLLog.InsertLog("Employee Profile", employee.EmployeeId, employee.FullName);
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

        [EmployeeAuthorize("id")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new Raw_ActionResult(ModuleConstants.BAD_REQUEST);
            }

            ViewBag.Title = "Employee";
            

            ViewBag.uiIsReadOnly = false;
            ViewBag.uiIncludeId = true;

            

            using (dalDataContext context = new dalDataContext())
            {
                tblEmployee Employee = context.tblEmployees.Where(x => x.EmployeeId == id).Single();
                if (Employee == null)
                {
                    return new Raw_ActionResult(ModuleConstants.ID_NOT_FOUND);
                }
                else
                {
                    return PartialView("_Employee", Employee);
                }
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(tblEmployee model, bool? GeoAuth_DeviceRef_Reset)
        {
            queryResult res = new queryResult { IsSuccessful = true, Data = null, Err = "", Remarks = "" };

            try
            {
                using (dalDataContext context = new dalDataContext())
                {
                    tblEmployee employee = context.tblEmployees.Where(x => x.EmployeeId == model.EmployeeId).SingleOrDefault();

                    if (context.AspNetUsers.ToList().Where(x => (string.IsNullOrEmpty(employee.UserId) || x.Id != employee.UserId) && x.Email.ToCleanString() == model.Email.ToCleanString()).Any() ||
                        context.tblEmployees.ToList().Where(x => x.EmployeeId != employee.EmployeeId && x.Email.ToCleanString() == model.Email.ToCleanString()).Any())
                    {
                        ModelState.AddModelError("EmailExists", "Email already exists");
                    }

                    if (ModelState.IsValid)
                    {

                        if (employee == null)
                        {
                            throw new Exception(ModuleConstants.ID_NOT_FOUND);
                        }
                        else
                        {
                            employee.LastName = model.LastName;
                            employee.FirstName = model.FirstName;
                            employee.MiddleName = model.MiddleName;
                            employee.NameExt = model.NameExt;
                            employee.Email = model.Email.ToLower().Trim();
                            employee.IdNo = model.IdNo;

                            if (GeoAuth_DeviceRef_Reset == true)
                            {
                                employee.GeoAuth_DeviceRef = null;
                            }

                            ModifiedMemberInfo[] mmi = context.tblEmployees.GetModifiedMembers(employee);
                            context.SubmitChanges();
                            
                            res.Remarks = "Record was successfully updated";

                            MvcApplication.HLLog.UpdateLog("Employee Profile", employee.EmployeeId, employee.FullName, mmi);
                            if (GeoAuth_DeviceRef_Reset == true)
                            {
                                MvcApplication.HLLog.WriteLog("Employee Profile", "Reset Geo-Authentication Device Id",
                                    string.Format("Reset Geo-Authentication Device Id of [{0}] {1}", employee.EmployeeId, employee.FullName));
                            }

                            Cache.Update();
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
        [EmployeeAuthorize("id")]
        public ActionResult Delete(int id)
        {
            queryResult res = new queryResult { IsSuccessful = true, Data = null, Err = "", Remarks = "" };

            try
            {
                using (dalDataContext context = new dalDataContext())
                {
                    if (ModelState.IsValid)
                    {

                        tblEmployee employee = context.tblEmployees.Where(x => x.EmployeeId == id).SingleOrDefault();
                        if (employee == null)
                        {
                            throw new Exception(ModuleConstants.ID_NOT_FOUND);
                        }
                        else
                        {

                            bool proceed = true;
                            IdentityResult result = null;

                            if(employee.UserId != null)
                            {
                                result = Account.DeleteUser(UserManager, employee.UserId);
                                proceed = result.Succeeded;
                            }

                            if (proceed)
                            {

                                context.tblEmployees.DeleteOnSubmit(employee);
                                context.SubmitChanges();

                                res.Remarks = "Record was successfully deleted";

                                MvcApplication.HLLog.DeleteLog("Employee Profile", employee.EmployeeId, employee.FullName);
                            }
                            else
                            {
                                AddErrors(result);
                            }
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
        
        
    }
}
