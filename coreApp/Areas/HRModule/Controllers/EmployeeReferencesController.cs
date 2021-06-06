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
    public class EmployeeReferencesController : HLBase_AuthorizedController, IEmployeeController
    {
        public tblEmployee employee { get; set; }

        public EmployeeReferencesController()
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

                var model = context.tblEmployee_References.Where(x => x.EmployeeId == employee.EmployeeId).OrderBy(x => x.Name).ToList();
                return View(model);
            }
        }


        public ActionResult _Details(int? id)
        {
            ViewBag.Title = "Reference";
            

            ViewBag.uiIsReadOnly = true;
            ViewBag.uiIncludeId = true;

            using (hr2017DataContext context = new hr2017DataContext())
            {
                tblEmployee_Reference Reference = context.tblEmployee_References.Where(x => x.EmployeeId == employee.EmployeeId && x.Id == id).SingleOrDefault();
                if (Reference == null)
                {
                    return new Raw_ActionResult(ModuleConstants.ID_NOT_FOUND);
                }
                else
                {
                    return PartialView("_Reference",Reference);
                }
            }
        }

        public ActionResult _Create()
        {
            ViewBag.Title = "Reference";
            

            ViewBag.uiIsReadOnly = false;
            ViewBag.uiIncludeId = false;

            tblEmployee_Reference model = new tblEmployee_Reference
            {
                EmployeeId = employee.EmployeeId,
                Name = "",
                Address = "",
                TelephoneNo = ""
            };

            return PartialView("_Reference",model);
        }

        public void _CreatePost(FormCollection _model, ref queryResult res)
        {
            tblEmployee_Reference model = (tblEmployee_Reference)UpdateModel(new tblEmployee_Reference().GetType(), _model);

            using (hr2017DataContext context = new hr2017DataContext())
            {
                if (!Cache.Get().userAccess.HasPermission())
                {
                    AddError(ModuleConstants_Authorization.USERACCESS_UNAUTHORIZED_ACTION);
                }

                if (context.tblEmployee_References.Where(x => x.EmployeeId == employee.EmployeeId).Count() >= 3)
                {
                    AddError("Maximum no. of references reached");
                }

                if (ModelState.IsValid)
                {

                    tblEmployee_Reference Reference = new tblEmployee_Reference
                    {
                        EmployeeId = model.EmployeeId,
                        Name = model.Name,
                        Address = model.Address,
                        TelephoneNo = model.TelephoneNo
                    };

                    context.tblEmployee_References.InsertOnSubmit(Reference);
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
            ViewBag.Title = "Reference";
            

            ViewBag.uiIsReadOnly = false;
            ViewBag.uiIncludeId = true;

            using (hr2017DataContext context = new hr2017DataContext())
            {
                tblEmployee_Reference Reference = context.tblEmployee_References.Where(x => x.EmployeeId == employee.EmployeeId && x.Id == id).Single();
                if (Reference == null)
                {
                    return new Raw_ActionResult(ModuleConstants.ID_NOT_FOUND);
                }
                else
                {
                    return PartialView("_Reference",Reference);
                }
            }
        }


        public void _EditPost(FormCollection _model, ref queryResult res)
        {
            tblEmployee_Reference model = (tblEmployee_Reference)UpdateModel(new tblEmployee_Reference().GetType(), _model);

            using (hr2017DataContext context = new hr2017DataContext())
            {
                tblEmployee_Reference Reference = context.tblEmployee_References.Where(x => x.Id == model.Id).SingleOrDefault();
                if (Reference == null)
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
                        Reference.EmployeeId = model.EmployeeId;
                        Reference.Name = model.Name;
                        Reference.Address = model.Address;
                        Reference.TelephoneNo = model.TelephoneNo;

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
                    tblEmployee_Reference Reference = context.tblEmployee_References.Where(x => x.Id == id).SingleOrDefault();
                    if (Reference == null)
                    {
                        throw new Exception(ModuleConstants.ID_NOT_FOUND);
                    }
                    else
                    {
                        context.tblEmployee_References.DeleteOnSubmit(Reference);
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
