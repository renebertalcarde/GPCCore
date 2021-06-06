using coreApp.Controllers;
using coreApp.DAL;
using coreLib.Objects;
using Module.Core;
using Module.DB;
using Module.DB.Enums;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace coreApp.Areas.HRModule.Controllers
{
    [UserAccessAuthorize(allowedAccess: "hr_emp_travel_app")]
    public class TravelApplicationsController : HLBase_NoCoreAuthorizedController
    {
        
        public ActionResult Index(bool showApproved = false, bool showDenied = false)
        {
            using (dalDataContext context = new dalDataContext())
            {
                List<tblTravelApplication> model = context.tblTravelApplications.ToList()
                    .Where(x => Cache.Get().userAccess.AllowedEmployee(x.EmployeeId) && x.HasBeenSubmitted &&
                        (showApproved || !x.IsApproved) && (showDenied || !x.IsDenied)
                    )
                    .OrderByDescending(x => x.DateSubmitted).ToList();

                ViewBag.ShowApproved = showApproved;
                ViewBag.ShowDenied = showDenied;

                return View(model);
            }
        }

        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new Raw_ActionResult(ModuleConstants.BAD_REQUEST);
            }
            
            ViewBag.Title = "Travel Application";
            

            ViewBag.uiIsReadOnly = true;
            ViewBag.uiIncludeId = true;

            using (dalDataContext context = new dalDataContext())
            {
                tblTravelApplication app = context.tblTravelApplications.Where(x => x.Id == id).SingleOrDefault();
                if (app == null)
                {
                    return new Raw_ActionResult(ModuleConstants.ID_NOT_FOUND);
                }
                else
                {
                    return PartialView("~/Areas/EmployeeModule/Views/MyTravelApplications/_TravelApplication.cshtml", app);
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
                    tblTravelApplication app = context.tblTravelApplications.Where(x => x.Id == id).SingleOrDefault();
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

                        if (app.HasBeenServed)
                        {
                            AddError("Application has already been served");
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
                    tblTravelApplication app = context.tblTravelApplications.Where(x => x.Id == id).SingleOrDefault();
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

                        if (app.HasBeenServed)
                        {
                            AddError("Application has already been served");
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
        public ActionResult Approve(int id, string NoWithPay, string LWP, string LWOP)
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
                        if (!app.HasBeenSubmitted)
                        {
                            AddError("Application has not been submitted");
                        }

                        if (app.HasBeenServed)
                        {
                            AddError("Application has already been served");
                        }

                        if (ModelState.IsValid)
                        {
                            app.ApproveDate = DateTime.Now;
                            app.ApprovedBy_UserId = Cache.Get().userAccess.user.Id;

                            context.SubmitChanges();

                            updateTravelCredits(app);

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
                    tblTravelApplication app = context.tblTravelApplications.Where(x => x.Id == id).SingleOrDefault();
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

                        if (ModelState.IsValid)
                        {
                            if (app.IsApproved)
                            {
                                app.ApproveDate = null;
                                app.ApprovedBy_UserId = null;

                                context.SubmitChanges();
                                updateTravelCredits(app, true);
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

        private void updateTravelCredits(tblTravelApplication app, bool revertApproval = false)
        {
            using (dalDataContext context = new dalDataContext())
            {
                var existing = context.tblEmployee_Travels.Where(x => x.ApplicationId == app.Id);
                context.tblEmployee_Travels.DeleteAllOnSubmit(existing);
                context.SubmitChanges();

                if (revertApproval) return;

                tblEmployee_Travel lc = new tblEmployee_Travel
                {
                    EmployeeId = app.EmployeeId,
                    ApplicationId = app.Id,
                    StartDate = app.StartDate,
                    EndDate = app.EndDate,
                    Purpose = "Approved Travel Application [" + app.Id + "]: " + app.Purpose,
                    Destination = app.Destination,
                    CreatedBy_UserId = Cache.Get().userAccess.user.Id,
                    CreateDate = DateTime.Now
                };
                
                context.tblEmployee_Travels.InsertOnSubmit(lc);                
                context.SubmitChanges();

            }
        }

        public ActionResult Print(int id, bool dlWord = false)
        {
            using(dalDataContext context = new dalDataContext())
            {
                tblTravelApplication app = context.tblTravelApplications.Where(x => x.Id == id).SingleOrDefault();
                if(app == null)
                {
                    throw new Exception(ModuleConstants.ID_NOT_FOUND);
                }
                
                return new EmployeeModule.Controllers.MyTravelApplicationsController { employee = app.employee }.Print(id, dlWord, Server.MapPath("~/"));
            }            
        }

        public ActionResult ApproveUI(int id)
        {
            using (dalDataContext context = new dalDataContext())
            {
                tblTravelApplication app = context.tblTravelApplications.Where(x => x.Id == id).SingleOrDefault();
                if (app == null)
                {
                    throw new Exception(ModuleConstants.ID_NOT_FOUND);
                }

                return PartialView(app);
            }
        }
    }
}