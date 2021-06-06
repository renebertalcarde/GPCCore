using System.Linq;
using System.Net;
using System.Web.Mvc;
using System;
using coreApp.Controllers;
using coreApp;
using coreLib.Objects;
using Module.DB;
using Module.Core;

namespace coreApp.Areas.HRModule.Controllers
{
    [UserAccessAuthorize(allowedAccess: "hr_config")]
    public class MFOsController : HLBase_AuthorizedController
    {
        public MFOsController()
        {
            IndexProc = new IndexDelegate(_Index);
            DetailsProc = new DetailsDelegate(_Details);
            CreateProc = new CreateDelegate(_Create);
            CreatePostProc = new CreatePostDelegate(_CreatePost);
            EditProc = new EditDelegate(_Edit);
            EditPostProc = new EditPostDelegate(_EditPost);
            DeletePostProc = new DeletePostDelegate(_Delete);
        }

        public ActionResult _Index()
        {
            var model = Cache.Get_Tables().MFOs.OrderBy(x => x.Description).ToList();
            return View(model);
        }

        public ActionResult _Details(int? id)
        {
            ViewBag.Title = "MFO";
            

            ViewBag.uiIsReadOnly = true;
            ViewBag.uiIncludeId = true;


            tblMFO mfo = Cache.Get_Tables().MFOs.Where(x => x.Id == id).SingleOrDefault();
            if (mfo == null)
            {
                return new Raw_ActionResult(ModuleConstants.ID_NOT_FOUND);
            }
            else
            {
                return PartialView("_MFO", mfo);
            }
        }

        public ActionResult _Create()
        {
            ViewBag.Title = "MFO";
            

            ViewBag.uiIsReadOnly = false;
            ViewBag.uiIncludeId = false;

            return PartialView("_MFO", new tblMFO());
        }

        public void _CreatePost(FormCollection _model, ref queryResult res)
        {
            tblMFO model = (tblMFO)UpdateModel(new tblMFO().GetType(), _model);

            using (dalDataContext context = new dalDataContext())
            {
                if (ModelState.IsValid)
                {
                    tblMFO mfo = new tblMFO
                    {
                        Description = model.Description
                    };

                    context.tblMFOs.InsertOnSubmit(mfo);
                    context.SubmitChanges();

                    //MFOSettings.setMFOSettings(mfo.MFOId);

                    res.Remarks = "Record was successfully created";

                    Cache.Update_Tables(_mfos: true);
                }
                else
                {
                    throw new Exception(coreProcs.ShowErrors(ModelState));
                }
            }
        }

        public ActionResult _Edit(int? id)
        {
            ViewBag.Title = "MFO";
            

            ViewBag.uiIsReadOnly = false;
            ViewBag.uiIncludeId = true;


            tblMFO mfo = Cache.Get_Tables().MFOs.Where(x => x.Id == id).Single();
            if (mfo == null)
            {
                return new Raw_ActionResult(ModuleConstants.ID_NOT_FOUND);
            }
            else
            {
                return PartialView("_MFO", mfo);
            }
        }


        public void _EditPost(FormCollection _model, ref queryResult res)
        {
            tblMFO model = (tblMFO)UpdateModel(new tblMFO().GetType(), _model);

            using (dalDataContext context = new dalDataContext())
            {
                if (ModelState.IsValid)
                {

                    tblMFO mfo = context.tblMFOs.Where(x => x.Id == model.Id).SingleOrDefault();
                    if (mfo == null)
                    {
                        throw new Exception(ModuleConstants.ID_NOT_FOUND);
                    }
                    else
                    {
                        mfo.Description = model.Description;

                        context.SubmitChanges();

                        //MFOSettings.setMFOSettings(mfo.MFOId);

                        res.Remarks = "Record was successfully updated";

                        Cache.Update_Tables(_mfos: true);
                    }

                }
                else
                {
                    throw new Exception(coreProcs.ShowErrors(ModelState));
                }
            }
        }

        public void _Delete(int id, ref queryResult res)
        {
            using (dalDataContext context = new dalDataContext())
            {
                if (ModelState.IsValid)
                {

                    tblMFO mfo = context.tblMFOs.Where(x => x.Id == id).SingleOrDefault();
                    if (mfo == null)
                    {
                        throw new Exception(ModuleConstants.ID_NOT_FOUND);
                    }
                    else
                    {
                        context.tblMFOs.DeleteOnSubmit(mfo);
                        context.SubmitChanges();

                        res.Remarks = "Record was successfully deleted";

                        Cache.Update_Tables(_mfos: true);
                    }

                }
                else
                {
                    throw new Exception(coreProcs.ShowErrors(ModelState));
                }
            }
        }
    }
}
