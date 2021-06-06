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
    public class EmployeeRDController : HLBase_AuthorizedController, IEmployeeController
    {
        public tblEmployee employee { get; set; }

        public EmployeeRDController()
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

                var model = context.tblEmployee_RDs.Where(x => x.EmployeeId == employee.EmployeeId).OrderByDescending(x => x.StartDate).ToList();
                return View(model);
            }
        }

        public ActionResult _Details(int? id)
        {
            ViewBag.Title = "Rest Day";
            

            ViewBag.uiIsReadOnly = true;
            ViewBag.uiIncludeId = true;

            using (dalDataContext context = new dalDataContext())
            {
                tblEmployee_RD RD = context.tblEmployee_RDs.Where(x => x.Id == id).SingleOrDefault();
                if (RD == null)
                {
                    return new Raw_ActionResult(ModuleConstants.ID_NOT_FOUND);
                }
                else
                {
                    return PartialView("_RD", RD);
                }
            }
        }

        public ActionResult _Create()
        {
            ViewBag.Title = "Rest Day";
            

            ViewBag.uiIsReadOnly = false;
            ViewBag.uiIncludeId = false;

            tblEmployee_RD model = new tblEmployee_RD
            {
                EmployeeId = employee.EmployeeId,
                StartDate = DateTime.Today,
                EndDate = DateTime.Today,
                Days_Sun = true
            };

            return PartialView("_RD",model);
        }

        public void _CreatePost(FormCollection _model, ref queryResult res)
        {
            tblEmployee_RD model = (tblEmployee_RD)UpdateModel(new tblEmployee_RD().GetType(), _model);

            using (dalDataContext context = new dalDataContext())
            {

                if (model.StartDate > model.EndDate)
                {
                    AddError("From date cannot be later than To date");
                }

                if (context.tblEmployee_RDs.Where(x => x.EmployeeId == model.EmployeeId).ToList().Any(x => model.IsSingleDate ? x.Match(model.StartDate) : x.Match(model.StartDate, model.EndDate)))
                {
                    AddError("Date(s) overlap with existing entry");
                }

                if (ModelState.IsValid)
                {

                    tblEmployee_RD RD = new tblEmployee_RD
                    {
                        EmployeeId = model.EmployeeId,
                        Description = model.Description,
                        Days_Sun = model.Days_Sun,
                        Days_Mon = model.Days_Mon,
                        Days_Tue = model.Days_Tue,
                        Days_Wed = model.Days_Wed,
                        Days_Thu = model.Days_Thu,
                        Days_Fri = model.Days_Fri,
                        Days_Sat = model.Days_Sat,
                        StartDate = model.StartDate,
                        EndDate = model.EndDate
                    };

                    context.tblEmployee_RDs.InsertOnSubmit(RD);
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
            ViewBag.Title = "Rest Day";
            

            ViewBag.uiIsReadOnly = false;
            ViewBag.uiIncludeId = true;

            using (dalDataContext context = new dalDataContext())
            {
                tblEmployee_RD RD = context.tblEmployee_RDs.Where(x => x.EmployeeId == employee.EmployeeId && x.Id == id).Single();
                if (RD == null)
                {
                    return new Raw_ActionResult(ModuleConstants.ID_NOT_FOUND);
                }
                else
                {
                    return PartialView("_RD", RD);
                }
            }
        }


        public void _EditPost(FormCollection _model, ref queryResult res)
        {
            tblEmployee_RD model = (tblEmployee_RD)UpdateModel(new tblEmployee_RD().GetType(), _model);

            using (dalDataContext context = new dalDataContext())
            {
                tblEmployee_RD RD = context.tblEmployee_RDs.Where(x => x.Id == model.Id).SingleOrDefault();
                if (RD == null)
                {
                    throw new Exception(ModuleConstants.ID_NOT_FOUND);
                }
                else
                {

                    if (model.StartDate > model.EndDate)
                    {
                        AddError("From date cannot be later than To date");
                    }

                    if (context.tblEmployee_RDs.Where(x => x.Id != model.Id && x.EmployeeId == model.EmployeeId).ToList().Any(x => model.IsSingleDate ? x.Match(model.StartDate) : x.Match(model.StartDate, model.EndDate)))
                    {
                        AddError("Date(s) overlap with existing entry");
                    }

                    if (ModelState.IsValid)
                    {
                        RD.Description = model.Description;
                        RD.Days_Sun = model.Days_Sun;
                        RD.Days_Mon = model.Days_Mon;
                        RD.Days_Tue = model.Days_Tue;
                        RD.Days_Wed = model.Days_Wed;
                        RD.Days_Thu = model.Days_Thu;
                        RD.Days_Fri = model.Days_Fri;
                        RD.Days_Sat = model.Days_Sat;
                        RD.StartDate = model.StartDate;
                        RD.EndDate = model.EndDate;
                        

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

                tblEmployee_RD RD = context.tblEmployee_RDs.Where(x => x.Id == id).SingleOrDefault();
                if (RD == null)
                {
                    throw new Exception(ModuleConstants.ID_NOT_FOUND);
                }

                if (ModelState.IsValid)
                {
                    context.tblEmployee_RDs.DeleteOnSubmit(RD);
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
