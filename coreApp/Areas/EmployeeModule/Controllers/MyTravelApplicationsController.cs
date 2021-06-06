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
    public class MyTravelApplicationsController : Base_NoCoreEmployeeController, IMyController
    {
        public tblEmployee employee { get; set; }

        public ActionResult Index()
        {
            using (dalDataContext context = new dalDataContext())
            {

                var model = context.tblTravelApplications.Where(x => x.EmployeeId == employee.EmployeeId).OrderBy(x => x.StartDate).ToList();
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
                tblTravelApplication app = context.tblTravelApplications.Where(x => x.EmployeeId == employee.EmployeeId && x.Id == id).SingleOrDefault();
                if (app == null)
                {
                    return new Raw_ActionResult(ModuleConstants.ID_NOT_FOUND);
                }
                else
                {
                    return PartialView("_TravelApplication", app);
                }
            }
        }

        public ActionResult Create()
        {

            ViewBag.Title = "Travel Application";
            

            ViewBag.uiIsReadOnly = false;
            ViewBag.uiIncludeId = false;

            tblTravelApplication model = new tblTravelApplication
            {
                EmployeeId = employee.EmployeeId,
                StartDate = DateTime.Today,
                EndDate = DateTime.Today                
            };

            return PartialView("_TravelApplication", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Create(tblTravelApplication model, HttpPostedFileBase SupportFilex)
        {
            queryResult res = new queryResult { IsSuccessful = true, Data = null, Err = "", Remarks = "" };

            try
            {
                using (dalDataContext context = new dalDataContext())
                {

                    if (SupportFilex != null)
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

                    if (context.tblTravelApplications.Where(x => x.EmployeeId == model.EmployeeId).ToList().Any(x => model.IsSingleDate ? x.Match(model.StartDate) : x.Match(model.StartDate, model.EndDate)))
                    {
                        AddError("Date(s) overlap with existing entry");
                    }
                    
                    if (ModelState.IsValid)
                    {
                        if (SupportFilex != null)
                        {
                            byte[] imgByte = new Byte[SupportFilex.ContentLength];
                            SupportFilex.InputStream.Read(imgByte, 0, SupportFilex.ContentLength);

                            model.SupportFile = imgByte;
                        }

                        tblTravelApplication app = new tblTravelApplication
                        {
                            EmployeeId = model.EmployeeId,
                            StartDate = model.StartDate,
                            EndDate = model.EndDate,
                            Purpose = model.Purpose,
                            Destination = model.Destination,
                            SupportFile = model.SupportFile
                        };

                        context.tblTravelApplications.InsertOnSubmit(app);
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
                tblTravelApplication app = context.tblTravelApplications.Where(x => x.EmployeeId == employee.EmployeeId && x.Id == id).Single();
                if (app == null)
                {
                    return new Raw_ActionResult(ModuleConstants.ID_NOT_FOUND);
                }
                else
                {
                    return PartialView("_TravelApplication", app);
                }
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(tblTravelApplication model, HttpPostedFileBase SupportFilex, string SupportFilex_Remove)
        {
            queryResult res = new queryResult { IsSuccessful = true, Data = null, Err = "", Remarks = "" };

            bool RemoveSupportFile = SupportFilex_Remove == "True";

            try
            {
                using (dalDataContext context = new dalDataContext())
                {
                    tblTravelApplication app = context.tblTravelApplications.Where(x => x.Id == model.Id).SingleOrDefault();
                    if (app == null)
                    {
                        throw new Exception(ModuleConstants.ID_NOT_FOUND);
                    }
                    else
                    {
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
                                if (SupportFilex != null)
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

                            if (context.tblTravelApplications.Where(x => x.Id != model.Id && x.EmployeeId == model.EmployeeId).ToList().Any(x => model.IsSingleDate ? x.Match(model.StartDate) : x.Match(model.StartDate, model.EndDate)))
                            {
                                AddError("Date(s) overlap with existing entry");
                            }
                        }

                        if (ModelState.IsValid)
                        {
                            if (SupportFilex != null)
                            {
                                byte[] imgByte = new Byte[SupportFilex.ContentLength];
                                SupportFilex.InputStream.Read(imgByte, 0, SupportFilex.ContentLength);

                                app.SupportFile = imgByte;
                            }

                            app.EmployeeId = model.EmployeeId;
                            app.StartDate = model.StartDate;
                            app.EndDate = model.EndDate;
                            app.Purpose = model.Purpose;
                            app.Destination = model.Destination;

                            context.SubmitChanges();

                            res.Remarks = "Record was successfully updated";
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
        public ActionResult Delete(int id)
        {
            queryResult res = new queryResult { IsSuccessful = true, Data = null, Err = "", Remarks = "" };

            try
            {
                using (dalDataContext context = new dalDataContext())
                {
                    tblTravelApplication app = context.tblTravelApplications.Where(x => x.Id == id).SingleOrDefault();
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
                        context.tblTravelApplications.DeleteOnSubmit(app);
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
                    tblTravelApplication app = context.tblTravelApplications.Where(x => x.Id == id).SingleOrDefault();
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
                tblTravelApplication app = context.tblTravelApplications.Where(x => x.Id == id).SingleOrDefault();
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
                    ReportName = "travelapp"
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
                    "inclusive-dates", "purpose", "destination", "date-served"
                };

                tblTravelApplication app = (tblTravelApplication)data;
                tblEmployee_Career career = new procs_tblEmployee(app.employee).CareerAsOfDate(app.StartDate);
                
                string[] fieldValues = new string[] {
                    career.Office == null ? "" : career.Office.Office,
                    app.employee.LastName.ToUpper(),
                    app.employee.FirstName.ToUpper(),
                    (app.employee.MiddleName ?? "").ToUpper(),
                    app.HasBeenSubmitted ? app.DateSubmitted.Value.ToString("dd MMM yyyy") : "",
                    career.Position == null ? "" : career.Position.Position,
                    career.MonthlyRate.ToCurrencyString(),
                    app.Date_Desc,
                    app.Purpose,
                    app.Destination,                    
                    app.HasBeenServed ? (app.IsApproved ? app.ApproveDate.Value.ToString("dd MMM yyyy") : app.DenyDate.Value.ToString("dd MMM yyyy")):""
                };

                Procs.ApplySignatories("hr", "travelapp", ref fields, ref fieldValues);

                wordDoc.MailMerge.Execute(fields, fieldValues);
            }
        }
        
    }
}