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
    public class BulletinBoardDocTypesController : HLBase_AuthorizedController
    {
        public BulletinBoardDocTypesController()
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
            using (dalDataContext context = new dalDataContext())
            {
                var model = context.tblBulletinBoardDocTypes.OrderBy(x => x.Description).ToList();
                return View(model);
            }
        }

        public ActionResult _Details(int? id)
        {
            ViewBag.Title = "Doc Type";
            

            ViewBag.uiIsReadOnly = true;
            ViewBag.uiIncludeId = true;

            using (dalDataContext context = new dalDataContext())
            {
                tblBulletinBoardDocType docType = context.tblBulletinBoardDocTypes.Where(x => x.Id == id).SingleOrDefault();
                if (docType == null)
                {
                    return new Raw_ActionResult(ModuleConstants.ID_NOT_FOUND);
                }
                else
                {
                    return PartialView("_BulletinBoardDocType", docType);
                }
            }
        }

        public ActionResult _Create()
        {
            ViewBag.Title = "Doc Type";
            

            ViewBag.uiIsReadOnly = false;
            ViewBag.uiIncludeId = false;

            return PartialView("_BulletinBoardDocType", new tblBulletinBoardDocType());
        }

        public void _CreatePost(FormCollection _model, ref queryResult res)
        {
            tblBulletinBoardDocType model = (tblBulletinBoardDocType)UpdateModel(new tblBulletinBoardDocType().GetType(), _model);

            using (dalDataContext context = new dalDataContext())
            {
                if (ModelState.IsValid)
                {
                    tblBulletinBoardDocType docType = new tblBulletinBoardDocType
                    {
                        Description = model.Description
                    };

                    context.tblBulletinBoardDocTypes.InsertOnSubmit(docType);
                    context.SubmitChanges();

                    res.Remarks = "Record was successfully created";
                }
                else
                {
                    throw new Exception(coreProcs.ShowErrors(ModelState));
                }
            }
        }

        public ActionResult _Edit(int? id)
        {
            ViewBag.Title = "Doc Type";
            

            ViewBag.uiIsReadOnly = false;
            ViewBag.uiIncludeId = true;

            using (dalDataContext context = new dalDataContext())
            {
                tblBulletinBoardDocType docType = context.tblBulletinBoardDocTypes.Where(x => x.Id == id).Single();
                if (docType == null)
                {
                    return new Raw_ActionResult(ModuleConstants.ID_NOT_FOUND);
                }
                else
                {
                    return PartialView("_BulletinBoardDocType", docType);
                }
            }
        }


        public void _EditPost(FormCollection _model, ref queryResult res)
        {
            tblBulletinBoardDocType model = (tblBulletinBoardDocType)UpdateModel(new tblBulletinBoardDocType().GetType(), _model);

            using (dalDataContext context = new dalDataContext())
            {
                if (ModelState.IsValid)
                {

                    tblBulletinBoardDocType docType = context.tblBulletinBoardDocTypes.Where(x => x.Id == model.Id).SingleOrDefault();
                    if (docType == null)
                    {
                        throw new Exception(ModuleConstants.ID_NOT_FOUND);
                    }
                    else
                    {
                        docType.Description = model.Description;

                        context.SubmitChanges();

                        res.Remarks = "Record was successfully updated";
                        
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

                    tblBulletinBoardDocType docType = context.tblBulletinBoardDocTypes.Where(x => x.Id == id).SingleOrDefault();
                    if (docType == null)
                    {
                        throw new Exception(ModuleConstants.ID_NOT_FOUND);
                    }
                    else
                    {
                        context.tblBulletinBoardDocTypes.DeleteOnSubmit(docType);
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
    }
}
