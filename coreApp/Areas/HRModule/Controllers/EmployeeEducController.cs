using System;
using System.Linq;
using System.Web.Mvc;
using coreApp.Filters;
using coreApp.DAL;
using coreApp.Controllers;
using coreApp.Interfaces;
using coreLib.Objects;
using Module.DB;
using Module.Core;
using System.Collections.Generic;

namespace coreApp.Areas.HRModule.Controllers
{
    [EmployeeInfoFilter]
    [EmployeeAuthorize]
    public class EmployeeEducController : HLBase_AuthorizedController, IEmployeeController
    {
        public tblEmployee employee { get; set; }

        public EmployeeEducController()
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

                var model = context.tblEmployee_Educs.Where(x => x.EmployeeId == employee.EmployeeId).OrderBy(x => x.YearFrom).ToList();
                return View(model);
            }
        }

        public ActionResult _Details(int? id)
        {
            
            ViewBag.Title = "Education";
            

            ViewBag.uiIsReadOnly = true;
            ViewBag.uiIncludeId = true;

            using (hr2017DataContext context = new hr2017DataContext())
            {
                tblEmployee_Educ Educ = context.tblEmployee_Educs.Where(x => x.EmployeeId == employee.EmployeeId && x.EducId == id).SingleOrDefault();
                if (Educ == null)
                {
                    return new Raw_ActionResult(ModuleConstants.ID_NOT_FOUND);
                }
                else
                {
                    return PartialView("_Educ", Educ);
                }
            }
        }

        public ActionResult _Create()
        {
            
            ViewBag.Title = "Education";
            

            ViewBag.uiIsReadOnly = false;
            ViewBag.uiIncludeId = false;

            tblEmployee_Educ model = new tblEmployee_Educ
            {
                EmployeeId = employee.EmployeeId
            };

            return PartialView("_Educ", model);
        }

        public void _CreatePost(FormCollection _model, ref queryResult res)
        {
            tblEmployee_Educ model = (tblEmployee_Educ)UpdateModel(new tblEmployee_Educ().GetType(), _model);

            using (hr2017DataContext context = new hr2017DataContext())
            {
                if (!Cache.Get().userAccess.HasPermission())
                {
                    AddError(ModuleConstants_Authorization.USERACCESS_UNAUTHORIZED_ACTION);
                }

                if (model.YearFrom == -1) model.YearFrom = null;

                if (model.YearTo == -1)
                {

                }
                else if (model.YearTo == -2)
                {
                    ModelState.Remove("YearTo");
                }
                else if (model.YearFrom > model.YearTo)
                {
                    AddError("From year cannot be later than To year");
                }

                if (ModelState.IsValid)
                {

                    tblEmployee_Educ Educ = new tblEmployee_Educ
                    {
                        EmployeeId = model.EmployeeId,
                        EducLevel = model.EducLevel,
                        School = model.School,
                        Degree = model.Degree,
                        YearGraduated = model.YearGraduated,
                        UnitsEarned = model.UnitsEarned,
                        YearFrom = model.YearFrom,
                        YearTo = model.YearTo == -2 ? null : model.YearTo,
                        YearTo_Present = model.YearTo == -2,
                        HonorsReceived = model.HonorsReceived
                    };

                    context.tblEmployee_Educs.InsertOnSubmit(Educ);
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
            
            ViewBag.Title = "Education";
            

            ViewBag.uiIsReadOnly = false;
            ViewBag.uiIncludeId = true;

            using (hr2017DataContext context = new hr2017DataContext())
            {
                tblEmployee_Educ Educ = context.tblEmployee_Educs.Where(x => x.EmployeeId == employee.EmployeeId && x.EducId == id).Single();
                if (Educ == null)
                {
                    return new Raw_ActionResult(ModuleConstants.ID_NOT_FOUND);
                }

                if (Educ.YearTo_Present)
                {
                    Educ.YearTo = -2;
                }

                return PartialView("_Educ", Educ);
            }
        }


        public void _EditPost(FormCollection _model, ref queryResult res)
        {
            tblEmployee_Educ model = (tblEmployee_Educ)UpdateModel(new tblEmployee_Educ().GetType(), _model);

            using (hr2017DataContext context = new hr2017DataContext())
            {
                tblEmployee_Educ Educ = context.tblEmployee_Educs.Where(x => x.EducId == model.EducId).SingleOrDefault();
                if (Educ == null)
                {
                    throw new Exception(ModuleConstants.ID_NOT_FOUND);
                }
                else
                {
                    if (!Cache.Get().userAccess.HasPermission())
                    {
                        AddError(ModuleConstants_Authorization.USERACCESS_UNAUTHORIZED_ACTION);
                    }

                    if (model.YearFrom == -1) model.YearFrom = null;

                    if (model.YearTo == -1)
                    {

                    }
                    else if (model.YearTo == -2)
                    {
                        ModelState.Remove("YearTo");
                    }
                    else if (model.YearFrom > model.YearTo)
                    {
                        AddError("From year cannot be later than To year");
                    }

                    if (ModelState.IsValid)
                    {
                        Educ.EmployeeId = model.EmployeeId;
                        Educ.EducLevel = model.EducLevel;
                        Educ.School = model.School;
                        Educ.Degree = model.Degree;
                        Educ.YearGraduated = model.YearGraduated;
                        Educ.UnitsEarned = model.UnitsEarned;
                        Educ.YearFrom = model.YearFrom;
                        Educ.YearTo = model.YearTo == -2 ? null : model.YearTo;
                        Educ.YearTo_Present = model.YearTo == -2;
                        Educ.HonorsReceived = model.HonorsReceived;

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

                    tblEmployee_Educ Educ = context.tblEmployee_Educs.Where(x => x.EducId == id).SingleOrDefault();
                    if (Educ == null)
                    {
                        throw new Exception(ModuleConstants.ID_NOT_FOUND);
                    }
                    else
                    {
                        context.tblEmployee_Educs.DeleteOnSubmit(Educ);
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
