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
    public class EmployeeWorkExpsController : HLBase_AuthorizedController, IEmployeeController
    {

        public tblEmployee employee { get; set; }

        public EmployeeWorkExpsController()
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

                var model = context.tblEmployee_WorkExps.Where(x => x.EmployeeId == employee.EmployeeId).OrderByDescending(x => x.DateFrom).ToList();
                return View(model);
            }
        }

        public ActionResult _Details(int? id)
        {
            ViewBag.Title = "Work Experience";
            

            ViewBag.uiIsReadOnly = true;
            ViewBag.uiIncludeId = true;

            using (hr2017DataContext context = new hr2017DataContext())
            {
                tblEmployee_WorkExp WorkExp = context.tblEmployee_WorkExps.Where(x => x.EmployeeId == employee.EmployeeId && x.WEId == id).SingleOrDefault();
                if (WorkExp == null)
                {
                    return new Raw_ActionResult(ModuleConstants.ID_NOT_FOUND);
                }
                else
                {
                    return PartialView("_WorkExp",WorkExp);
                }
            }
        }

        public ActionResult _Create()
        {
            ViewBag.Title = "Work Experience";
            

            ViewBag.uiIsReadOnly = false;
            ViewBag.uiIncludeId = false;

            tblEmployee_WorkExp model = new tblEmployee_WorkExp
            {
                EmployeeId = employee.EmployeeId
            };

            return PartialView("_WorkExp",model);
        }

        public void _CreatePost(FormCollection _model, ref queryResult res)
        {
            tblEmployee_WorkExp model = (tblEmployee_WorkExp)UpdateModel(new tblEmployee_WorkExp().GetType(), _model);
            

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

                    tblEmployee_WorkExp WorkExp = new tblEmployee_WorkExp
                    {
                        EmployeeId = model.EmployeeId,
                        DateFrom = model.DateFrom,
                        DateTo = model.DateTo_Present == true ? null : model.DateTo,
                        DateTo_Present = model.DateTo_Present,
                        Position = model.Position,
                        Company = model.Company,
                        Salary = model.Salary,
                        SalaryGrade = model.SalaryGrade,
                        StatusOfAppointment = model.StatusOfAppointment,
                        IsGovService = model.IsGovService
                    };

                    context.tblEmployee_WorkExps.InsertOnSubmit(WorkExp);
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
            ViewBag.Title = "Work Experience";
            

            ViewBag.uiIsReadOnly = false;
            ViewBag.uiIncludeId = true;

            using (hr2017DataContext context = new hr2017DataContext())
            {
                tblEmployee_WorkExp WorkExp = context.tblEmployee_WorkExps.Where(x => x.EmployeeId == employee.EmployeeId && x.WEId == id).Single();
                if (WorkExp == null)
                {
                    return new Raw_ActionResult(ModuleConstants.ID_NOT_FOUND);
                }
                else
                {
                    return PartialView("_WorkExp",WorkExp);
                }
            }
        }


        public void _EditPost(FormCollection _model, ref queryResult res)
        {
            tblEmployee_WorkExp model = (tblEmployee_WorkExp)UpdateModel(new tblEmployee_WorkExp().GetType(), _model);
            
            using (hr2017DataContext context = new hr2017DataContext())
            {
                tblEmployee_WorkExp WorkExp = context.tblEmployee_WorkExps.Where(x => x.WEId == model.WEId).SingleOrDefault();
                if (WorkExp == null)
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
                        WorkExp.DateFrom = model.DateFrom;
                        WorkExp.DateTo = model.DateTo_Present == true ? null : model.DateTo;
                        WorkExp.DateTo_Present = model.DateTo_Present;
                        WorkExp.Position = model.Position;
                        WorkExp.Company = model.Company;
                        WorkExp.Salary = model.Salary;
                        WorkExp.SalaryGrade = model.SalaryGrade;
                        WorkExp.StatusOfAppointment = model.StatusOfAppointment;
                        WorkExp.IsGovService = model.IsGovService;

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

                    tblEmployee_WorkExp WorkExp = context.tblEmployee_WorkExps.Where(x => x.WEId == id).SingleOrDefault();
                    if (WorkExp == null)
                    {
                        throw new Exception(ModuleConstants.ID_NOT_FOUND);
                    }
                    else
                    {
                        context.tblEmployee_WorkExps.DeleteOnSubmit(WorkExp);
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
