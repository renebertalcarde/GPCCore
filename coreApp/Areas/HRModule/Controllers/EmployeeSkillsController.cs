using System;
using System.Linq;
using System.Web.Mvc;
using coreApp.Filters;
using coreApp.Controllers;
using coreApp.DAL;
using coreApp.Interfaces;
using coreLib.Objects;
using Module.DB;
using Module.Core;

namespace coreApp.Areas.HRModule.Controllers
{
    [EmployeeInfoFilter]
    [EmployeeAuthorize]
    public class EmployeeSkillsController : HLBase_AuthorizedController, IEmployeeController
    {

        public tblEmployee employee { get; set; }

        public EmployeeSkillsController()
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
            using (hr2017DataContext context = new hr2017DataContext())
            {

                var model = context.tblEmployee_OtherInfo_Skills.Where(x => x.EmployeeId == employee.EmployeeId).OrderByDescending(x => x.Description).ToList();
                return View(model);
            }
        }


        public ActionResult _Details(int? id)
        {

            ViewBag.uiIsReadOnly = true;
            ViewBag.uiIncludeId = true;

            using (hr2017DataContext context = new hr2017DataContext())
            {
                tblEmployee_OtherInfo_Skill item = context.tblEmployee_OtherInfo_Skills.Where(x => x.EmployeeId == employee.EmployeeId && x.Id == id).SingleOrDefault();
                if (item == null)
                {
                    return new Raw_ActionResult(ModuleConstants.ID_NOT_FOUND);
                }
                else
                {
                    return PartialView("_OtherInfo", item);
                }
            }
        }

        public ActionResult _Create()
        {

            ViewBag.uiIsReadOnly = false;
            ViewBag.uiIncludeId = false;

            tblEmployee_OtherInfo_Skill model = new tblEmployee_OtherInfo_Skill
            {
                EmployeeId = employee.EmployeeId,
                Description = ""
            };

            return PartialView("_OtherInfo", model);
        }

        public void _CreatePost(FormCollection _model, ref queryResult res)
        {
            tblEmployee_OtherInfo_Skill model = (tblEmployee_OtherInfo_Skill)UpdateModel(new tblEmployee_OtherInfo_Skill().GetType(), _model);

            using (hr2017DataContext context = new hr2017DataContext())
            {
                if (!Cache.Get().userAccess.HasPermission())
                {
                    AddError(ModuleConstants_Authorization.USERACCESS_UNAUTHORIZED_ACTION);
                }

                if (ModelState.IsValid)
                {
                    tblEmployee_OtherInfo_Skill Info = new tblEmployee_OtherInfo_Skill
                    {
                        EmployeeId = model.EmployeeId,
                        Description = model.Description
                    };

                    context.tblEmployee_OtherInfo_Skills.InsertOnSubmit(Info);
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

            ViewBag.uiIsReadOnly = false;
            ViewBag.uiIncludeId = true;

            using (hr2017DataContext context = new hr2017DataContext())
            {
                tblEmployee_OtherInfo_Skill item = context.tblEmployee_OtherInfo_Skills.Where(x => x.EmployeeId == employee.EmployeeId && x.Id == id).Single();
                if (item == null)
                {
                    return new Raw_ActionResult(ModuleConstants.ID_NOT_FOUND);
                }
                else
                {
                    return PartialView("_OtherInfo", item);
                }
            }
        }


        public void _EditPost(FormCollection _model, ref queryResult res)
        {
            tblEmployee_OtherInfo_Skill model = (tblEmployee_OtherInfo_Skill)UpdateModel(new tblEmployee_OtherInfo_Skill().GetType(), _model);

            using (hr2017DataContext context = new hr2017DataContext())
            {
                tblEmployee_OtherInfo_Skill Info = context.tblEmployee_OtherInfo_Skills.Where(x => x.Id == model.Id).SingleOrDefault();
                if (Info == null)
                {
                    throw new Exception(ModuleConstants.ID_NOT_FOUND);
                }
                else
                {
                    if (!Cache.Get().userAccess.HasPermission())
                    {
                        AddError(ModuleConstants_Authorization.USERACCESS_UNAUTHORIZED_ACTION);
                    }

                    if (ModelState.IsValid)
                    {
                        Info.EmployeeId = model.EmployeeId;
                        Info.Description = model.Description;

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


        public void _Delete(int id, ref queryResult res)
        {
            using (hr2017DataContext context = new hr2017DataContext())
            {
                if (!Cache.Get().userAccess.HasPermission())
                {
                    AddError(ModuleConstants_Authorization.USERACCESS_UNAUTHORIZED_ACTION);
                }

                if (ModelState.IsValid)
                {
                    tblEmployee_OtherInfo_Skill Info = context.tblEmployee_OtherInfo_Skills.Where(x => x.Id == id).SingleOrDefault();
                    if (Info == null)
                    {
                        throw new Exception(ModuleConstants.ID_NOT_FOUND);
                    }
                    else
                    {
                        context.tblEmployee_OtherInfo_Skills.DeleteOnSubmit(Info);
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
