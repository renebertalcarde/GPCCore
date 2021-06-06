using coreApp.Controllers;
using coreApp.DAL;
using coreLib.Objects;
using Module.Core;
using Module.DB;
using Module.DB.Enums;
using Module.Leave;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace coreApp.Areas.HRModule.Controllers
{
    [UserAccessAuthorize(allowedAccess: "hr_emp_leave_app,hr_emp_leave_app_approve")]
    public class LeaveApplicationsController : HLBase_NoCoreAuthorizedController
    {
        UserAccess access = coreApp.Cache.Get().userAccess;

        public ActionResult Index(bool showApproved = false, bool showDenied = false, bool showRecommended = false)
        {
            bool approveOny = !access.CanAccessLeaveApplications && access.CanApproveLeaveApplications;

            using (dalDataContext context = new dalDataContext())
            {
                List<tblLeaveApplication> model = context.tblLeaveApplications.ToList()
                    .Where(x => Cache.Get().userAccess.AllowedEmployee(x.EmployeeId, disableMe: true, forLeave: true) && x.HasBeenSubmitted && 
                        (showApproved || !x.IsApproved) && (showDenied || !x.IsDenied) && (approveOny ? x.IsRecommended : (showRecommended || !x.IsRecommended))
                    )
                    .OrderByDescending(x => x.DateSubmitted).ToList();

                ViewBag.ShowApproved = showApproved;
                ViewBag.ShowDenied = showDenied;
                ViewBag.ShowRecommended = showRecommended;

                return View(model);
            }
        }

        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new Raw_ActionResult(ModuleConstants.BAD_REQUEST);
            }
            
            ViewBag.Title = "Leave Application";
            

            ViewBag.uiIsReadOnly = true;
            ViewBag.uiIncludeId = true;

            using (dalDataContext context = new dalDataContext())
            {
                tblLeaveApplication app = context.tblLeaveApplications.Where(x => x.Id == id).SingleOrDefault();
                if (app == null)
                {
                    return new Raw_ActionResult(ModuleConstants.ID_NOT_FOUND);
                }
                else
                {
                    return PartialView("~/Areas/EmployeeModule/Views/MyLeaveApplications/_LeaveApplication.cshtml", app);
                }
            }
        }

        [HttpPost]
        public ActionResult Return(int id, string message)
        {
            queryResult res = new queryResult { IsSuccessful = true, Data = null, Err = "", Remarks = "" };

            try
            {
                using (dalDataContext context = new dalDataContext())
                {
                    tblLeaveApplication app = context.tblLeaveApplications.Where(x => x.Id == id).SingleOrDefault();
                    if (app == null)
                    {
                        throw new Exception(ModuleConstants.ID_NOT_FOUND);
                    }
                    else
                    {
                        if (!app.HasBeenSubmitted)
                        {
                            AddError("Application has not been submitted");
                        }

                        if (app.IsRecommended)
                        {
                            AddError("Application has already been recommended");
                        }

                        if (!access.CanAccessLeaveApplications)
                        {
                            AddError("Invalid action for approver");
                        }

                        if (ModelState.IsValid)
                        {
                            app.ReturnDate = DateTime.Now;
                            app.ReturnedBy_UserId = Cache.Get().userAccess.user.Id;
                            app.ReturnMessage = message;
                            
                            context.SubmitChanges();

                            res.Remarks = "Record was successfully returned";
                            
                        }
                        else
                        {
                            throw new Exception(coreProcs.ShowErrors(ModelState));
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

        [HttpPost]
        public ActionResult Deny(int id, string message)
        {
            queryResult res = new queryResult { IsSuccessful = true, Data = null, Err = "", Remarks = "" };

            try
            {
                using (dalDataContext context = new dalDataContext())
                {
                    tblLeaveApplication app = context.tblLeaveApplications.Where(x => x.Id == id).SingleOrDefault();
                    if (app == null)
                    {
                        throw new Exception(ModuleConstants.ID_NOT_FOUND);
                    }
                    else
                    {
                        if (!app.IsRecommended)
                        {
                            AddError("Application has not been recommended");
                        }

                        if (app.HasBeenServed)
                        {
                            AddError("Application has already been served");
                        }

                        if (!access.CanApproveLeaveApplications)
                        {
                            AddError("Invalid action for pre-approver");
                        }

                        if (ModelState.IsValid)
                        {
                            app.DenyDate = DateTime.Now;
                            app.DeniedBy_UserId = Cache.Get().userAccess.user.Id;
                            app.DenyMessage = message;

                            context.SubmitChanges();

                            res.Remarks = "Record was successfully denied";

                        }
                        else
                        {
                            throw new Exception(coreProcs.ShowErrors(ModelState));
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

        [HttpPost]
        public ActionResult Recommend(int id, bool approve, string message)
        {
            queryResult res = new queryResult { IsSuccessful = true, Data = null, Err = "", Remarks = "" };

            try
            {
                using (dalDataContext context = new dalDataContext())
                {
                    tblLeaveApplication app = context.tblLeaveApplications.Where(x => x.Id == id).SingleOrDefault();
                    if (app == null)
                    {
                        throw new Exception(ModuleConstants.ID_NOT_FOUND);
                    }
                    else
                    {
                        if (!app.HasBeenSubmitted)
                        {
                            AddError("Application has not been submitted");
                        }

                        if (app.IsRecommended)
                        {
                            AddError("Application has already been recommended");
                        }

                        if (!access.CanAccessLeaveApplications)
                        {
                            AddError("Invalid action for approver");
                        }

                        if (ModelState.IsValid)
                        {
                            app.RecommendAction = approve;
                            app.RecommendDate = DateTime.Now;
                            app.RecommendedBy_UserId = Cache.Get().userAccess.user.Id;
                            app.RecommendMessage = message;

                            context.SubmitChanges();

                            res.Remarks = "Record was successfully denied";

                        }
                        else
                        {
                            throw new Exception(coreProcs.ShowErrors(ModelState));
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

        [HttpPost]
        public ActionResult Approve(int id, string NoWithPay, string LWP, string LWOP)
        {
            queryResult res = new queryResult { IsSuccessful = true, Data = null, Err = "", Remarks = "" };

            try
            {
                leaveEntryModel withPay = JsonConvert.DeserializeObject<leaveEntryModel>(LWP);

                using (dalDataContext context = new dalDataContext())
                {
                    tblLeaveApplication app = context.tblLeaveApplications.Where(x => x.Id == id).SingleOrDefault();
                    if (app == null)
                    {
                        throw new Exception(ModuleConstants.ID_NOT_FOUND);
                    }
                    else
                    {
                        if (!app.IsRecommended)
                        {
                            AddError("Application has not been recommended");
                        }

                        if (app.HasBeenServed)
                        {
                            AddError("Application has already been served");
                        }

                        if (!access.CanApproveLeaveApplications)
                        {
                            AddError("Invalid action for pre-approver");
                        }

                        if (ModelState.IsValid)
                        {
                            tblEmployee_LeaveCredit lc = new tblEmployee_LeaveCredit
                            {
                                IsDrEntry = true,
                                StartDate = app.StartDate,
                                EndDate = app.EndDate,
                                IncludeRestDays = app.IncludeRestDays,
                                EmployeeId = app.EmployeeId,
                                StartDate_IsHalfDay = app.StartDate_IsHalfDay,
                                EndDate_IsHalfDay = app.EndDate_IsHalfDay
                            };

                            Module.Time.Procs.UpdateDr(ref lc);
                            double leaveBalance = new employeeLeave(app.EmployeeId, app.LeaveTypeId).LeaveBalance();

                            if (leaveBalance < lc.Dr)
                            {
                                AddError("Not enough leave credits earned");
                            }
                        }

                        if (ModelState.IsValid)
                        {
                            app.Approved_StartDate = withPay.StartDate;
                            app.Approved_EndDate = withPay.EndDate;
                            app.Approved_StartDate_IsHalfDay = withPay.StartDate_IsHalfDay;
                            app.Approved_EndDate_IsHalfDay = withPay.EndDate_IsHalfDay;

                            app.Approved_WithoutPay_Data = LWOP;

                            app.ApproveDate = DateTime.Now;
                            app.ApprovedBy_UserId = Cache.Get().userAccess.user.Id;

                            context.SubmitChanges();

                            updateLeaveCredits(app);

                            res.Remarks = "Record was successfully approved";

                        }
                        else
                        {
                            throw new Exception(coreProcs.ShowErrors(ModelState));
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

        [HttpPost]
        public ActionResult RevertApproval(int id)
        {
            queryResult res = new queryResult { IsSuccessful = true, Data = null, Err = "", Remarks = "" };

            try
            {
                using (dalDataContext context = new dalDataContext())
                {
                    tblLeaveApplication app = context.tblLeaveApplications.Where(x => x.Id == id).SingleOrDefault();
                    if (app == null)
                    {
                        throw new Exception(ModuleConstants.ID_NOT_FOUND);
                    }
                    else
                    {
                        if (!app.HasBeenServed)
                        {
                            AddError("Application has not been served");
                        }

                        if (!access.CanApproveLeaveApplications)
                        {
                            AddError("Invalid action for pre-approver");
                        }

                        if (ModelState.IsValid)
                        {
                            if (app.IsApproved)
                            {
                                app.Approved_StartDate = null;
                                app.Approved_EndDate = null;
                                app.Approved_StartDate_IsHalfDay = null;
                                app.Approved_EndDate_IsHalfDay = null;
                                app.Approved_WithoutPay_Data = null;
                                app.ApproveDate = null;
                                app.ApprovedBy_UserId = null;

                                context.SubmitChanges();
                                updateLeaveCredits(app, true);
                            }
                            else
                            {
                                app.DenyDate = null;
                                app.DeniedBy_UserId = null;
                                app.DenyMessage = null;

                                context.SubmitChanges();
                            }

                            res.Remarks = "Record was successfully reverted";

                        }
                        else
                        {
                            throw new Exception(coreProcs.ShowErrors(ModelState));
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

        private void updateLeaveCredits(tblLeaveApplication app, bool revertApproval = false)
        {

            List<leaveEntryModel> LWOP = new List<leaveEntryModel>();
            if (!string.IsNullOrEmpty(app.Approved_WithoutPay_Data))
            {
                string[] tmp = JsonConvert.DeserializeObject<string[]>(app.Approved_WithoutPay_Data);
                LWOP = tmp.Select(x => JsonConvert.DeserializeObject<leaveEntryModel>(x)).ToList();
            }

            using (dalDataContext context = new dalDataContext())
            {
                var existing = context.tblEmployee_LeaveCredits.Where(x => x.LeaveApplicationId == app.Id);
                context.tblEmployee_LeaveCredits.DeleteAllOnSubmit(existing);
                context.SubmitChanges();

                if (revertApproval) return;

                
                LeaveRuleModel ruleModel = new LeaveRuleModel(app.LeaveType.Rules());
                
                List<tblEmployee_LeaveCredit> newItems = new List<tblEmployee_LeaveCredit>();
                
                tblEmployee_LeaveCredit lc = new tblEmployee_LeaveCredit
                {
                    EmployeeId = app.EmployeeId,
                    LeaveApplicationId = app.Id,
                    StartDate = app.Approved_StartDate.Value,
                    StartDate_IsHalfDay = app.Approved_StartDate_IsHalfDay.Value,
                    EndDate = app.Approved_EndDate.Value,
                    EndDate_IsHalfDay = app.Approved_EndDate_IsHalfDay.Value,
                    EntryType = (int)LeaveEntryType.Manual,
                    Cr = 0,
                    Dr = 0,
                    IncludeRestDays = app.IncludeRestDays,
                    LeaveTypeId = app.LeaveTypeId,
                    Remarks = string.Format("Approved application [{0}]", app.Id),
                    EnteredBy_UserId = Cache.Get().userAccess.user.Id,
                    IsDrEntry = true,
                    WithoutPay = ruleModel.WithoutPay
                };
                                
                Module.Time.Procs.UpdateDr(ref lc);


                if (!LWOP.Any(x => lc.IsSingleDate ? x.Match(lc.StartDate) : x.Match(lc.StartDate, lc.EndDate)))
                {
                    newItems.Add(lc);
                }
                
                if (LWOP.Any())
                {
                    foreach (leaveEntryModel wopay in LWOP)
                    {
                        tblEmployee_LeaveCredit lc_approved_wopay = new tblEmployee_LeaveCredit
                        {
                            EmployeeId = app.EmployeeId,
                            LeaveApplicationId = app.Id,
                            StartDate = wopay.StartDate,
                            StartDate_IsHalfDay = wopay.StartDate_IsHalfDay,
                            EndDate = wopay.EndDate,
                            EndDate_IsHalfDay = wopay.EndDate_IsHalfDay,
                            EntryType = (int)LeaveEntryType.Manual,
                            Cr = 0,
                            Dr = 0,
                            IncludeRestDays = app.IncludeRestDays,
                            LeaveTypeId = app.LeaveTypeId,
                            Remarks = string.Format("Approved application [{0}]", app.Id),
                            EnteredBy_UserId = Cache.Get().userAccess.user.Id,
                            IsDrEntry = true,
                            WithoutPay = true
                        };

                        Module.Time.Procs.UpdateDr(ref lc_approved_wopay);
                        newItems.Add(lc_approved_wopay);
                    }
                }

                context.tblEmployee_LeaveCredits.InsertAllOnSubmit(newItems);                
                context.SubmitChanges();

            }
        }

        public ActionResult Print(int id, bool dlWord = false)
        {
            using(dalDataContext context = new dalDataContext())
            {
                tblLeaveApplication app = context.tblLeaveApplications.Where(x => x.Id == id).SingleOrDefault();
                if(app == null)
                {
                    throw new Exception(ModuleConstants.ID_NOT_FOUND);
                }
                
                return new EmployeeModule.Controllers.MyLeaveApplicationsController { employee = app.employee }.Print(id, dlWord, Server.MapPath("~/"));
            }            
        }

        public ActionResult ApproveUI(int id)
        {
            using (dalDataContext context = new dalDataContext())
            {
                tblLeaveApplication app = context.tblLeaveApplications.Where(x => x.Id == id).SingleOrDefault();
                if (app == null)
                {
                    throw new Exception(ModuleConstants.ID_NOT_FOUND);
                }

                return PartialView(new leaveApplicationApproveModel(app));
            }
        }
        
        public ActionResult GetLWOP(int id, string approvedWithPay)
        {
            leaveEntryModel withPay = null;

            if (!string.IsNullOrEmpty(approvedWithPay))
            {
                withPay = JsonConvert.DeserializeObject<leaveEntryModel>(approvedWithPay);
            }

            using (dalDataContext context = new dalDataContext())
            {
                tblLeaveApplication app = context.tblLeaveApplications.Where(x => x.Id == id).SingleOrDefault();
                if (app == null)
                {
                    throw new Exception(ModuleConstants.ID_NOT_FOUND);
                }

                leaveApplicationApproveModel leaveAppModel = new leaveApplicationApproveModel(app);
                leaveAppModel.WithPay = withPay;

                return PartialView(leaveAppModel.GetLWOP());
            }
        }
    }
}