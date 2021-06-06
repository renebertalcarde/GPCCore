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
using Module.DB.Procs;
using Module.Core;

namespace coreApp.Areas.EmployeeModule.Controllers
{
    [MyFilter]
    public class MyOTApplicationsController : Base_NoCoreEmployeeController, IMyController
    {
        public tblEmployee employee { get; set; }

        public ActionResult Index()
        {
            using (dalDataContext context = new dalDataContext())
            {

                var model = context.tblOTApplications.Where(x => x.EmployeeId == employee.EmployeeId).OrderBy(x => x.StartDate).ToList();
                return View(model);
            }
        }

        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new Raw_ActionResult(ModuleConstants.BAD_REQUEST);
            }            

            ViewBag.uiIsReadOnly = true;
            ViewBag.uiIncludeId = true;

            using (dalDataContext context = new dalDataContext())
            {
                tblOTApplication app = context.tblOTApplications.Where(x => x.EmployeeId == employee.EmployeeId && x.Id == id).SingleOrDefault();
                if (app == null)
                {
                    return new Raw_ActionResult(ModuleConstants.ID_NOT_FOUND);
                }
                else
                {
                    return PartialView("_OTApplication", app);
                }
            }
        }

        public ActionResult Create()
        {            

            ViewBag.uiIsReadOnly = false;
            ViewBag.uiIncludeId = false;

            tblOTApplication model = new tblOTApplication
            {
                EmployeeId = employee.EmployeeId,
                StartDate = DateTime.Today,
                EndDate = DateTime.Today
            };

            return PartialView("_OTApplication", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Create(tblOTApplication model)
        {
            queryResult res = new queryResult { IsSuccessful = true, Data = null, Err = "", Remarks = "" };

            try
            {
                using (dalDataContext context = new dalDataContext())
                {                    
                    if (context.tblOTApplications.Where(x => x.EmployeeId == model.EmployeeId).ToList().Any(x => model.IsSingleDate ? x.Match(model.StartDate) : x.Match(model.StartDate, model.EndDate)))
                    {
                        AddError("Date(s) overlap with existing entry");
                    }
                    
                    if (ModelState.IsValid)
                    {
                        tblOTApplication app = new tblOTApplication
                        {
                            EmployeeId = model.EmployeeId,
                            OTTemplateId = model.OTTemplateId,
                            StartDate = model.StartDate,
                            EndDate = model.EndDate,
                            Remarks = model.Remarks
                        };

                        context.tblOTApplications.InsertOnSubmit(app);
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
                tblOTApplication app = context.tblOTApplications.Where(x => x.EmployeeId == employee.EmployeeId && x.Id == id).Single();
                if (app == null)
                {
                    return new Raw_ActionResult(ModuleConstants.ID_NOT_FOUND);
                }
                else
                {
                    return PartialView("_OTApplication", app);
                }
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(tblOTApplication model)
        {
            queryResult res = new queryResult { IsSuccessful = true, Data = null, Err = "", Remarks = "" };

            try
            {
                using (dalDataContext context = new dalDataContext())
                {
                    tblOTApplication app = context.tblOTApplications.Where(x => x.Id == model.Id).SingleOrDefault();
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
                        if (context.tblOTApplications.Where(x => x.Id != model.Id && x.EmployeeId == model.EmployeeId).ToList().Any(x => model.IsSingleDate ? x.Match(model.StartDate) : x.Match(model.StartDate, model.EndDate)))
                        {
                            AddError("Date(s) overlap with existing entry");
                        }
                    }

                    if (ModelState.IsValid)
                    {
                        app.EmployeeId = model.EmployeeId;
                        app.OTTemplateId = model.OTTemplateId;
                        app.StartDate = model.StartDate;
                        app.EndDate = model.EndDate;
                        app.Remarks = model.Remarks;

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
                    tblOTApplication app = context.tblOTApplications.Where(x => x.Id == id).SingleOrDefault();
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
                        context.tblOTApplications.DeleteOnSubmit(app);
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
                    tblOTApplication app = context.tblOTApplications.Where(x => x.Id == id).SingleOrDefault();
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
                tblOTApplication app = context.tblOTApplications.Where(x => x.Id == id).SingleOrDefault();
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
                    ReportName = "otapp"
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
                    "office", "lastname", "firstname", "middlename", "date-of-filing", "position", "salary",
                    "inclusive-dates", "time", "days", "purpose", "date-served", "breaks"
                };

                tblOTApplication app = (tblOTApplication)data;
                tblEmployee_Career career = new procs_tblEmployee(app.employee).CareerAsOfDate(app.StartDate);
                tblOTTemplate template = coreApp.Cache.Get_Tables().OTTemplates.Where(x => x.Id == app.OTTemplateId).Single();

                string[] fieldValues = new string[] {
                    career.Office == null ? "" : career.Office.Office,
                    app.employee.LastName.ToUpper(),
                    app.employee.FirstName.ToUpper(),
                    (app.employee.MiddleName ?? "").ToUpper(),
                    app.HasBeenSubmitted ? app.DateSubmitted.Value.ToString("dd MMM yyyy") : "",
                    career.Position == null ? "" : career.Position.Position,
                    career.MonthlyRate.ToCurrencyString(),
                    app.Date_Desc,
                    template.TimeString,
                    template.Days,
                    app.Remarks,
                    app.HasBeenServed ? (app.IsApproved ? app.ApproveDate.Value.ToString("dd MMM yyyy") : app.DenyDate.Value.ToString("dd MMM yyyy")):"",
                    template.AutoBreak.ToString("#,##0.###")
                };

                Procs.ApplySignatories("hr", "otapp", ref fields, ref fieldValues);

                wordDoc.MailMerge.Execute(fields, fieldValues);
            }
        }
        
    }
}