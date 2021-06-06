using coreApp.Models;
using coreLib.Enums;
using coreLib.Objects;
using Module.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HRLogixMobileLib;

namespace coreApp.Controllers
{
    public class WidgetsController : Base_NoCoreAuthorizedController
    {
        [ChildActionOnly]
        public ActionResult BulletinBoard(int count = 6)
        {
            return PartialView(new coreApp.Areas.HRModule.Controllers.BulletinBoardViewController().WidgetList(count));
        }

        public ActionResult PeriodAttendance(int stakeholderId, DateTime? startDate = null, DateTime? endDate = null, bool showData = false, string contSelector = "")
        {
            PeriodModel pm = new PeriodModel(PeriodModelInitType.ThisMonth);

            if (startDate != null) pm.StartDate = startDate.Value;
            if (endDate != null) pm.EndDate = endDate.Value;

            int noOfDays = pm.NoOfDays();
            
            etPeriod period;

            try
            {
                period = new etPeriod(stakeholderId, pm.StartDate, pm.EndDate);
            }
            catch (Exception ex)
            {
                TempData["PA_GlobalError"] = ex.Message;
                pm = new PeriodModel(PeriodModelInitType.ThisMonth);

                period = new etPeriod(stakeholderId, pm.StartDate, pm.EndDate);
            }


            TimeSettingsModel settings = period.data.settings;

            LineChartModel model = new LineChartModel
            {
                PM = pm,
                Period = period
            };

            DateTime d = pm.StartDate;

            List<double> dsWorkHours = new List<double>();
            List<double> dsHoursWorked = new List<double>();

            if (noOfDays <= 31)
            {
                foreach (etDay day in period.Days)
                {                    
                    double wh = day.PeriodAttendance_WorkHours;
                    double hw = day.PeriodAttendance_HoursWorked;

                    if (wh == 0) continue;
                        
                    model.Labels.Add(day.d.Day.ToString());
                    dsWorkHours.Add(Math.Round(wh, 1));
                    dsHoursWorked.Add(Math.Round(hw, 1));
                }
            }
            else if (noOfDays <= 93)
            {
                int weekNo = 1;
                string moyr = d.ToString("MMyyyy");

                while (d <= pm.EndDate)
                {
                    if (moyr != d.ToString("MMyyyy"))
                    {
                        weekNo = 1;
                        moyr = d.ToString("MMyyyy");
                    }

                    etDay day = period.Days.Where(x => x.d == d).Single();
                    
                    DateTime weekStart = d.AddDays(-1 * (int)(d.DayOfWeek));
                    DateTime weekEnd = weekStart.AddDays(6);

                    if (weekStart < period.startDate)
                    {
                        weekStart = period.startDate;
                    }

                    if (weekEnd > period.endDate)
                    {
                        weekEnd = period.endDate;
                    }

                    double wh = Math.Round(period.Days.Where(x => x.d >= weekStart && x.d <= weekEnd).Sum(x => x.PeriodAttendance_WorkHours), 1);
                    double hw = Math.Round(period.Days.Where(x => x.d >= weekStart && x.d <= weekEnd).Sum(x => x.PeriodAttendance_HoursWorked), 1);

                    if (wh > 0)
                    {
                        model.Labels.Add(string.Format("Week {0} ({1})", weekNo, d.ToString("MMM")));

                        dsWorkHours.Add(wh);
                        dsHoursWorked.Add(hw);
                    }

                    d = weekEnd.AddDays(1);
                    weekNo++;
                }
            }
            else
            {
                while (d <= pm.EndDate)
                {
                    bool IsDec = d.Month == 12;

                    etDay day = period.Days.Where(x => x.d == d).Single();

                    DateTime monthStart = new DateTime(d.Year, d.Month, 1);
                    DateTime monthEnd = IsDec ? new DateTime(d.Year, 12, 31) : new DateTime(d.Year, d.Month + 1, 1).AddDays(-1);

                    if (monthStart < period.startDate)
                    {
                        monthStart = period.startDate;
                    }

                    if (monthEnd > period.endDate)
                    {
                        monthEnd = period.endDate;
                    }

                    double wh = Math.Round(period.Days.Where(x => x.d >= monthStart && x.d <= monthEnd).Sum(x => x.PeriodAttendance_WorkHours), 1);
                    double hw = Math.Round(period.Days.Where(x => x.d >= monthStart && x.d <= monthEnd).Sum(x => x.PeriodAttendance_HoursWorked), 1);

                    if (wh > 0)
                    {
                        model.Labels.Add(d.ToString("MMM"));
                        
                        dsWorkHours.Add(wh);
                        dsHoursWorked.Add(hw);
                    }

                    d = monthEnd.AddDays(1);
                }
            }

            model.Data.Add(dsWorkHours);
            model.Data.Add(dsHoursWorked);
            
            ViewBag.ShowData = showData;
            ViewBag.ContSelector = contSelector;

            return PartialView(model);

        }

        public ActionResult DailyAttendanceMonitor(int stakeholderId, DateTime? dt = null)
        {
            PeriodModel pm = new PeriodModel();

            if (dt == null)
            {
                dt = DateTime.Today;
            }

            pm.StartDate = dt.Value;
            pm.EndDate = dt.Value.Date.AddHours(23).AddMinutes(59).AddSeconds(59);
            
            etPeriod period = new etPeriod(stakeholderId, pm.StartDate, pm.EndDate);
            
            return PartialView(period);

        }

        public ActionResult Applications()
        {
            coreApp.UserAccess access = coreApp.Cache.Get().userAccess;
            OnlineApplicationsModel model = GetApplicationsModel(access);

            return PartialView(model);
        }

        public OnlineApplicationsModel GetApplicationsModel(coreApp.UserAccess access, HttpRequestBase request = null)
        {
            OnlineApplicationsModel model = new OnlineApplicationsModel();

            using (dalDataContext context = new dalDataContext())
            {
                model.Leave = 0;

                if (access.HasAccess("hr_emp_leave_app"))
                {
                    model.Leave = context.tblLeaveApplications
                                    .Where(x => x.DateSubmitted != null && x.ApproveDate == null && x.DenyDate == null)
                                    .ToList()
                                    .Where(x => Cache.Get().userAccess.AllowedStakeholder(x.StakeholderId, disableMe: true, forLeave: true) && x.HasBeenSubmitted && !x.IsRecommended)
                                    .Count();
                }

                if (access.HasAccess("hr_emp_leave_app_approve"))
                {
                    model.Leave += context.tblLeaveApplications
                                    .Where(x => x.DateSubmitted != null && x.ApproveDate == null && x.DenyDate == null)
                                    .ToList()
                                    .Where(x => Cache.Get().userAccess.AllowedStakeholder(x.StakeholderId, disableMe: true, forLeave: true) && x.IsRecommended && !x.HasBeenServed)
                                    .Count();
                }

                if (access.HasAccess("hr_emp_travel_app"))
                {
                    model.Travel = context.tblTravelApplications
                                    .Where(x => x.DateSubmitted != null && x.ApproveDate == null && x.DenyDate == null)
                                    .ToList()
                                    .Where(x => Cache.Get().userAccess.AllowedStakeholder(x.StakeholderId) && x.HasBeenSubmitted && !x.HasBeenServed)
                                    .Count();
                }

                if (access.HasAccess("hr_emp_ot_app"))
                {
                    model.OT = context.tblOTApplications
                                   .Where(x => x.DateSubmitted != null && x.ApproveDate == null && x.DenyDate == null)
                                   .ToList()
                                   .Where(x => Cache.Get().userAccess.AllowedStakeholder(x.StakeholderId) && x.HasBeenSubmitted && !x.HasBeenServed)
                                   .Count();
                }
            }

            objGreeting greeting = new objGreeting(access.stakeholder.FirstName);
            int count = model.Total;

            model.Greetings = string.Format("Good {0} {1}, there {2} {3} today.",
                            greeting.Greeting,
                            greeting.Me,
                            model.Total > 1 ? "are" : "is",
                            model.GetString(model.Total).ToLower()
                            );

            var requestContext = request == null ? Request.RequestContext : request.RequestContext;
            string scheme = requestContext.HttpContext.Request.Url.Scheme;

            Dictionary<string, string> tmp = new Dictionary<string, string>();
            if (model.Leave > 0 && access.HasAccess("hr_emp_leave_app"))
            {
                tmp.Add(model.GetString(model.Leave, "{0} Leave application{1}"), new UrlHelper(requestContext).Action("Index", "LeaveApplications", new { area = "HRModule" }, scheme));
            }
            if (model.Travel > 0 && access.HasAccess("hr_emp_travel_app"))
            {
                tmp.Add(model.GetString(model.Travel, "{0} Travel application{1}"), new UrlHelper(requestContext).Action("Index", "TravelApplications", new { area = "HRModule" }, scheme));
            }
            if (model.OT > 0 && access.HasAccess("hr_emp_ot_app"))
            {
                tmp.Add(model.GetString(model.OT, "{0} Overtime application{1}"), new UrlHelper(requestContext).Action("Index", "OTApplications", new { area = "HRModule" }, scheme));
            }
            
            model.Greetings_Details = tmp;

            return model;
        }
    }
}