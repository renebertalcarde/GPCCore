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
using System.Collections.Generic;

namespace coreApp.Areas.HRModule.Controllers
{
    [EmployeeInfoFilter]
    [EmployeeAuthorize]
    public class EmployeeChildrenController : HLBase_AuthorizedController, IEmployeeController
    {
        public tblEmployee employee { get; set; }

        public EmployeeChildrenController()
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

                var model = context.tblEmployee_Childrens.Where(x => x.EmployeeId == employee.EmployeeId).OrderByDescending(x => x.Name).ToList();
                return View(model);
            }
        }


        public ActionResult _Details(int? id)
        {
            ViewBag.Title = "Child";
            

            ViewBag.uiIsReadOnly = true;
            ViewBag.uiIncludeId = true;

            using (hr2017DataContext context = new hr2017DataContext())
            {
                tblEmployee_Children Child = context.tblEmployee_Childrens.Where(x => x.EmployeeId == employee.EmployeeId && x.ChildId == id).SingleOrDefault();
                if (Child == null)
                {
                    return new Raw_ActionResult(ModuleConstants.ID_NOT_FOUND);
                }
                else
                {
                    return PartialView("_Child",Child);
                }
            }
        }

        public ActionResult _Create()
        {
            ViewBag.Title = "Child";
            

            ViewBag.uiIsReadOnly = false;
            ViewBag.uiIncludeId = false;

            tblEmployee_Children model = new tblEmployee_Children
            {
                EmployeeId = employee.EmployeeId,
                Name = "",
                DateOfBirth = DateTime.Now
            };

            return PartialView("_Child",model);
        }

        public void _CreatePost(FormCollection _model, ref queryResult res)
        {
            tblEmployee_Children model = (tblEmployee_Children)UpdateModel(new tblEmployee_Children().GetType(), _model);

            using (hr2017DataContext context = new hr2017DataContext())
            {
                if (!Cache.Get().userAccess.HasPermission())
                {
                    AddError(ModuleConstants_Authorization.USERACCESS_UNAUTHORIZED_ACTION);
                }

                if (ModelState.IsValid)
                {             

                    tblEmployee_Children Child = new tblEmployee_Children
                    {
                        EmployeeId = model.EmployeeId,
                        Name = model.Name,
                        DateOfBirth = model.DateOfBirth,
                        Gender = model.Gender
                    };

                    context.tblEmployee_Childrens.InsertOnSubmit(Child);
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
            ViewBag.Title = "Child";
            

            ViewBag.uiIsReadOnly = false;
            ViewBag.uiIncludeId = true;

            using (hr2017DataContext context = new hr2017DataContext())
            {
                tblEmployee_Children Child = context.tblEmployee_Childrens.Where(x => x.EmployeeId == employee.EmployeeId && x.ChildId == id).Single();
                if (Child == null)
                {
                    return new Raw_ActionResult(ModuleConstants.ID_NOT_FOUND);
                }
                else
                {
                    return PartialView("_Child",Child);
                }
            }
        }


        public void _EditPost(FormCollection _model, ref queryResult res)
        {
            tblEmployee_Children model = (tblEmployee_Children)UpdateModel(new tblEmployee_Children().GetType(), _model);

            using (hr2017DataContext context = new hr2017DataContext())
            {
                tblEmployee_Children Child = context.tblEmployee_Childrens.Where(x => x.ChildId == model.ChildId).SingleOrDefault();
                if (Child == null)
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
                        Child.EmployeeId = model.EmployeeId;
                        Child.Name = model.Name;
                        Child.DateOfBirth = model.DateOfBirth;
                        Child.Gender = model.Gender;

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
                    tblEmployee_Children Child = context.tblEmployee_Childrens.Where(x => x.ChildId == id).SingleOrDefault();
                    if (Child == null)
                    {
                        throw new Exception(ModuleConstants.ID_NOT_FOUND);
                    }
                    else
                    {
                        context.tblEmployee_Childrens.DeleteOnSubmit(Child);
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
