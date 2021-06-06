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
using coreLib.Enums;
using coreApp.Areas.TimeModule.Controllers;

namespace coreApp.Areas.HRModule.Controllers
{
    [EmployeeInfoFilter]
    [EmployeeAuthorize(true)]
    [UserAccessAuthorize(allowedAccess: "hr_module,hr_emp_sched")]
    public class EmployeeTravelsController : HLBase_AuthorizedController, IEmployeeController
    {
        public tblEmployee employee { get; set; }

        public EmployeeTravelsController()
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

                var model = context.tblEmployee_Travels.Where(x => x.EmployeeId == employee.EmployeeId).OrderByDescending(x => x.StartDate).ToList();
                return View(model);
            }
        }

        public ActionResult _Details(int? id)
        {
            ViewBag.Title = "Travel";
            

            ViewBag.uiIsReadOnly = true;
            ViewBag.uiIncludeId = true;

            using (dalDataContext context = new dalDataContext())
            {
                tblEmployee_Travel Travel = context.tblEmployee_Travels.Where(x => x.Id == id).SingleOrDefault();
                if (Travel == null)
                {
                    return new Raw_ActionResult(ModuleConstants.ID_NOT_FOUND);
                }
                else
                {
                    return PartialView("_Travel",Travel);
                }
            }
        }

        public ActionResult _Create()
        {
            ViewBag.Title = "Travel";
            

            ViewBag.uiIsReadOnly = false;
            ViewBag.uiIncludeId = false;
            
            tblEmployee_Travel model = new tblEmployee_Travel
            {
                EmployeeId = employee.EmployeeId,
                StartDate = DateTime.Today,
                EndDate = DateTime.Today
            };

            return PartialView("_Travel",model);
        }

        public void _CreatePost(FormCollection _model, ref queryResult res)
        {
            tblEmployee_Travel model = (tblEmployee_Travel)UpdateModel(new tblEmployee_Travel().GetType(), _model);

            using (dalDataContext context = new dalDataContext())
            {
               

                if (model.StartDate > model.EndDate)
                {
                    AddError("From date cannot be later than To date");
                }

                if (context.tblEmployee_Travels.Where(x => x.EmployeeId == model.EmployeeId).ToList().Any(x => model.IsSingleDate ? x.Match(model.StartDate) : x.Match(model.StartDate, model.EndDate)))
                {
                    AddError("Date(s) overlap with existing entry");
                }

                if (ModelState.IsValid)
                {

                    tblEmployee_Travel Travel = new tblEmployee_Travel
                    {
                        EmployeeId = model.EmployeeId,
                        StartDate = model.StartDate,
                        EndDate = model.EndDate,
                        Purpose = model.Purpose,
                        Destination = model.Destination,
                        CreateDate = DateTime.Now,
                        CreatedBy_UserId = Cache.Get().userAccess.user.Id
                    };

                    context.tblEmployee_Travels.InsertOnSubmit(Travel);
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
            ViewBag.Title = "Travel";
            

            ViewBag.uiIsReadOnly = false;
            ViewBag.uiIncludeId = true;

            using (dalDataContext context = new dalDataContext())
            {
                tblEmployee_Travel Travel = context.tblEmployee_Travels.Where(x => x.EmployeeId == employee.EmployeeId && x.Id == id).Single();
                if (Travel == null)
                {
                    return new Raw_ActionResult(ModuleConstants.ID_NOT_FOUND);
                }
                else
                {
                    return PartialView("_Travel",Travel);
                }
            }
        }


        public void _EditPost(FormCollection _model, ref queryResult res)
        {
            tblEmployee_Travel model = (tblEmployee_Travel)UpdateModel(new tblEmployee_Travel().GetType(), _model);

            using (dalDataContext context = new dalDataContext())
            {
                tblEmployee_Travel Travel = context.tblEmployee_Travels.Where(x => x.Id == model.Id).SingleOrDefault();
                if (Travel == null)
                {
                    throw new Exception(ModuleConstants.ID_NOT_FOUND);
                }
                else
                {

                    if (model.StartDate > model.EndDate)
                    {
                        AddError("From date cannot be later than To date");
                    }

                    if (context.tblEmployee_Travels.Where(x => x.Id != model.Id && x.EmployeeId == model.EmployeeId).ToList().Any(x => model.IsSingleDate ? x.Match(model.StartDate) : x.Match(model.StartDate, model.EndDate)))
                    {
                        AddError("Date(s) overlap with existing entry");
                    }

                    if (ModelState.IsValid)
                    {
                        Travel.StartDate = model.StartDate;
                        Travel.EndDate = model.EndDate;
                        Travel.Purpose = model.Purpose;
                        Travel.Destination = model.Destination;

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
            using (dalDataContext context = new dalDataContext())
            {

                tblEmployee_Travel Travel = context.tblEmployee_Travels.Where(x => x.Id == id).SingleOrDefault();
                if (Travel == null)
                {
                    throw new Exception(ModuleConstants.ID_NOT_FOUND);
                }

                if (Travel.GetApplication() != null)
                {
                    AddError("Cannot delete approved item. You may revert approval to remove this item");
                }

                if (ModelState.IsValid)
                {
                    context.tblEmployee_Travels.DeleteOnSubmit(Travel);
                    context.SubmitChanges();

                    res.Remarks = "Record was successfully deleted";
                }
                else
                {
                    throw new Exception(coreProcs.ShowErrors(ModelState));
                }
            }
        }
    }
}
