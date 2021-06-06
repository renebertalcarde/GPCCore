using coreApp.Areas.LeaveModule;
using coreApp.Controllers;
using coreApp.DAL;
using coreApp.Filters;
using coreApp.Interfaces;
using coreLib.Extensions;
using coreLib.Objects;
using reports;
using System;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using reports.AsposeLib;
using Aspose.Words;
using Newtonsoft.Json;
using System.Collections.Generic;
using Module.DB;
using Module.DB.Enums;
using Module.Leave;
using Module.DB.Procs;
using Module.Core;

namespace coreApp.Areas.EmployeeModule.Controllers
{
    [MyFilter]
    public class MyLeaveApplicationsController : Base_NoCoreEmployeeController, IMyController
    {
        public tblEmployee employee { get; set; }

        public ActionResult Index()
        {
            using (dalDataContext context = new dalDataContext())
            {

                var model = context.tblLeaveApplications.Where(x => x.EmployeeId == employee.EmployeeId).OrderBy(x => x.StartDate).ToList();
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
                tblLeaveApplication app = context.tblLeaveApplications.Where(x => x.EmployeeId == employee.EmployeeId && x.Id == id).SingleOrDefault();
                if (app == null)
                {
                    return new Raw_ActionResult(ModuleConstants.ID_NOT_FOUND);
                }
                else
                {
                    return PartialView("_LeaveApplication", app);
                }
            }
        }

        public ActionResult SelectLeaveType()
        {
            return PartialView();
        }

        public ActionResult Create(int leaveTypeId)
        {
            using (dalDataContext context = new dalDataContext())
            {
                ViewBag.uiIsReadOnly = false;
                ViewBag.uiIncludeId = false;

                tblLeaveType leaveType = context.tblLeaveTypes.Where(x => x.Id == leaveTypeId).SingleOrDefault();
                if (leaveType == null)
                {
                    throw new Exception("Leave Type Id not found");
                }

                tblLeaveApplication model = new tblLeaveApplication
                {
                    EmployeeId = employee.EmployeeId,
                    LeaveTypeId = leaveTypeId,
                    StartDate = DateTime.Today,
                    EndDate = DateTime.Today,
                    VL_WithinPH = true
                };

                return PartialView("_LeaveApplication", model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Create(tblLeaveApplication model, HttpPostedFileBase SupportFilex, string VL_Reason, string VL_Location, bool AutoSubmit = false)
        {
            queryResult res = new queryResult { IsSuccessful = true, Data = null, Err = "", Remarks = "" };

            try
            {
                using (dalDataContext context = new dalDataContext())
                {
                    LeaveRuleModel ruleModel = new LeaveRuleModel(model.LeaveType.Rules());

                    if (ruleModel.ApplicationRequiresSupport)
                    {
                        if (SupportFilex == null)
                        {
                            AddError("Support File is required");
                        }
                        else
                        {
                            if (!FixedSettings.SupportFileTypes.Split(',').Contains(SupportFilex.ContentType))
                            {
                                AddError("Invalid file type");
                            }

                            if (SupportFilex.ContentLength > FixedSettings.SupportFileSize)
                            {
                                AddError("File size exceeded allowed limit (" + FixedSettings.SupportFileSize.ToBytes() + ")");
                            }
                        }
                    }

                    if (context.tblLeaveApplications.Where(x => x.EmployeeId == employee.EmployeeId && x.LeaveTypeId == model.LeaveTypeId).ToList().Any(x => model.IsSingleDate ? x.Match(model.StartDate) : x.Match(model.StartDate, model.EndDate)))
                    {
                        AddError("Date(s) overlap with existing entry");
                    }

                    prepModel(ref model, VL_Reason, VL_Location);

                    if (model.LeaveType.Category == (int)LeaveTypeCategory.VL)
                    {
                        if (!model.VL_SeekEmployment && string.IsNullOrEmpty(model.VL_Others))
                        {
                            AddError("Please provide reason for leave");
                        }

                        if (!model.VL_WithinPH && string.IsNullOrEmpty(model.VL_Abroad))
                        {
                            AddError("Please provide more details on where leave will be spent");
                        }
                    }
                    else if (model.LeaveType.Category == (int)LeaveTypeCategory.SL)
                    {
                        if (string.IsNullOrEmpty(model.SL))
                        {
                            AddError("Please provide more details about your sickness and/or hospitalization");
                        }
                    }

                    if (ModelState.IsValid)
                    {
                        tblEmployee_LeaveCredit lc = new tblEmployee_LeaveCredit
                        {
                            IsDrEntry = true,
                            StartDate = model.StartDate,
                            EndDate = model.EndDate,
                            IncludeRestDays = model.IncludeRestDays,
                            EmployeeId = employee.EmployeeId,
                            StartDate_IsHalfDay = model.StartDate_IsHalfDay,
                            EndDate_IsHalfDay = model.EndDate_IsHalfDay
                        };

                        Module.Time.Procs.UpdateDr(ref lc);
                        double leaveBalance = new employeeLeave(employee.EmployeeId, model.LeaveTypeId).LeaveBalance();

                        if (leaveBalance < lc.Dr)
                        {
                            AddError("Not enough leave credits earned");
                        }
                    }

                    if (ModelState.IsValid)
                    {
                        if (ruleModel.ApplicationRequiresSupport)
                        {
                            byte[] imgByte = new Byte[SupportFilex.ContentLength];
                            SupportFilex.InputStream.Read(imgByte, 0, SupportFilex.ContentLength);

                            model.SupportFile = imgByte;
                        }

                        tblLeaveApplication app = new tblLeaveApplication
                        {
                            EmployeeId = model.EmployeeId,
                            LeaveTypeId = model.LeaveTypeId,
                            StartDate = model.StartDate,
                            EndDate = model.EndDate,
                            StartDate_IsHalfDay = model.StartDate_IsHalfDay,
                            EndDate_IsHalfDay = model.EndDate_IsHalfDay,
                            IncludeRestDays = model.IncludeRestDays,
                            SupportFile = model.SupportFile,
                            VL_SeekEmployment = model.VL_SeekEmployment,
                            VL_Others = model.VL_Others,
                            VL_WithinPH = model.VL_WithinPH,
                            VL_Abroad = model.VL_Abroad,
                            SL_InHospital = model.SL_InHospital,
                            SL = model.SL,
                            Commutation_Requested = model.Commutation_Requested
                        };

                        if (app.IsSingleDate)
                        {
                            app.EndDate_IsHalfDay = false;
                        }

                        if (AutoSubmit)
                        {
                            app.DateSubmitted = DateTime.Now;
                        }

                        context.tblLeaveApplications.InsertOnSubmit(app);
                        context.SubmitChanges();

                        res.Remarks = "Record was successfully created";
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

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new Raw_ActionResult(ModuleConstants.BAD_REQUEST);
            }
            
            ViewBag.uiIsReadOnly = false;
            ViewBag.uiIncludeId = true;

            using (dalDataContext context = new dalDataContext())
            {
                tblLeaveApplication app = context.tblLeaveApplications.Where(x => x.EmployeeId == employee.EmployeeId && x.Id == id).Single();
                if (app == null)
                {
                    return new Raw_ActionResult(ModuleConstants.ID_NOT_FOUND);
                }
                else
                {
                    return PartialView("_LeaveApplication", app);
                }
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(tblLeaveApplication model, HttpPostedFileBase SupportFilex, string SupportFilex_Remove, string VL_Reason, string VL_Location, string SL_Reason, bool AutoSubmit = false)
        {
            queryResult res = new queryResult { IsSuccessful = true, Data = null, Err = "", Remarks = "" };

            bool RemoveSupportFile = SupportFilex_Remove == "True";

            try
            {
                using (dalDataContext context = new dalDataContext())
                {
                    LeaveRuleModel ruleModel = new LeaveRuleModel(model.LeaveType.Rules());

                    tblLeaveApplication app = context.tblLeaveApplications.Where(x => x.Id == model.Id).SingleOrDefault();
                    if (app == null)
                    {
                        throw new Exception(ModuleConstants.ID_NOT_FOUND);
                    }

                    if (app.Application_Status != (int)DocumentStatus.Draft && app.Application_Status != (int)DocumentStatus.Returned)
                    {
                        AddError("Application has already been submitted");
                    }
                    else
                    {
                        if (RemoveSupportFile)
                        {
                            app.SupportFile = null;
                        }
                        else
                        {
                            if (ruleModel.ApplicationRequiresSupport)
                            {
                                if (SupportFilex == null)
                                {
                                    if (app.SupportFile == null)
                                    {
                                        AddError("Support File is required");
                                    }                                    
                                }
                                else
                                {
                                    if (!FixedSettings.SupportFileTypes.Split(',').Contains(SupportFilex.ContentType))
                                    {
                                        AddError("Invalid file type");
                                    }

                                    if (SupportFilex.ContentLength > FixedSettings.SupportFileSize)
                                    {
                                        AddError("File size exceeded allowed limit (" + FixedSettings.SupportFileSize.ToBytes() + ")");
                                    }
                                }
                            }
                        }

                        if (context.tblLeaveApplications.Where(x => x.Id != model.Id && x.EmployeeId == model.EmployeeId && x.LeaveTypeId == model.LeaveTypeId).ToList().Any(x => model.IsSingleDate ? x.Match(model.StartDate) : x.Match(model.StartDate, model.EndDate)))
                        {
                            AddError("Date(s) overlap with existing entry");
                        }

                        prepModel(ref model, VL_Reason, VL_Location);

                        if (model.LeaveType.Category == (int)LeaveTypeCategory.VL)
                        {
                            if (!model.VL_SeekEmployment && string.IsNullOrEmpty(model.VL_Others))
                            {
                                AddError("Please provide reason for leave");
                            }

                            if (!model.VL_WithinPH && string.IsNullOrEmpty(model.VL_Abroad))
                            {
                                AddError("Please provide more details on where leave will be spent");
                            }
                        }
                        else if (model.LeaveType.Category == (int)LeaveTypeCategory.SL)
                        {
                            if (string.IsNullOrEmpty(model.SL))
                            {
                                AddError("Please provide more details about your sickness and/or hospitalization");
                            }
                        }
                    }

                    if (ModelState.IsValid)
                    {
                        tblEmployee_LeaveCredit lc = new tblEmployee_LeaveCredit
                        {
                            IsDrEntry = true,
                            StartDate = model.StartDate,
                            EndDate = model.EndDate,
                            IncludeRestDays = model.IncludeRestDays,
                            EmployeeId = employee.EmployeeId,
                            StartDate_IsHalfDay = model.StartDate_IsHalfDay,
                            EndDate_IsHalfDay = model.EndDate_IsHalfDay
                        };

                        Module.Time.Procs.UpdateDr(ref lc);
                        double leaveBalance = new employeeLeave(employee.EmployeeId, model.LeaveTypeId).LeaveBalance();

                        if (leaveBalance < lc.Dr)
                        {
                            AddError("Not enough leave credits earned");
                        }
                    }

                    if (ModelState.IsValid)
                    {
                        if (ruleModel.ApplicationRequiresSupport && SupportFilex != null)
                        {
                            byte[] imgByte = new Byte[SupportFilex.ContentLength];
                            SupportFilex.InputStream.Read(imgByte, 0, SupportFilex.ContentLength);

                            app.SupportFile = imgByte;
                        }

                        app.EmployeeId = model.EmployeeId;
                        app.LeaveTypeId = model.LeaveTypeId;
                        app.StartDate = model.StartDate;
                        app.EndDate = model.EndDate;
                        app.StartDate_IsHalfDay = model.StartDate_IsHalfDay;
                        app.EndDate_IsHalfDay = model.EndDate_IsHalfDay;
                        app.IncludeRestDays = model.IncludeRestDays;
                        app.VL_SeekEmployment = model.VL_SeekEmployment;
                        app.VL_Others = model.VL_Others;
                        app.VL_WithinPH = model.VL_WithinPH;
                        app.VL_Abroad = model.VL_Abroad;
                        app.SL_InHospital = model.SL_InHospital;
                        app.SL = model.SL;
                        app.Commutation_Requested = model.Commutation_Requested;

                        if (app.IsSingleDate)
                        {
                            app.EndDate_IsHalfDay = false;
                        }

                        if (AutoSubmit)
                        {
                            app.DateSubmitted = DateTime.Now;
                        }

                        context.SubmitChanges();

                        res.Remarks = "Record was successfully updated";
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
        public ActionResult Delete(int id)
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

                    if (app.HasBeenSubmitted)
                    {
                        AddError("Application has already been submitted");
                    }

                    if (ModelState.IsValid)
                    {
                        context.tblLeaveApplications.DeleteOnSubmit(app);
                        context.SubmitChanges();

                        res.Remarks = "Record was successfully deleted";
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
        public ActionResult Submit(int id)
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
                        if (app.HasBeenSubmitted)
                        {
                            AddError("Application has already been submitted");
                        }

                        if (ModelState.IsValid)
                        {
                            app.DateSubmitted = DateTime.Now;

                            context.SubmitChanges();

                            res.Remarks = "Record was successfully submitted";

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

        public ActionResult Print(int id, bool dlWord = false, string serverPath = null)
        {
            using (dalDataContext context = new dalDataContext())
            {
                tblLeaveApplication app = context.tblLeaveApplications.Where(x => x.Id == id).SingleOrDefault();
                if (app == null)
                {
                    throw new Exception(ModuleConstants.ID_NOT_FOUND);
                }

                if (!app.HasBeenSubmitted)
                {
                    throw new Exception("Application has not been submitted");
                }

                string fn = app.employee.LastName.ToCleanString();

                objData CustomData = new objData
                {
                    ApplicationName = MvcApplication.ApplicationLicense.Info.ApplicationName,
                    ModuleName = "HR",
                    ReportName = "leaveapp"
                };

                string appPath = serverPath ?? Server.MapPath("~/");

                return new asposeWordsTemplateReport(CustomizeDoc_Aspose, null, FixedSettings.ApplicationName, FixedSettings.Owner,
                    new ReportFooterParams(appPath), CustomData)
                    .Get(CustomData.ReportName, fn, app, dlWord);

            }
        }

        public void CustomizeDoc_Aspose(object data, ref Aspose.Words.Document wordDoc)
        {
            using (dalDataContext context = new dalDataContext())
            {
                string[] fields = new string[] {
                    "office", "lastname", "firstname", "middlename", "date-of-filing", "position", "salary", "is-vac", "is-vac-seek", "is-vac-others", "is-vac-others-specify", "is-sick",
                    "is-maternity", "tol-others", "tol-others-specify", "is-within", "is-abroad", "is-abroad-specify", "is-inhospital", "is-inhospital-specify", "no-of-days",
                    "inclusive-dates", "is-comm-requested", "is-comm-notrequested", "cert-of-lc", "vl", "sl", "total", "approved-no-of-days", "approved-no-of-days-wopay", "is-disapproved-reason", "date-served",
                    "rec-approval", "rec-disapproval", "recommendedby", "recommendedmessage", "actionby"
                };

                tblLeaveApplication app = (tblLeaveApplication)data;
                tblEmployee_Career career = new procs_tblEmployee(app.employee).CareerAsOfDate(app.StartDate);

                tblEmployee_LeaveCredit lc = new tblEmployee_LeaveCredit { EmployeeId = employee.EmployeeId, StartDate = app.StartDate, StartDate_IsHalfDay = app.StartDate_IsHalfDay, EndDate = app.EndDate, EndDate_IsHalfDay = app.EndDate_IsHalfDay, IsDrEntry = true };
                Module.Time.Procs.UpdateDr(ref lc);

                tblEmployee_LeaveCredit lc_approved = app.IsApproved ? new tblEmployee_LeaveCredit { EmployeeId = employee.EmployeeId, StartDate = app.Approved_StartDate.Value, StartDate_IsHalfDay = app.Approved_StartDate_IsHalfDay.Value, EndDate = app.Approved_EndDate.Value, EndDate_IsHalfDay = app.Approved_EndDate_IsHalfDay.Value, IsDrEntry = true } : null;
                if (lc_approved != null) Module.Time.Procs.UpdateDr(ref lc_approved);

                string approved_wopay = "";
                if (app.IsApproved)
                {
                    string[] tmp = JsonConvert.DeserializeObject<string[]>(app.Approved_WithoutPay_Data);
                    List<leaveEntryModel> LWOP = tmp.Select(x => JsonConvert.DeserializeObject<leaveEntryModel>(x)).ToList();

                    double t = 0;
                    foreach(leaveEntryModel wopay in LWOP)
                    {
                        tblEmployee_LeaveCredit lc_approved_wopay = new tblEmployee_LeaveCredit { EmployeeId = employee.EmployeeId, StartDate = wopay.StartDate, StartDate_IsHalfDay = wopay.StartDate_IsHalfDay, EndDate = wopay.EndDate, EndDate_IsHalfDay = wopay.EndDate_IsHalfDay, IsDrEntry = true };
                        Module.Time.Procs.UpdateDr(ref lc_approved_wopay);

                        t += lc_approved_wopay.Dr;
                    }

                    approved_wopay = t.ToString("#,##0.#");
                }

                string recommendapproval = "";
                string recommenddisapproval = "";
                string recommendedby = "";
                string recommendedmessage = "";
                string actionby = "";

                if (app.IsRecommended)
                {
                    recommendedby = app.RecommendedBy;
                    recommendedmessage = app.RecommendMessage;

                    if (app.RecommendAction == true)
                    {
                        recommendapproval = "X";
                    }
                    else
                    {
                        recommenddisapproval = "X";
                    }
                }

                if (app.IsApproved)
                {
                    actionby = app.ApprovedBy;
                }
                else if (app.IsDenied)
                {
                    actionby = app.DeniedBy;
                }

                string[] fieldValues = new string[] {
                    career.Office == null ? "" : career.Office.Office,
                    app.employee.LastName.ToUpper(),
                    app.employee.FirstName.ToUpper(),
                    (app.employee.MiddleName ?? "").ToUpper(),
                    app.HasBeenSubmitted ? app.DateSubmitted.Value.ToString("dd MMM yyyy") : "",
                    career.Position == null ? "" : career.Position.Position,
                    career.MonthlyRate.ToCurrencyString(),
                    app.LeaveType.Category == (int)LeaveTypeCategory.VL ? "X" : "",
                    app.LeaveType.Category == (int)LeaveTypeCategory.VL && app.VL_SeekEmployment ? "X" : "",
                    app.LeaveType.Category == (int)LeaveTypeCategory.VL && !app.VL_SeekEmployment ? "X" : "",
                    app.LeaveType.Category == (int)LeaveTypeCategory.VL && !app.VL_SeekEmployment ? app.VL_Others : "",
                    app.LeaveType.Category == (int)LeaveTypeCategory.SL ? "X" : "",
                    app.LeaveType.Category == (int)LeaveTypeCategory.Maternity ? "X" : "",
                    app.LeaveType.Category == (int)LeaveTypeCategory.Others ? "X" : "",
                    app.LeaveType.Category == (int)LeaveTypeCategory.Others ? app.LeaveType.Description : "",
                    app.LeaveType.Category == (int)LeaveTypeCategory.VL && app.VL_WithinPH ? "X" : "",
                    app.LeaveType.Category == (int)LeaveTypeCategory.VL && !app.VL_WithinPH ? "X" : "",
                    app.LeaveType.Category == (int)LeaveTypeCategory.VL && !app.VL_WithinPH ? app.VL_Abroad : "",
                    app.LeaveType.Category == (int)LeaveTypeCategory.SL && app.SL_InHospital == true ? "X" : "",
                    app.LeaveType.Category == (int)LeaveTypeCategory.SL && !string.IsNullOrEmpty(app.SL) ? app.SL : "",
                    lc.Dr.ToString("#,##0.##"),
                    app.Date_Desc,
                    app.Commutation_Requested ? "X" : "",
                    !app.Commutation_Requested ? "X" : "",
                    app.DateSubmitted == null ? "" : app.DateSubmitted.Value.ToString("dd MMM yyyy"),
                    getTotal(app, LeaveTypeCategory.VL).ToString("#,##0.##"),
                    getTotal(app, LeaveTypeCategory.SL).ToString("#,##0.##"),
                    (getTotal(app, LeaveTypeCategory.VL) + getTotal(app, LeaveTypeCategory.SL)).ToString("#,##0.##"),
                    app.IsApproved ? lc_approved.Dr.ToString("#,##0.##") : "",
                    approved_wopay,
                    !app.IsApproved ? app.DenyMessage : "",
                    app.HasBeenServed ? (app.IsApproved ? app.ApproveDate.Value.ToString("dd MMM yyyy") : app.DenyDate.Value.ToString("dd MMM yyyy")):"",
                    recommendapproval,
                    recommenddisapproval,
                    recommendedby,
                    recommendedmessage,
                    actionby
                };

                Procs.ApplySignatories("hr", "leaveapp", ref fields, ref fieldValues);

                wordDoc.MailMerge.Execute(fields, fieldValues);
            }
        }


        private double getTotal(tblLeaveApplication app, LeaveTypeCategory category)
        {
            double ret = 0;
            foreach (tblLeaveType ltype in Cache.Get_Tables().LeaveTypes.Where(x => x.Category == (int)category))
            {
                employeeLeave empLeave = new employeeLeave(employee.EmployeeId, ltype.Id, app.DateSubmitted.Value);
                if (!empLeave.IsApplicable) continue;

                ret += empLeave.LeaveBalance(app.DateSubmitted.Value);
            }
            return ret;
        }



        private void prepModel(ref tblLeaveApplication model, string VL_Reason, string VL_Location)
        {
            model.VL_SeekEmployment = VL_Reason == "VL_SeekEmployment";
            if (model.VL_SeekEmployment)
            {
                model.VL_Others = null;
            }

            model.VL_WithinPH = VL_Location == "VL_WithinPH";
            if (model.VL_WithinPH)
            {
                model.VL_Abroad = null;
            }

            if (model.LeaveType.Category != (int)LeaveTypeCategory.VL)
            {
                model.VL_Others = null;
                model.VL_Abroad = null;
            }

            if (model.LeaveType.Category != (int)LeaveTypeCategory.SL)
            {
                model.SL = null;
            }
        }
    }
}