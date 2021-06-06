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
    public class EmployeeOTController : HLBase_AuthorizedController, IEmployeeController
    {
        public tblEmployee employee { get; set; }

        public EmployeeOTController()
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

                var model = context.tblEmployee_OTs.Where(x => x.EmployeeId == employee.EmployeeId).OrderByDescending(x => x.StartDate).ToList();
                return View(model);
            }
        }

        public ActionResult _Details(int? id)
        {
            ViewBag.Title = "OT";
            

            ViewBag.uiIsReadOnly = true;
            ViewBag.uiIncludeId = true;

            using (dalDataContext context = new dalDataContext())
            {
                tblEmployee_OT OT = context.tblEmployee_OTs.Where(x => x.Id == id).SingleOrDefault();
                if (OT == null)
                {
                    return new Raw_ActionResult(ModuleConstants.ID_NOT_FOUND);
                }
                else
                {
                    return PartialView("_OT",OT);
                }
            }
        }

        public ActionResult _Create()
        {
            ViewBag.Title = "OT";
            

            ViewBag.uiIsReadOnly = false;
            ViewBag.uiIncludeId = false;

            tblEmployee_OT model = new tblEmployee_OT
            {
                EmployeeId = employee.EmployeeId,
                StartDate = DateTime.Today,
                EndDate = DateTime.Today,
                MustTimeIn = false,
                MustTimeOut = true,
                Breaks = 0,
                OTRateOnly = false,
                Days_Sun = true,
                Days_Mon = true,
                Days_Tue = true,
                Days_Wed = true,
                Days_Thu = true,
                Days_Fri = true,
                Days_Sat = true
            };

            return PartialView("_OT",model);
        }

        public void _CreatePost(FormCollection _model, ref queryResult res)
        {
            tblEmployee_OT model = (tblEmployee_OT)UpdateModel(new tblEmployee_OT().GetType(), _model);

            using (dalDataContext context = new dalDataContext())
            {

                if (model.StartDate > model.EndDate)
                {
                    AddError("From date cannot be later than To date");
                }

                if (context.tblEmployee_OTs.Where(x => x.EmployeeId == model.EmployeeId).ToList().Any(x => model.IsSingleDate ? x.Match(model.StartDate) : x.Match(model.StartDate, model.EndDate)))
                {
                    AddError("Date(s) overlap with existing entry");
                }

                if (string.IsNullOrEmpty(model.Days))
                {
                    AddError("Days not specified");
                }

                if (ModelState.IsValid)
                {

                    tblEmployee_OT OT = new tblEmployee_OT
                    {
                        EmployeeId = model.EmployeeId,
                        TimeInFrom = model.TimeInFrom,
                        TimeInTo = model.TimeInTo,
                        TimeIn = model.TimeIn,
                        TimeOutFrom = model.TimeOutFrom,
                        TimeOutTo = model.TimeOutTo,
                        TimeOut = model.TimeOut,
                        TimeInFrom_IsPrev = model.TimeInFrom_IsPrev,
                        TimeInTo_IsNext = model.TimeInTo_IsNext,
                        TimeOut_IsNext = model.TimeOut_IsNext,
                        TimeOutFrom_IsNext = model.TimeOutFrom_IsNext,
                        TimeOutTo_IsNext = model.TimeOutTo_IsNext,
                        MustTimeIn = model.MustTimeIn,
                        MustTimeOut = model.MustTimeOut,
                        Days_Sun = model.Days_Sun,
                        Days_Mon = model.Days_Mon,
                        Days_Tue = model.Days_Tue,
                        Days_Wed = model.Days_Wed,
                        Days_Thu = model.Days_Thu,
                        Days_Fri = model.Days_Fri,
                        Days_Sat = model.Days_Sat,
                        StartDate = model.StartDate,
                        EndDate = model.EndDate,
                        Description = model.Description,
                        Remarks = model.Remarks,
                        CreateDate = DateTime.Now,
                        CreatedBy_UserId = Cache.Get().userAccess.user.Id,
                        Breaks = model.Breaks,
                        WorkDayEq = model.WorkDayEq,
                        SkipLastLog = model.SkipLastLog,
                        OTRateOnly = model.OTRateOnly
                    };

                    context.tblEmployee_OTs.InsertOnSubmit(OT);
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
            ViewBag.Title = "OT";
            

            ViewBag.uiIsReadOnly = false;
            ViewBag.uiIncludeId = true;

            using (dalDataContext context = new dalDataContext())
            {
                tblEmployee_OT OT = context.tblEmployee_OTs.Where(x => x.EmployeeId == employee.EmployeeId && x.Id == id).Single();
                if (OT == null)
                {
                    return new Raw_ActionResult(ModuleConstants.ID_NOT_FOUND);
                }
                else
                {
                    return PartialView("_OT",OT);
                }
            }
        }


        public void _EditPost(FormCollection _model, ref queryResult res)
        {
            tblEmployee_OT model = (tblEmployee_OT)UpdateModel(new tblEmployee_OT().GetType(), _model);

            using (dalDataContext context = new dalDataContext())
            {
                tblEmployee_OT OT = context.tblEmployee_OTs.Where(x => x.Id == model.Id).SingleOrDefault();
                if (OT == null)
                {
                    throw new Exception(ModuleConstants.ID_NOT_FOUND);
                }
                else
                {

                    if (model.StartDate > model.EndDate)
                    {
                        AddError("From date cannot be later than To date");
                    }

                    if (context.tblEmployee_OTs.Where(x => x.Id != model.Id && x.EmployeeId == model.EmployeeId).ToList().Any(x => model.IsSingleDate ? x.Match(model.StartDate) : x.Match(model.StartDate, model.EndDate)))
                    {
                        AddError("Date(s) overlap with existing entry");
                    }

                    if (string.IsNullOrEmpty(model.Days))
                    {
                        AddError("Days not specified");
                    }

                    if (ModelState.IsValid)
                    {
                        OT.TimeInFrom = model.TimeInFrom;
                        OT.TimeInTo = model.TimeInTo;
                        OT.TimeIn = model.TimeIn;
                        OT.TimeOutFrom = model.TimeOutFrom;
                        OT.TimeOutTo = model.TimeOutTo;
                        OT.TimeOut = model.TimeOut;
                        OT.TimeInFrom_IsPrev = model.TimeInFrom_IsPrev;
                        OT.TimeInTo_IsNext = model.TimeInTo_IsNext;
                        OT.TimeOut_IsNext = model.TimeOut_IsNext;
                        OT.TimeOutFrom_IsNext = model.TimeOutFrom_IsNext;
                        OT.TimeOutTo_IsNext = model.TimeOutTo_IsNext;
                        OT.MustTimeIn = model.MustTimeIn;
                        OT.MustTimeOut = model.MustTimeOut;
                        OT.Days_Sun = model.Days_Sun;
                        OT.Days_Mon = model.Days_Mon;
                        OT.Days_Tue = model.Days_Tue;
                        OT.Days_Wed = model.Days_Wed;
                        OT.Days_Thu = model.Days_Thu;
                        OT.Days_Fri = model.Days_Fri;
                        OT.Days_Sat = model.Days_Sat;
                        OT.StartDate = model.StartDate;
                        OT.EndDate = model.EndDate;
                        OT.Description = model.Description;
                        OT.Remarks = model.Remarks;
                        OT.Breaks = model.Breaks;
                        OT.WorkDayEq = model.WorkDayEq;
                        OT.SkipLastLog = model.SkipLastLog;
                        OT.OTRateOnly = model.OTRateOnly;

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

                tblEmployee_OT OT = context.tblEmployee_OTs.Where(x => x.Id == id).SingleOrDefault();
                if (OT == null)
                {
                    throw new Exception(ModuleConstants.ID_NOT_FOUND);
                }

                if (OT.GetApplication() != null)
                {
                    AddError("Cannot delete approved item. You may revert approval to remove this item");
                }

                if (ModelState.IsValid)
                {
                    context.tblEmployee_OTs.DeleteOnSubmit(OT);
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
