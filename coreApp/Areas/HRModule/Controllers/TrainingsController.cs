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

namespace coreApp.Areas.HRModule.Controllers
{
    public class TrainingsController : Base_NoCoreHRStaffController
    {
        public ActionResult Index()
        {
            using (hr2017DataContext context = new hr2017DataContext())
            {
                List<tblTraining> model = context.tblTrainings.OrderBy(x => x.Title).ToList();
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

            using (hr2017DataContext context = new hr2017DataContext())
            {
                tblTraining trn = context.tblTrainings.Where(x => x.Id == id).SingleOrDefault();
                if (trn == null)
                {
                    return new Raw_ActionResult(ModuleConstants.ID_NOT_FOUND);
                }
                else
                {
                    return PartialView("_Training", trn);
                }
            }
        }

        public ActionResult Create()
        {

            ViewBag.uiIsReadOnly = false;
            ViewBag.uiIncludeId = false;

            tblTraining model = new tblTraining();
            return PartialView("_Training", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Create(tblTraining model)
        {
            queryResult res = new queryResult { IsSuccessful = true, Data = null, Err = "", Remarks = "" };

            try
            {
                using (hr2017DataContext context = new hr2017DataContext())
                {

                    if (ModelState.IsValid)
                    {
                        tblTraining trn = new tblTraining
                        {
                            Title = model.Title,
                            Venue = model.Venue,
                            DateFrom = model.DateFrom,
                            DateTo = model.DateTo,
                            NoOfHours = model.NoOfHours,
                            ConductedBy = model.ConductedBy,
                            Type = model.Type,
                            LastUpdatedBy_UserId = coreApp.Cache.Get().userAccess.user.Id,
                            LastUpdated = DateTime.Now
                        };

                        context.tblTrainings.InsertOnSubmit(trn);
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

            using (hr2017DataContext context = new hr2017DataContext())
            {
                tblTraining trn = context.tblTrainings.Where(x => x.Id == id).Single();
                if (trn == null)
                {
                    return new Raw_ActionResult(ModuleConstants.ID_NOT_FOUND);
                }
                else
                {
                    return PartialView("_Training", trn);
                }
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(tblTraining model)
        {
            queryResult res = new queryResult { IsSuccessful = true, Data = null, Err = "", Remarks = "" };

            try
            {
                using (hr2017DataContext context = new hr2017DataContext())
                {
                    tblTraining trn = context.tblTrainings.Where(x => x.Id == model.Id).SingleOrDefault();
                    if (trn == null)
                    {
                        throw new Exception(ModuleConstants.ID_NOT_FOUND);
                    }

                    if (ModelState.IsValid)
                    {
                        trn.Title = model.Title;
                        trn.Venue = model.Venue;
                        trn.DateFrom = model.DateFrom;
                        trn.DateTo = model.DateTo;
                        trn.NoOfHours = model.NoOfHours;
                        trn.ConductedBy = model.ConductedBy;
                        trn.Type = model.Type;
                        trn.LastUpdatedBy_UserId = coreApp.Cache.Get().userAccess.user.Id;
                        trn.LastUpdated = DateTime.Now;

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
                using (hr2017DataContext context = new hr2017DataContext())
                {
                    tblTraining trn = context.tblTrainings.Where(x => x.Id == id).SingleOrDefault();
                    if (trn == null)
                    {
                        throw new Exception(ModuleConstants.ID_NOT_FOUND);
                    }

                    if (ModelState.IsValid)
                    {
                        context.tblTrainings.DeleteOnSubmit(trn);
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
    }
}
