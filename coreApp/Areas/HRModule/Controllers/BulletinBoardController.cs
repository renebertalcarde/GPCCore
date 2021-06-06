using System.Linq;
using System.Web.Mvc;
using System;
using coreApp.Controllers;
using coreLib.Objects;
using Module.Core;
using Module.DB;

namespace coreApp.Areas.HRModule.Controllers
{
    [UserAccessAuthorize(allowedAccess: "hr_bulletinboard")]
    public class BulletinBoardController : HLBase_NoCoreAuthorizedController
    {
        public ActionResult Index()
        {
            using (dalDataContext context = new dalDataContext())
            {
                var model = context.tblBulletinBoards.OrderByDescending(x => x.DateOfPosting).ThenBy(x => x.Title).ToList();
                return View(model);
            }
        }

        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new Raw_ActionResult(ModuleConstants.BAD_REQUEST);
            }

            ViewBag.Title = "Bulletin Board";
            

            ViewBag.uiIsReadOnly = true;
            ViewBag.uiIncludeId = true;

            using (dalDataContext context = new dalDataContext())
            {
                tblBulletinBoard bb = context.tblBulletinBoards.Where(x => x.Id == id).SingleOrDefault();
                if (bb == null)
                {
                    return new Raw_ActionResult(ModuleConstants.ID_NOT_FOUND);
                }
                else
                {
                    return View(bb);
                }
            }
        }

        public ActionResult Create()
        {
            ViewBag.Title = "Bulletin Board";
            

            ViewBag.uiIsReadOnly = false;
            ViewBag.uiIncludeId = false;

            DateTime n = DateTime.Now;

            return View("Details", new tblBulletinBoard
            {
                Id = -1,
                DateCreated = n,
                DateOfPosting = n.AddMinutes(1),
                EndOfPosting = n.AddMonths(1),
                DocType = -1,
                ShowInKiosk = false,
                ShowInDashboard = true
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Create(tblBulletinBoard model)
        {
            queryResult res = new queryResult { IsSuccessful = true, Data = null, Err = "", Remarks = "" };

            try
            {
                using (dalDataContext context = new dalDataContext())
                {
                    

                    if (model.DateOfPosting < model.DateCreated)
                    {
                        AddError("Date of Posting cannot be earlier than the date of creation");
                    }

                    if (ModelState.IsValid)
                    {
                        tblBulletinBoard bb = new tblBulletinBoard
                        {
                            DateCreated = DateTime.Now,
                            DateOfPosting = model.DateOfPosting,
                            Title = model.Title,
                            Contents = model.Contents,
                            DocType = model.DocType,
                            CreatedBy_UserId = Cache.Get().userAccess.user.Id,
                            Enabled = model.Enabled,
                            EndOfPosting = model.EndOfPosting,
                            ShowInKiosk = model.ShowInKiosk,
                            ShowInDashboard = model.ShowInDashboard                            
                        };

                        context.tblBulletinBoards.InsertOnSubmit(bb);
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

            ViewBag.Title = "Bulletin Board";
            

            ViewBag.uiIsReadOnly = false;
            ViewBag.uiIncludeId = true;

            using (dalDataContext context = new dalDataContext())
            {
                tblBulletinBoard bb = context.tblBulletinBoards.Where(x => x.Id == id).Single();
                if (bb == null)
                {
                    return new Raw_ActionResult(ModuleConstants.ID_NOT_FOUND);
                }
                else
                {
                    return View("Details", bb);
                }
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Edit(tblBulletinBoard model)
        {
            queryResult res = new queryResult { IsSuccessful = true, Data = null, Err = "", Remarks = "" };

            try
            {
                using (dalDataContext context = new dalDataContext())
                {
                    tblBulletinBoard bb = context.tblBulletinBoards.Where(x => x.Id == model.Id).SingleOrDefault();
                    if (bb == null)
                    {
                        throw new Exception(ModuleConstants.ID_NOT_FOUND);
                    }

                    if (model.DateOfPosting < bb.DateCreated)
                    {
                        AddError("Date of Posting cannot be earlier than the date of creation");
                    }

                    if (ModelState.IsValid)
                    {

                        bb.DateOfPosting = model.DateOfPosting;
                        bb.EndOfPosting = model.EndOfPosting;
                        bb.Title = model.Title;
                        bb.Contents = model.Contents;
                        bb.DocType = model.DocType;
                        bb.Enabled = model.Enabled;
                        bb.ShowInKiosk = model.ShowInKiosk;
                        bb.ShowInDashboard = model.ShowInDashboard;
                        bb.LastUpdatedBy_UserId = Cache.Get().userAccess.user.Id;
                        bb.LastUpdated = DateTime.Now;

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
                    if (ModelState.IsValid)
                    {

                        tblBulletinBoard bb = context.tblBulletinBoards.Where(x => x.Id == id).SingleOrDefault();
                        if (bb == null)
                        {
                            throw new Exception(ModuleConstants.ID_NOT_FOUND);
                        }
                        else
                        {
                            context.tblBulletinBoards.DeleteOnSubmit(bb);
                            context.SubmitChanges();

                            res.Remarks = "Record was successfully deleted";
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
