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
    public class EmployeeVoluntaryController : HLBase_AuthorizedController, IEmployeeController
    {

        public tblEmployee employee { get; set; }

        public EmployeeVoluntaryController()
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

                var model = context.tblEmployee_Voluntaries.Where(x => x.EmployeeId == employee.EmployeeId).OrderByDescending(x => x.DateFrom).ToList();
                return View(model);
            }
        }

        public ActionResult _Details(int? id)
        {
            ViewBag.Title = "Voluntary Works";
            

            ViewBag.uiIsReadOnly = true;
            ViewBag.uiIncludeId = true;

            using (hr2017DataContext context = new hr2017DataContext())
            {
                tblEmployee_Voluntary Voluntary = context.tblEmployee_Voluntaries.Where(x => x.EmployeeId == employee.EmployeeId && x.Id == id).SingleOrDefault();
                if (Voluntary == null)
                {
                    return new Raw_ActionResult(ModuleConstants.ID_NOT_FOUND);
                }
                else
                {
                    return PartialView("_Voluntary",Voluntary);
                }
            }
        }

        public ActionResult _Create()
        {
            ViewBag.Title = "Voluntary Works";
            

            ViewBag.uiIsReadOnly = false;
            ViewBag.uiIncludeId = false;

            tblEmployee_Voluntary model = new tblEmployee_Voluntary
            {
                EmployeeId = employee.EmployeeId
            };

            return PartialView("_Voluntary",model);
        }

        public void _CreatePost(FormCollection _model, ref queryResult res)
        {
            tblEmployee_Voluntary model = (tblEmployee_Voluntary)UpdateModel(new tblEmployee_Voluntary().GetType(), _model);
            

            using (hr2017DataContext context = new hr2017DataContext())
            {
                if (!Cache.Get().userAccess.HasPermission())
                {
                    AddError(ModuleConstants_Authorization.USERACCESS_UNAUTHORIZED_ACTION);
                }

                if (model.DateTo == null)
                {
                    if (model.DateTo_Present != true)
                    {
                        AddError("The To field is required.");
                    }
                }
                else if (model.DateFrom > model.DateTo)
                {
                    AddError("From date cannot be later than To date");
                }
                

                if (ModelState.IsValid)
                {

                    tblEmployee_Voluntary Voluntary = new tblEmployee_Voluntary
                    {
                        EmployeeId = model.EmployeeId,
                        DateFrom = model.DateFrom,
                        DateTo = model.DateTo_Present == true ? null : model.DateTo,
                        DateTo_Present = model.DateTo_Present,
                        Organization = model.Organization,
                        Address = model.Address,
                        NoOfHours = model.NoOfHours,
                        PositionNatureOfWork = model.PositionNatureOfWork
                    };

                    context.tblEmployee_Voluntaries.InsertOnSubmit(Voluntary);
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
            ViewBag.Title = "Voluntary Works";
            

            ViewBag.uiIsReadOnly = false;
            ViewBag.uiIncludeId = true;

            using (hr2017DataContext context = new hr2017DataContext())
            {
                tblEmployee_Voluntary Voluntary = context.tblEmployee_Voluntaries.Where(x => x.EmployeeId == employee.EmployeeId && x.Id == id).Single();
                if (Voluntary == null)
                {
                    return new Raw_ActionResult(ModuleConstants.ID_NOT_FOUND);
                }
                else
                {
                    return PartialView("_Voluntary",Voluntary);
                }
            }
        }


        public void _EditPost(FormCollection _model, ref queryResult res)
        {
            tblEmployee_Voluntary model = (tblEmployee_Voluntary)UpdateModel(new tblEmployee_Voluntary().GetType(), _model);
            
            using (hr2017DataContext context = new hr2017DataContext())
            {
                tblEmployee_Voluntary Voluntary = context.tblEmployee_Voluntaries.Where(x => x.Id == model.Id).SingleOrDefault();
                if (Voluntary == null)
                {
                    throw new Exception(ModuleConstants.ID_NOT_FOUND);
                }
                else
                {
                    if (!Cache.Get().userAccess.HasPermission())
                    {
                        AddError(ModuleConstants_Authorization.USERACCESS_UNAUTHORIZED_ACTION);
                    }

                    if (model.DateTo == null)
                    {
                        if (model.DateTo_Present != true)
                        {
                            AddError("The To field is required.");
                        }
                    }
                    else if (model.DateFrom > model.DateTo)
                    {
                        AddError("From date cannot be later than To date");
                    }

                    if (ModelState.IsValid)
                    {

                        Voluntary.DateFrom = model.DateFrom;
                        Voluntary.DateTo = model.DateTo_Present == true ? null : model.DateTo;
                        Voluntary.DateTo_Present = model.DateTo_Present;
                        Voluntary.Organization = model.Organization;
                        Voluntary.Address = model.Address;
                        Voluntary.NoOfHours = model.NoOfHours;
                        Voluntary.PositionNatureOfWork = model.PositionNatureOfWork;

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

                    tblEmployee_Voluntary Voluntary = context.tblEmployee_Voluntaries.Where(x => x.Id == id).SingleOrDefault();
                    if (Voluntary == null)
                    {
                        throw new Exception(ModuleConstants.ID_NOT_FOUND);
                    }
                    else
                    {
                        context.tblEmployee_Voluntaries.DeleteOnSubmit(Voluntary);
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
