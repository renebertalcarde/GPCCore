
using coreApp.Areas.TimeModule.ViewModels;
using coreApp.Controllers;
using coreApp.DAL;
using coreApp.Filters;
using coreApp.Interfaces;
using coreLib.Enums;
using coreLib.Objects;
using Module.Core;
using Module.DB;
using Module.DB.Enums;
using Module.Time;
using reports.AsposeLib;
using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Web.Mvc;

namespace coreApp.Areas.EmployeeModule.Controllers
{
    [MyFilter]
    public class MyTimeLogsController : Base_NoCoreEmployeeController, IMyController
    {
        public tblEmployee employee { get; set; }

        public ActionResult Index(string startDate, string endDate)
        {
            PeriodModel pm = new PeriodModel(PeriodModelInitType.ThisMonth);
            DateTime tmp;

            if (DateTime.TryParse(startDate, out tmp))
            {
                pm.StartDate = tmp;
            }

            if (DateTime.TryParse(endDate, out tmp))
            {
                pm.EndDate = tmp;
            }

            using (dalDataContext context = new dalDataContext())
            {
                ViewBag.Employee = employee;
                ViewBag.Period = pm;

                var model = context.tblEmployee_TimeLogs.Where(x => x.EmployeeId == employee.EmployeeId && x.TimeLog >= pm.StartDate && x.TimeLog <= pm.EndDate).OrderByDescending(x => x.TimeLog).ToList();

                return View(model);
            }
        }

        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new Raw_ActionResult(ModuleConstants.BAD_REQUEST);
            }

            ViewBag.Title = "Time Log";


            ViewBag.uiIsReadOnly = true;
            ViewBag.uiIncludeId = true;

            using (dalDataContext context = new dalDataContext())
            {
                tblEmployee_TimeLog TimeLog = context.tblEmployee_TimeLogs.Where(x => x.EmployeeId == employee.EmployeeId && x.LogId == id).SingleOrDefault();
                if (TimeLog == null)
                {
                    return new Raw_ActionResult(ModuleConstants.ID_NOT_FOUND);
                }
                else
                {
                    return PartialView("~/Areas/TimeModule/Views/EmployeeTimeLogs/_TimeLog.cshtml", TimeLog);
                }
            }
        }

        public ActionResult Create()
        {
            ViewBag.Title = "Time Log";


            ViewBag.uiIsReadOnly = false;
            ViewBag.uiIncludeId = false;

            tblEmployee_TimeLog model = new tblEmployee_TimeLog
            {
                EmployeeId = employee.EmployeeId,
                TimeLog = DateTime.Now,
                Mode = (int)DeviceLogMode.In
            };

            return PartialView("~/Areas/TimeModule/Views/EmployeeTimeLogs/_TimeLog.cshtml", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Create(tblEmployee_TimeLog model)
        {
            queryResult res = new queryResult { IsSuccessful = true, Data = null, Err = "", Remarks = "" };

            try
            {
                using (dalDataContext context = new dalDataContext())
                {
                    if (!Cache.Get().userAccess.HasAccess("hr_emp_timelogs_modify"))
                    {
                        AddError(ModuleConstants_Authorization.USERACCESS_UNAUTHORIZED_ACTION);
                    }

                    if (model.TimeLog < Constants.VALID_MIN_DATETIME || model.TimeLog > Constants.VALID_MAX_DATETIME)
                    {
                        AddError(string.Format("The field Time Log must be between {0} and {1}", Constants.VALID_MIN_DATETIME, Constants.VALID_MAX_DATETIME));
                    }

                    if (ModelState.IsValid)
                    {

                        tblEmployee_TimeLog TimeLog = new tblEmployee_TimeLog
                        {
                            EmployeeId = employee.EmployeeId,
                            TimeLog = model.TimeLog,
                            Mode = model.Mode,
                            EntryType = (int)TimeLogEntryType.Manual,
                            EnteredBy_UserId = Cache.Get().userAccess.user.Id,
                            DeviceReference = model.DeviceReference
                        };

                        context.tblEmployee_TimeLogs.InsertOnSubmit(TimeLog);
                        context.SubmitChanges();

                        res.Remarks = "Record was successfully created";

                        MvcApplication.HLLog.WriteLog("Employee TimeLogs", "Insert TimeLog",
                                  string.Format("Inserted record for [{0}] {1}", employee.EmployeeId, employee.FullName),
                                  TimeLog);

                    }
                    else
                    {
                        throw new Exception(coreProcs.ShowErrors(ModelState));
                    }
                }
            }
            catch (Exception ex)
            {
                res.IsSuccessful = false;
                res.Err = coreProcs.ShowErrors(ex);
            }

            return Json(res);
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new Raw_ActionResult(ModuleConstants.BAD_REQUEST);
            }

            ViewBag.Title = "TimeLog";


            ViewBag.uiIsReadOnly = false;
            ViewBag.uiIncludeId = true;

            using (dalDataContext context = new dalDataContext())
            {
                tblEmployee_TimeLog TimeLog = context.tblEmployee_TimeLogs.Where(x => x.EmployeeId == employee.EmployeeId && x.LogId == id).Single();
                if (TimeLog == null)
                {
                    return new Raw_ActionResult(ModuleConstants.ID_NOT_FOUND);
                }
                else
                {
                    return PartialView("~/Areas/TimeModule/Views/EmployeeTimeLogs/_TimeLog.cshtml", TimeLog);
                }
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(tblEmployee_TimeLog model)
        {
            queryResult res = new queryResult { IsSuccessful = true, Data = null, Err = "", Remarks = "" };

            try
            {
                using (dalDataContext context = new dalDataContext())
                {
                    tblEmployee_TimeLog TimeLog = context.tblEmployee_TimeLogs.Where(x => x.LogId == model.LogId).SingleOrDefault();
                    if (TimeLog == null)
                    {
                        throw new Exception(ModuleConstants.ID_NOT_FOUND);
                    }
                    else
                    {
                        if (model.TimeLog < Constants.VALID_MIN_DATETIME || model.TimeLog > Constants.VALID_MAX_DATETIME)
                        {
                            AddError(string.Format("The field Time Log must be between {0} and {1}", Constants.VALID_MIN_DATETIME, Constants.VALID_MAX_DATETIME));
                        }

                        if (ModelState.IsValid)
                        {
                            TimeLog.EmployeeId = model.EmployeeId;
                            TimeLog.Mode = model.Mode;

                            ModifiedMemberInfo[] mmi = context.tblEmployee_TimeLogs.GetModifiedMembers(TimeLog);
                            context.SubmitChanges();

                            res.Remarks = "Record was successfully updated";

                            new coreApp.Areas.TimeModule.Controllers.EmployeeTimeLogsController() { employee = employee }.Log_Edit(TimeLog, mmi);
                        }
                        else
                        {
                            throw new Exception(coreProcs.ShowErrors(ModelState));
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                res.IsSuccessful = false;
                res.Err = coreProcs.ShowErrors(ex);
            }

            return Json(res);
        }

        public ActionResult Print(bool dlWord = false, string startDate = "", string endDate = "")
        {
            PeriodModel pm = new PeriodModel(PeriodModelInitType.ThisMonth);
            DateTime tmp;

            if (DateTime.TryParse(startDate, out tmp))
            {
                pm.StartDate = tmp;
            }

            if (DateTime.TryParse(endDate, out tmp))
            {
                pm.EndDate = tmp;
            }

            using (dalDataContext context = new dalDataContext())
            {

                List<tblEmployee_TimeLog> model = context.tblEmployee_TimeLogs.Where(x => x.EmployeeId == employee.EmployeeId && x.TimeLog >= pm.StartDate && x.TimeLog <= pm.EndDate).OrderByDescending(x => x.TimeLog).ToList();
                TimelogViewModel data = new TimelogViewModel { periodModel = pm, timelogs = model };

                string fn = string.Format("timelogs-{0}", employee.LastName.ToLower());

                objData CustomData = new objData
                {
                    ApplicationName = MvcApplication.ApplicationLicense.Info.ApplicationName,
                    ModuleName = "HR",
                    ReportName = "timelogs"
                };

                string appPath = Server.MapPath("~/");

                return new asposeWordsTemplateReport(new TimeModule.Controllers.EmployeeTimeLogsController { employee = employee }.CustomizeDoc_Aspose, null, FixedSettings.ApplicationName, FixedSettings.Owner,
                     new ReportFooterParams(appPath), CustomData)
                    .Get(CustomData.ReportName, fn, data, dlWord);
            }
        }
    }
}
