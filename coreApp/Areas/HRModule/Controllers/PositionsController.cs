using System.Linq;
using System.Web.Mvc;
using System;
using coreApp.DAL;

using coreApp.Controllers;
using coreLib.Objects;
using Module.DB;
using Module.Core;
using System.Data.Linq;

namespace coreApp.Areas.HRModule.Controllers
{
    [UserAccessAuthorize(allowedAccess: "hr_config")]
    public class PositionsController : HLBase_AuthorizedController
    {
        public PositionsController()
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
            var model = Cache.Get_Tables().Positions.OrderBy(x => x.Position).ToList();

            return View(model);
        }

        public ActionResult _Details(int? id)
        {
            ViewBag.uiIsReadOnly = true;
            ViewBag.uiIncludeId = true;

            tblPosition Position = Cache.Get_Tables().Positions.Where(x => x.PositionId == id).SingleOrDefault();
            if (Position == null)
            {
                return new Raw_ActionResult(ModuleConstants.ID_NOT_FOUND);
            }
            else
            {
                return PartialView("_Position", Position);
            }
        }

        public ActionResult _Create()
        {           

            ViewBag.uiIsReadOnly = false;
            ViewBag.uiIncludeId = false;

            return PartialView("_Position", new tblPosition());
        }

        public void _CreatePost(FormCollection _model, ref queryResult res)
        {
            tblPosition model = (tblPosition)UpdateModel(new tblPosition().GetType(), _model);
            

            using (dalDataContext context = new dalDataContext())
            {
                if (ModelState.IsValid)
                {

                    tblPosition Position = new tblPosition
                    {
                        Code = model.Code,
                        Position = model.Position,
                        Confidential = model.Confidential,
                        IsFaculty = model.IsFaculty,
                        RankAndFile = model.RankAndFile
                    };

                    context.tblPositions.InsertOnSubmit(Position);
                    context.SubmitChanges();

                    res.Remarks = "Record was successfully created";

                    Cache.Update_Tables(_positions: true);

                    MvcApplication.HLLog.InsertLog("Job Titles", Position.PositionId, Position.Position);
                }
                else
                {
                    throw new Exception(coreProcs.ShowErrors(ModelState));
                }
            }
        }

        public ActionResult _Edit(int? id)
        {
            ViewBag.Title = "Position";
            

            ViewBag.uiIsReadOnly = false;
            ViewBag.uiIncludeId = true;

            tblPosition Position = Cache.Get_Tables().Positions.Where(x => x.PositionId == id).Single();
            if (Position == null)
            {
                return new Raw_ActionResult(ModuleConstants.ID_NOT_FOUND);
            }
            else
            {
                return PartialView("_Position", Position);
            }
        }


        public void _EditPost(FormCollection _model, ref queryResult res)
        {
            tblPosition model = (tblPosition)UpdateModel(new tblPosition().GetType(), _model);
            
            using (dalDataContext context = new dalDataContext())
            {
                if (ModelState.IsValid)
                {

                    tblPosition Position = context.tblPositions.Where(x => x.PositionId == model.PositionId).SingleOrDefault();
                    if (Position == null)
                    {
                        throw new Exception(ModuleConstants.ID_NOT_FOUND);
                    }
                    else
                    {
                        Position.Code = model.Code;
                        Position.Position = model.Position;
                        Position.Confidential = model.Confidential;
                        Position.IsFaculty = model.IsFaculty;
                        Position.RankAndFile = model.RankAndFile;

                        ModifiedMemberInfo[] mmi = context.tblPositions.GetModifiedMembers(Position);
                        context.SubmitChanges();

                        res.Remarks = "Record was successfully updated";

                        Cache.Update_Tables(_positions: true);

                        MvcApplication.HLLog.UpdateLog("Job Titles", Position.PositionId, Position.Position, mmi);
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

                    tblPosition Position = context.tblPositions.Where(x => x.PositionId == id).SingleOrDefault();
                    if (Position == null)
                    {
                        throw new Exception(ModuleConstants.ID_NOT_FOUND);
                    }
                    else
                    {
                        context.tblPositions.DeleteOnSubmit(Position);
                        context.SubmitChanges();

                        res.Remarks = "Record was successfully deleted";

                        Cache.Update_Tables(_positions: true);

                        MvcApplication.HLLog.DeleteLog("Job Titles", Position.PositionId, Position.Position);
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
