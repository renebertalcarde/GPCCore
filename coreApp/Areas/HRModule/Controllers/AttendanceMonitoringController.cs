using coreApp.Areas.FinanceModule.Models;
using coreApp.Controllers;
using coreApp.DAL;
using coreApp.Enums;
using coreApp.Models;
using coreLib.Objects;
using Module.Core;
using Module.DB;
using Module.DB.Enums;
using Module.Time;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace coreApp.Areas.HRModule.Controllers
{
    public class AttendanceMonitoringController : HLBase_NoCoreAuthorizedController
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ViewList(DateTime date, string sessionId)
        {
            ViewBag.SessionId = sessionId;
            ViewBag.Date = date;
            
            List<AMModel> model = (List<AMModel>)Session[sessionId];
            return PartialView(model);
        }

        public ActionResult GenerateList(DateTime date, string lastName = "", string firstName = "", int mfoId = -1, string departmentIds = "", int positionId = -1, int groupId = -1, int employmentType = -1, bool excludeNoCareer = false, bool excludeNoOffice = false, string altSource = "", string active = "")
        {
            
            queryResult res = new queryResult { IsSuccessful = true, Data = null, Remarks = "", Err = "" };

            try
            {
                using (dalDataContext context = new dalDataContext())
                {
                    DateTime startDate = date.Date;
                    DateTime endDate = startDate.AddDays(1).AddSeconds(-1);

                    List<tblEmployee> employees = new EmployeeSearch(altSource).GetEmployees(lastName, firstName, mfoId, departmentIds, positionId, groupId, employmentType, excludeNoCareer, excludeNoOffice, active);

                    employees = employees.Where(x => x._tmpCareer != null && x.IsActive()).ToList();

                    List<AMModel> model = new List<AMModel>();
                    List<string> errors = new List<string>();

                    int count = employees.Count();

                    int i = 1;
                    foreach (tblEmployee emp in employees)
                    {
                        Response.ContentType = "text/event-stream";
                        Response.Buffer = false;

                        try
                        {
                            etPeriod period = new etPeriod(emp.EmployeeId, startDate, endDate, null);
                            etDay day = period.Days.First();

                            model.Add(new AMModel(emp, day));
                        }
                        catch (Exception ex)
                        {
                            string err = string.Format("{0} - {1}", emp.FullName, coreProcs.ShowErrors(ex));
                            errors.Add(err);
                        }

                        IterateEmployeeHandler(i, count);
                        i++;
                    }

                    string sessionId = Guid.NewGuid().ToString();
                    Session[sessionId] = model;

                    res.Data = new
                    {
                        sessionId = sessionId,
                        errors = errors
                    };
                    
                    Response.ContentType = "text/event-stream";
                    Response.Buffer = false;
                }
            }
            catch (Exception e)
            {
                res.IsSuccessful = false;
                res.Err = coreProcs.ShowErrors(e);
            }

            string finalData = string.Format("data:{0}\n\n", JsonConvert.SerializeObject(res));
            return Content(finalData, "text/event-stream");
        }

        private void IterateEmployeeHandler(int current, int total)
        {
            ProgressData pd = new ProgressData { Total = total, Current = current, IsProgressUpdate = true };
            string data = string.Format("data:{0}\n\n", JsonConvert.SerializeObject(pd));

            Response.Write(data);
            Response.Flush();
            Response.Clear();
        }

        public ActionResult PeriodAttendance(int id)
        {
            using (dalDataContext context = new dalDataContext())
            {
                tblEmployee employee = context.tblEmployees.Where(x => x.EmployeeId == id).SingleOrDefault();
                if (employee == null)
                {
                    throw new Exception(ModuleConstants.RECORD_ID_NOT_FOUND);
                }

                return View(employee);
            }
        }
    }
}