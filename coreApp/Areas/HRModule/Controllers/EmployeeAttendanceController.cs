using System.Linq;
using System.Web.Mvc;
using System;
using coreApp.DAL;
using coreApp.Controllers;
using coreApp.Interfaces;
using coreApp.Filters;
using reports.AsposeLib;
using Aspose.Words;
using Module.DB;
using Module.Time;
using coreLib.Objects;
using coreLib.Enums;
using System.Drawing;
using System.Collections.Generic;
using Module.DB.Enums;
using Module.DB.Interfaces;

namespace coreApp.Areas.HRModule.Controllers
{
    [EmployeeInfoFilter]
    [EmployeeAuthorize(true)]
    [UserAccessAuthorize(allowedAccess: "hr_module,hr_emp_sched")]
    public class EmployeeAttendanceController : HLBase_NoCoreAuthorizedController, IEmployeeController
    {
        public tblEmployee employee { get; set; }
        public string serverPath { get; set; }

        public ActionResult Index(string startDate, string endDate, string filters = "")
        {
            PeriodModel pm = new PeriodModel(PeriodModelInitType.ThisMonth);
            DateTime tmp;
            etPeriodFilter filter = null;

            if (DateTime.TryParse(startDate, out tmp))
            {
                pm.StartDate = tmp;
            }

            if (DateTime.TryParse(endDate, out tmp))
            {
                pm.EndDate = tmp;
            }

            if (!string.IsNullOrEmpty(filters))
            {
                filter = new etPeriodFilter(filters);
            }

            ViewBag.Period = pm;

            using (hr2017DataContext context = new hr2017DataContext())
            {
                AttendanceModel model = new AttendanceModel { periodModel = pm };

                model.period = new etPeriod(employee.EmployeeId, pm.StartDate, pm.EndDate, null, filter: filter);
                
                return View(model);
            }
        }

        public ActionResult Print(string startDate = "", string endDate = "", string filters = "")
        {
            PeriodModel pm = new PeriodModel(PeriodModelInitType.ThisMonth);
            DateTime tmp;
            etPeriodFilter filter = null;

            if (DateTime.TryParse(startDate, out tmp))
            {
                pm.StartDate = tmp;
            }

            if (DateTime.TryParse(endDate, out tmp))
            {
                pm.EndDate = tmp;
            }

            if (!string.IsNullOrEmpty(filters))
            {
                filter = new etPeriodFilter(filters);
            }

            using (hr2017DataContext context = new hr2017DataContext())
            {                

                etPeriod data = new etPeriod(employee.EmployeeId, pm.StartDate, pm.EndDate, null, filter: filter);

                string templateName = FixedSettings.DTRType;
                serverPath = Server.MapPath("~/");

                objData CustomData = new objData
                {
                    ApplicationName = MvcApplication.ApplicationLicense.Info.ApplicationName,
                    ModuleName = "HR",
                    ReportName = templateName
                };

                string appPath = serverPath;

                return new asposeWordsTemplateReport(CustomizeDoc_Aspose, (FixedSettings.DTRHeader && !DTR.IsDTRByCutOff ? new ReportHeaderParams(appPath) { ReportLogo_Width = 40, ReportLogo_Height = 40 } : null), FixedSettings.ApplicationName, FixedSettings.Owner,
                    new ReportFooterParams(appPath), CustomData)
                    .Get(CustomData.ReportName, employee.LastName, data, false);

            }
        }

        public void CustomizeDoc_Aspose(object data, ref Aspose.Words.Document wordDoc)
        {
            doSet(data, ref wordDoc);

            if (DTR.IsTwinDTR)
            {
                doSet(data, ref wordDoc, "_a");
            }

        }

        private void doSet(object data, ref Aspose.Words.Document wordDoc, string bookmarkSuffix = "")
        {
            etPeriod period = (etPeriod)data;
            etData d = period.data;

            string mfo = "";
            string office = "";
            string position = "";

            if (d.Career != null)
            {
                if (d.Career.MFO != null) mfo = d.Career.MFO.Description;
                if (d.Career.Office != null) office = d.Career.Office.Office;
                if (d.Career.Position != null) position = d.Career.Position.Position;
            }


            Aspose.Words.Bookmark bookmark;
            Aspose.Words.Tables.Row r;
            
            using (hr2017DataContext context = new hr2017DataContext())
            {
                if (DTR.IsDTRByCutOff)
                {
                    string[] fields = new string[] {
                        "employee", "mfo", "office", "position", "period"
                    };

                    string[] fieldValues = new string[] {
                        d.Employee.FullName_FN.ToUpper(),
                        mfo,
                        office,
                        position,
                        coreLib.Procs.friendlyPeriod(period.startDate, period.endDate, true)
                    };

                    Procs.ApplySignatories("hr", FixedSettings.DTRType, ref fields, ref fieldValues);

                    wordDoc.MailMerge.Execute(fields, fieldValues);

                    bookmark = wordDoc.Range.Bookmarks["tableRef"];
                    Node _t = bookmark.BookmarkStart.GetAncestor(NodeType.Table);

                    Aspose.Words.Tables.Table t = (Aspose.Words.Tables.Table)_t;

                    r = null;
                    Aspose.Words.Tables.Row row = (Aspose.Words.Tables.Row)t.Rows[t.Rows.Count - 1].Clone(true);

                    double total_lateUT = 0;
                    double onduty_count = 0;

                    for (int i = 1; i <= period.Days.Count; i++)
                    {
                        etDay e = period.Days[i - 1];
                        int dutyeq = e.Fld.Duty_Value > 0 ? 1 : 0;

                        if (i > 1)
                        {
                            t.Rows.Add(row.Clone(true));
                        }

                        r = t.Rows[t.Rows.Count - 1];
                        
                        Run run;

                        //DATE
                        r.Cells[0].FirstParagraph.AppendChild(new Run(wordDoc, e.d.ToString("MM/dd/yy")));

                        //DAY
                        r.Cells[1].FirstParagraph.AppendChild(new Run(wordDoc, e.d.ToString("ddd")));

                        bool inOk = false;
                        List<etTime> times = e.Times.Where(x => !x.IsDummy).ToList();

                        if (e.SkipAttendanceDetails)
                        {
                            if (e.OnTravel)
                            {
                                dutyeq = 1;
                            }

                            r.Cells[2].FirstParagraph.AppendChild(new Run(wordDoc, ""));
                            r.Cells[3].FirstParagraph.AppendChild(new Run(wordDoc, ""));

                            etTime logIn = times.Where(x => x.Log.Actual_TimeIn != coreLib.Constants.DEFAULT_DATETIME).OrderByDescending(x => x.LoggedInToday).ThenBy(x => x.Log.Actual_TimeIn).FirstOrDefault();
                            if (logIn != null)
                            {
                                string tin = logIn.Log.Actual_TimeIn.ToString("h:mm tt");
                                run = new Run(wordDoc, tin);
                                run.Font.Color = Color.Red;
                                r.Cells[4].FirstParagraph.AppendChild(run);

                                inOk = true;
                            }
                            else
                            {
                                r.Cells[4].FirstParagraph.AppendChild(new Run(wordDoc, ""));
                            }

                            etTime logOut = times.Where(x => x.Log.Actual_TimeOut != coreLib.Constants.DEFAULT_DATETIME).OrderBy(x => x.LoggedInToday).ThenBy(x => x.Log.Actual_TimeOut).LastOrDefault();
                            if (logOut != null)
                            {
                                if (!inOk)
                                {
                                    string tin = logOut.Log.Actual_TimeIn == coreLib.Constants.DEFAULT_DATETIME ? "" : logOut.Log.Actual_TimeIn.ToString("h:mm tt");
                                    run = new Run(wordDoc, tin);
                                    run.Font.Color = Color.Red;
                                    r.Cells[4].FirstParagraph.AppendChild(run);
                                }

                                string tout = logOut.Log.Actual_TimeOut.ToString("h:mm tt");
                                run = new Run(wordDoc, tout);
                                run.Font.Color = Color.Red;
                                r.Cells[5].FirstParagraph.AppendChild(run);
                            }
                            else
                            {
                                r.Cells[5].FirstParagraph.AppendChild(new Run(wordDoc, ""));
                            }
                            
                            r.Cells[6].FirstParagraph.AppendChild(new Run(wordDoc, ""));
                        
                        }
                        else
                        {
                            DateTime onDuty = times.OrderByDescending(x => x.segment.TimeInFrom).ThenBy(x => x.segment.TimeIn).First().segment.TimeIn;
                            DateTime offDuty = times.OrderBy(x => x.segment.TimeInFrom).OrderBy(x => x.segment.TimeOut).Last().segment.TimeOut;

                            r.Cells[2].FirstParagraph.AppendChild(new Run(wordDoc, onDuty.ToString("h:mm tt")));
                            r.Cells[3].FirstParagraph.AppendChild(new Run(wordDoc, offDuty.ToString("h:mm tt")));
                                                        
                            etTime logIn = times.Where(x => x.Log.Display_TimeIn_Value != coreLib.Constants.DEFAULT_DATETIME).OrderByDescending(x => x.LoggedInToday).ThenBy(x => x.Log.Display_TimeIn_Value).FirstOrDefault();
                            if (logIn != null)
                            {
                                if (!logIn.IsAbsent)
                                {
                                    string tin = logIn.Log.Display_TimeIn_Value == coreLib.Constants.DEFAULT_DATETIME ? "" : logIn.Log.Display_TimeIn_Value.ToString("h:mm tt") + (logIn.Log.autoLogin ? "*" : "");
                                    run = new Run(wordDoc, tin);
                                    run.Font.Color = logIn.IsAbsent ? Color.Red : Color.Black;
                                    r.Cells[4].FirstParagraph.AppendChild(run);

                                    inOk = true;
                                }                                
                            }
                            
                            etTime logOut = times.Where(x => x.Log.Display_TimeOut_Value != coreLib.Constants.DEFAULT_DATETIME).OrderBy(x => x.LoggedInToday).ThenBy(x => x.Log.Display_TimeOut_Value).LastOrDefault();
                            if (logOut != null)
                            {
                                if (!logOut.IsAbsent)
                                {
                                    if (!inOk)
                                    {
                                        string tin = logOut.Log.Display_TimeIn_Value == coreLib.Constants.DEFAULT_DATETIME ? "" : logOut.Log.Display_TimeIn_Value.ToString("h:mm tt") + (logOut.Log.autoLogin ? "*" : "");
                                        run = new Run(wordDoc, tin);
                                        run.Font.Color = logOut.IsAbsent ? Color.Red : Color.Black;
                                        r.Cells[4].FirstParagraph.AppendChild(run);
                                    }

                                    string tout = logOut.Log.Display_TimeOut_Value == coreLib.Constants.DEFAULT_DATETIME ? "" : logOut.Log.Display_TimeOut_Value.ToString("h:mm tt") + (logOut.Log.autoLogout ? "*" : "");
                                    run = new Run(wordDoc, tout);
                                    run.Font.Color = logOut.IsAbsent ? Color.Red : Color.Black;
                                    r.Cells[5].FirstParagraph.AppendChild(run);
                                }
                            }

                            //LATE/UT
                            double lateUT = e.LateUT.TotalHoursLate + e.LateUT.TotalHoursUT;
                            total_lateUT += lateUT;                            
                            r.Cells[6].FirstParagraph.AppendChild(new Run(wordDoc, lateUT <= 0 ? "" : lateUT.ToString("#,##0.000")));
                     
                        }

                        onduty_count += dutyeq;

                        //ON DUTY
                        r.Cells[7].FirstParagraph.AppendChild(new Run(wordDoc, dutyeq == 1 ? "1" : ""));

                        //REMARKS
                        string remarks = DayRemarks(e);
                        if (e.LateUT.HasLate_TRF || e.LateUT.HasUT_TR)
                        {
                            remarks += (remarks == "" ? "" : Environment.NewLine) + LogSuffix(e.LateUT);
                        }

                        r.Cells[8].FirstParagraph.AppendChild(new Run(wordDoc, remarks));
                    }

                    //Total Row *******************************************************************************************************

                    t.Rows.Add(row.Clone(true));
                    r = t.Rows[t.Rows.Count - 1];

                    r.Cells[6].FirstParagraph.AppendChild(new Run(wordDoc, total_lateUT <= 0 ? "" : total_lateUT.ToString("#,##0.000")));
                    r.Cells[7].FirstParagraph.AppendChild(new Run(wordDoc, onduty_count.ToString()));

                    t.GetChildNodes(NodeType.Run, true).ToArray().ToList().ForEach(x => ((Run)x).Font.Size = 8);
                }
                else if (DTR.IsDefault || DTR.IsTwinDTR)
                {
                    if (!FixedSettings.DTRHeader)
                    {
                        //******************************************
                        //insert logo
                        //******************************************            
                        if (wordDoc.Range.Bookmarks["companyLogo" + bookmarkSuffix] != null)
                        {
                            ReportHeaderParams ReportParams = new ReportHeaderParams(serverPath) { ReportLogo_Width = 40, ReportLogo_Height = 40 };
                            DocumentBuilder builder = new DocumentBuilder(wordDoc);
                            builder.MoveToBookmark("companyLogo" + bookmarkSuffix);
                            builder.InsertImage(ReportParams.ReportLogoPath, ReportParams.ReportLogo_Width, ReportParams.ReportLogo_Height);
                        }
                        //******************************************
                    }

                    string[] fields = new string[] {
                        "employee", "office", "position", "month"
                    };

                    string[] fieldValues = new string[] {
                        d.Employee.FullName_FN.ToUpper(),
                        office,
                        position,
                        string.Format("{0} {1}", period.startDate.ToString("MMMM"), period.startDate.Year),

                    };

                    Procs.ApplySignatories("hr", FixedSettings.DTRType, ref fields, ref fieldValues);

                    wordDoc.MailMerge.Execute(fields, fieldValues);

                    int i = 1;

                    //schedule
                    bookmark = wordDoc.Range.Bookmarks["schedule"];
                    if (bookmark != null)
                    {
                        Node _t = bookmark.BookmarkStart.GetAncestor(NodeType.Table);

                        Aspose.Words.Tables.Table t = (Aspose.Words.Tables.Table)_t;
                        Aspose.Words.Tables.Row row = (Aspose.Words.Tables.Row)t.Rows[t.Rows.Count - 1].Clone(true);
                        
                        using (Module.DB.dalDataContext _context = new dalDataContext())
                        {
                            foreach (etScheduleModel sm in period.GetActiveSchedules())
                            {
                                if (i > 1)
                                {
                                    t.Rows.Add(row.Clone(true));
                                }

                                r = t.Rows[t.Rows.Count - 1];

                                r.Cells[0].FirstParagraph.AppendChild(new Run(wordDoc, sm.schedule.Description));

                                string seg = string.Join(", ", sm.segments.Select(x => segmentDesc(x, d.settings.WorkHoursPerDay)));
                                r.Cells[2].FirstParagraph.AppendChild(new Run(wordDoc, seg));

                                i++;
                            }
                        }
                    }

                    //days table
                    int month = period.startDate.Month;

                    DateTime currentLog = coreLib.Constants.DEFAULT_DATETIME;
                    double totalminsLateUndertime = 0;

                    for (i = 1; i <= 31; i++)
                    {

                        bookmark = wordDoc.Range.Bookmarks["day" + i + bookmarkSuffix];
                        if (bookmark == null) continue;

                        r = (Aspose.Words.Tables.Row)bookmark.BookmarkStart.GetAncestor(NodeType.Row);

                        //set date
                        r.Cells[0].FirstParagraph.AppendChild(new Run(wordDoc, i.ToString()));

                        etDay day = period.Days.Where(x => x.d.Month == month && x.d.Day == i && x.IsWorkDay).SingleOrDefault();
                        if (day != null)
                        {
                            etTime timeAMIn, timeAMOut, timePMIn, timePMOut;
                            List<etTime> times = day.wiReg.Times.Where(x => !x.IsDummy).ToList();

                            if (day.IsWorkSpan)
                            {
                                timeAMIn = times.Where(x => x.Log.Actual_TimeIn > currentLog && x.Log.Actual_TimeIn.Hour < 13).OrderByDescending(x => x.LoggedInToday).ThenBy(x => x.Log.Actual_TimeIn).FirstOrDefault();
                                if (timeAMIn != null)
                                {
                                    writeToCell(ref wordDoc, ref r, ref currentLog, 1, timeAMIn.Log.Actual_TimeIn, Color.Black);
                                }

                                timeAMOut = times.Where(x => x.Log.Actual_TimeOut > currentLog && x.Log.Actual_TimeOut.Hour < 13).OrderBy(x => x.LoggedInToday).ThenBy(x => x.Log.Actual_TimeOut).LastOrDefault();
                                if (timeAMOut != null)
                                {
                                    writeToCell(ref wordDoc, ref r, ref currentLog, 2, timeAMOut.Log.Actual_TimeOut, Color.Black);
                                }

                                timePMIn = times.Where(x => x.Log.Actual_TimeIn > currentLog && x.Log.Actual_TimeIn.Hour > 13).OrderByDescending(x => x.LoggedInToday).ThenBy(x => x.Log.Actual_TimeIn).FirstOrDefault();
                                if (timePMIn != null)
                                {
                                    writeToCell(ref wordDoc, ref r, ref currentLog, 3, timePMIn.Log.Actual_TimeIn, Color.Black);
                                }

                                timePMOut = times.Where(x => x.Log.Actual_TimeOut > currentLog && x.Log.Actual_TimeOut.Hour > 13).OrderBy(x => x.LoggedInToday).ThenBy(x => x.Log.Actual_TimeOut).LastOrDefault();
                                if (timePMOut != null)
                                {
                                    writeToCell(ref wordDoc, ref r, ref currentLog, 4, timePMOut.Log.Actual_TimeOut, Color.Black);
                                }

                                continue;
                            }

                            if (day.SkipAttendanceDetails)
                            {
                                timeAMIn = times.Where(x => x.Segment_IsAM && x.Log.Actual_TimeIn > currentLog).OrderByDescending(x => x.LoggedInToday).ThenBy(x => x.Log.Actual_TimeIn).FirstOrDefault();
                                if (timeAMIn != null)
                                {
                                    writeToCell(ref wordDoc, ref r, ref currentLog, 1, timeAMIn.Log.Actual_TimeIn, Color.Red);
                                }

                                timeAMOut = times.Where(x => x.Segment_IsAM && x.Log.Actual_TimeOut > currentLog && x.Log.Actual_TimeOut.Hour < 13).OrderBy(x => x.LoggedInToday).ThenBy(x => x.Log.Actual_TimeOut).LastOrDefault();
                                if (timeAMOut != null)
                                {
                                    writeToCell(ref wordDoc, ref r, ref currentLog, 2, timeAMOut.Log.Actual_TimeOut, Color.Red);
                                }
                                else
                                {
                                    timeAMOut = times.Where(x => x.Segment_IsAM && x.Log.Actual_TimeOut > currentLog).LastOrDefault();
                                    if (timeAMOut != null)
                                    {
                                        if (day.Times.Any(x => x.Log.Actual_TimeIn > timeAMOut.Log.Actual_TimeOut || x.Log.Actual_TimeOut > timeAMOut.Log.Actual_TimeOut))
                                        {
                                            writeToCell(ref wordDoc, ref r, ref currentLog, 2, timeAMOut.Log.Actual_TimeOut, Color.Red);
                                        }
                                    }
                                }

                                timePMIn = times.Where(x => x.Segment_IsPM && x.Log.Actual_TimeIn > currentLog).OrderByDescending(x => x.LoggedInToday).ThenBy(x => x.Log.Actual_TimeIn).FirstOrDefault();
                                if (timePMIn != null)
                                {
                                    writeToCell(ref wordDoc, ref r, ref currentLog, 3, timePMIn.Log.Actual_TimeIn, Color.Red);
                                }

                                timePMOut = times.Where(x => x.Segment_IsPM && x.Log.Actual_TimeOut > currentLog).OrderBy(x => x.LoggedInToday).ThenBy(x => x.Log.Actual_TimeOut).LastOrDefault();
                                if (timePMOut != null)
                                {
                                    writeToCell(ref wordDoc, ref r, ref currentLog, 4, timePMOut.Log.Actual_TimeOut, Color.Red);
                                }

                                continue;
                            }

                            
                            
                            timeAMIn = times.Where(x => x.Segment_IsAM && x.Log.Display_TimeIn_Value > currentLog && (!x.IsAbsent || !x.Log.autoLogin)).OrderByDescending(x => x.LoggedInToday).ThenBy(x => x.Log.Display_TimeIn_Value).FirstOrDefault();
                            if (timeAMIn != null)
                            {
                                writeToCell(ref wordDoc, ref r, ref currentLog, 1, timeAMIn.Log.Display_TimeIn_Value, timeAMIn.IsAbsent ? Color.Red : Color.Black, timeAMIn.Log.autoLogin ? "*" : "");
                            }

                            timeAMOut = times.Where(x => x.Segment_IsAM && x.Log.Display_TimeOut_Value > currentLog && (!x.IsAbsent || !x.Log.autoLogout) && x.Log.Display_TimeOut_Value.Hour < 13).OrderBy(x => x.LoggedInToday).ThenBy(x => x.Log.Display_TimeOut_Value).LastOrDefault();
                            if (timeAMOut != null)
                            {
                                writeToCell(ref wordDoc, ref r, ref currentLog, 2, timeAMOut.Log.Display_TimeOut_Value, timeAMOut.IsAbsent ? Color.Red : Color.Black, timeAMOut.Log.autoLogout ? "*" : "");
                            }
                            else
                            {
                                timeAMOut = times.Where(x => x.Segment_IsAM && x.Log.Display_TimeOut_Value > currentLog && (!x.IsAbsent || !x.Log.autoLogout)).LastOrDefault();                                
                                if (timeAMOut != null)
                                {
                                    if (day.Times.Any(x => x.Log.Actual_TimeIn > timeAMOut.Log.Display_TimeOut_Value || x.Log.Actual_TimeOut > timeAMOut.Log.Display_TimeOut_Value))
                                    {
                                        writeToCell(ref wordDoc, ref r, ref currentLog, 2, timeAMOut.Log.Display_TimeOut_Value, timeAMOut.IsAbsent ? Color.Red : Color.Black, timeAMOut.Log.autoLogout ? "*" : "");                                        
                                    }
                                }
                            }

                            timePMIn = times.Where(x => x.Segment_IsPM && x.Log.Display_TimeIn_Value > currentLog && (!x.IsAbsent || !x.Log.autoLogin)).OrderByDescending(x => x.LoggedInToday).ThenBy(x => x.Log.Display_TimeIn_Value).FirstOrDefault();
                            if (timePMIn != null)
                            {
                                writeToCell(ref wordDoc, ref r, ref currentLog, 3, timePMIn.Log.Display_TimeIn_Value, timePMIn.IsAbsent ? Color.Red : Color.Black, timePMIn.Log.autoLogin ? "*" : "");
                            }

                            timePMOut = times.Where(x => x.Segment_IsPM && x.Log.Display_TimeOut_Value > currentLog && (!x.IsAbsent || !x.Log.autoLogout)).OrderBy(x => x.LoggedInToday).ThenBy(x => x.Log.Display_TimeOut_Value).LastOrDefault();
                            if (timePMOut != null)
                            {
                                writeToCell(ref wordDoc, ref r, ref currentLog, 4, timePMOut.Log.Display_TimeOut_Value, timePMOut.IsAbsent ? Color.Red : Color.Black, timePMOut.Log.autoLogout ? "*" : "");
                            }

                            if (day.ReportedForDuty)
                            {
                                if (DTR.IsDefault && day.wiOT.ReportedForDuty)
                                {
                                    List<etTime> otTimes = day.wiOT.Reported_Times.Where(x => !x.IsDummy).OrderByDescending(x => x.LoggedInToday).ThenBy(x => x.Log.Display_TimeIn_Value).ToList();

                                    DateTime login = day.wiReg.ReportedForDuty ? otTimes.First().Log.Computed_TimeIn : otTimes.First().Log.Display_TimeIn_Value;
                                    DateTime logout = otTimes.Last().Log.Display_TimeOut_Value;

                                    r.Cells[5].FirstParagraph.AppendChild(new Run(wordDoc, login.ToString("h:mm tt")));
                                    r.Cells[6].FirstParagraph.AppendChild(new Run(wordDoc, logout.ToString("h:mm tt")));
                                }
                                else if (DTR.IsTwinDTR)
                                {
                                    double minsLateUndertime = day.LateUT.TotalMinsLate + day.LateUT.TotalMinsUT;

                                    if (minsLateUndertime > 0)
                                    {
                                        double hrs = Math.Floor(minsLateUndertime / 60);
                                        double mins = minsLateUndertime % 60;

                                        if (hrs > 0)
                                        {
                                            r.Cells[5].FirstParagraph.AppendChild(new Run(wordDoc, hrs.ToString("#,###")));
                                        }

                                        if (mins > 0)
                                        {
                                            r.Cells[6].FirstParagraph.AppendChild(new Run(wordDoc, mins.ToString("##")));
                                        }

                                        totalminsLateUndertime += minsLateUndertime;
                                    }
                                }
                            }
                        }

                    }

                    if (totalminsLateUndertime > 0)
                    {
                        bookmark = wordDoc.Range.Bookmarks["totalhr" + bookmarkSuffix];
                        if (bookmark != null)
                        {
                            r = (Aspose.Words.Tables.Row)bookmark.BookmarkStart.GetAncestor(NodeType.Row);

                            double hrs = Math.Floor(totalminsLateUndertime / 60);
                            double mins = totalminsLateUndertime % 60;

                            if (hrs > 0)
                            {
                                r.Cells[2].FirstParagraph.AppendChild(new Run(wordDoc, hrs.ToString("#,###")));
                            }

                            if (mins > 0)
                            {
                                r.Cells[3].FirstParagraph.AppendChild(new Run(wordDoc, mins.ToString("##")));
                            }
                        }
                    }
                }
            }

            wordDoc.GetChildNodes(NodeType.Run, true).ToArray().ToList().ForEach(x => ((Run)x).Font.Size = 9);
        }

        void writeToCell(ref Document wordDoc, ref Aspose.Words.Tables.Row r, ref DateTime currentLog, int cellIndex, DateTime t, Color color, string autoLog = "")
        {
            string s = t == coreLib.Constants.DEFAULT_DATETIME ? "" : t.ToString("h:mm") + autoLog;
            Run run = new Run(wordDoc, s);
            run.Font.Color = color;
            r.Cells[cellIndex].FirstParagraph.AppendChild(run);
            currentLog = t;
        }

        string segmentDesc(ISegment x, double workHoursPerDay)
        {
            string ret = "";

            if (x.IsWorkSpan)
            {
                ret = string.Format("{0} ({1} hours)", x.SegmentType_Desc, workHoursPerDay);
            }
            else
            {
                ret = string.Format("{0} ({1} - {2})", x.Description, x.TimeIn.ToString("h:mm tt"), x.TimeOut.ToString("h:mm tt"));
            }
            return ret;
        }

        string LogSuffix(lateUTDetails lateUT)
        {
            string ret = "";

            List<string> tmp = new List<string>();

            if (lateUT.HasLateTolerance)
            {
                tmp.Add("lt");
            }
            else
            {
                if (lateUT.HasLateRoundOff)
                {
                    tmp.Add("lr");
                }

                if (lateUT.HasFlexiTime)
                {
                    tmp.Add("f");
                }
            }

            if (lateUT.HasUTTolerance)
            {
                tmp.Add("ut");
            }
            else
            {
                if (lateUT.HasUTRoundOff)
                {
                    tmp.Add("ur");
                }
            }

            if (tmp.Any())
            {
                ret = string.Format("[{0}]", string.Join("/", tmp));
            }

            return ret;
        }

        string DayRemarks(etDay day)
        {
            string ret = "";

            string woPay = day.Leave_WithoutPay ? " w/o Pay" : "";

            if (day.IsHoliday)
            {
                ret += (ret == "" ? "" : Environment.NewLine) + day.Holidays_Desc;
            }

            if (day.IsRestDay)
            {
                ret += (ret == "" ? "" : Environment.NewLine) + "Rest Day";
            }

            if (!day.IsWorkDay)
            {
                ret += (ret == "" ? "" : Environment.NewLine) + "No Schedule";
            }

            if (day.NoWorkCredit)
            {
                ret += (ret == "" ? "" : Environment.NewLine) + "No Work Credit";
            }

            if (day.OnTravel)
            {
                ret += (ret == "" ? "" : Environment.NewLine) + "On Travel";
            }

            if (day.IsAbsent_WholeDay)
            {
                if (!day.Faculty40_Saturday_Absence)
                {
                    ret += (ret == "" ? "" : Environment.NewLine) + "Absent";
                }
            }

            if (day.IsAbsent_HalfDay)
            {
                if (!day.Faculty40_Saturday_Absence)
                {
                    ret += (ret == "" ? "" : Environment.NewLine) + "Half - Day Absent";
                }
            }

            if (day.OnWholeDayLeave)
            {
                ret += (ret == "" ? "" : Environment.NewLine) + "On Leave" + (woPay == "" ? "" : " (woPay)");
            }

            if (day.OnHalfDayLeave)
            {
                ret += (ret == "" ? "" : Environment.NewLine) + "On Half - Day Leave" + (woPay == "" ? "" : " (woPay)");
            }

            return ret;
        }

        //public void IterateFields(Field field, string reportType, object data, ref Application wordApp)
        //{
        //    etPeriod d = (etPeriod)data;

        //    switch (reports.Common.getFieldName(field))
        //    {
        //        case "employee":
        //            reports.Common.setField(d.data.Employee.FullName_FN.ToUpper(), field, ref wordApp);
        //            break;
        //        case "month":
        //            reports.Common.setField(string.Format("{0} {1}", d.startDate.ToString("MMMM"), d.startDate.Year), field, ref wordApp);
        //            break;
        //    }
        //}

        //public void CustomizeDoc(object data, ref Application wordApp, ref Microsoft.Office.Interop.Word.Document wordDoc)
        //{
        //    etPeriod d = (etPeriod)data;
        //    int month = d.startDate.Month;

        //    DateTime currentLog = coreLib.Constants.DEFAULT_DATETIME;
        //    double totalminsLateUndertime = 0;

        //    Selection selection = wordApp.Selection;

        //    for (int i = 1; i <= 31; i++)
        //    {
        //        wordDoc.Bookmarks["day" + i.ToString()].Select();

        //        //set date
        //        selection.Text = i.ToString();

        //        etDay day = d.Days.Where(x => x.d.Month == month && x.d.Day == i && x.IsRegularWorkDay && !x.IsAbsent).SingleOrDefault();
        //        if (day != null)
        //        {
        //            selection.Move(WdUnits.wdCell, 1);

        //            etTime timeAMIn = day.Times.Where(x => x.Log.Display_TimeIn_Value > currentLog && x.Log.Display_TimeIn_Value.Hour < 12 && !x.IsAbsent).FirstOrDefault();
        //            if (timeAMIn != null)
        //            {
        //                //set AM Arrival
        //                selection.Text = timeAMIn.Log.Display_TimeIn_Value.ToString("h:mm") + (timeAMIn.Log.autoLogin ? "*" : "");

        //                currentLog = timeAMIn.Log.Display_TimeIn_Value;
        //            }

        //            selection.Move(WdUnits.wdCell, 1);

        //            etTime timeAMOut = day.Times.Where(x => x.Log.Display_TimeOut_Value > currentLog && x.Log.Display_TimeOut_Value.Hour < 13 && !x.IsAbsent).FirstOrDefault();
        //            if (timeAMOut != null)
        //            {
        //                //set AM Departure
        //                selection.Text = timeAMIn.Log.Display_TimeOut_Value.ToString("h:mm") + (timeAMOut.Log.autoLogout ? "*" : "");

        //                currentLog = timeAMIn.Log.Display_TimeOut_Value;
        //            }

        //            selection.Move(WdUnits.wdCell, 1);

        //            etTime timePMIn = day.Times.Where(x => x.Log.Display_TimeIn_Value > currentLog && x.Log.Display_TimeIn_Value.Hour > 12 && !x.IsAbsent).FirstOrDefault();
        //            if (timePMIn != null)
        //            {
        //                //set PM Arrival
        //                selection.Text = timePMIn.Log.Display_TimeIn_Value.ToString("h:mm") + (timePMIn.Log.autoLogin ? "*" : "");

        //                currentLog = timePMIn.Log.Display_TimeIn_Value;
        //            }

        //            selection.Move(WdUnits.wdCell, 1);

        //            etTime timePMOut = day.Times.Where(x => x.Log.Display_TimeOut_Value > currentLog && !x.IsAbsent).FirstOrDefault();
        //            if (timePMOut != null)
        //            {                        
        //                //set PM Departure
        //                selection.Text = timePMOut.Log.Display_TimeOut_Value.ToString("h:mm") + (timePMOut.Log.autoLogout ? "*" : "");

        //                currentLog = timePMOut.Log.Display_TimeOut_Value;
        //            }

        //            selection.Move(WdUnits.wdCell, 1);

        //            double minsLateUndertime = day.TotalMinsLate + day.TotalMinsUndertime;

        //            if (minsLateUndertime > 0)
        //            {
        //                double hrs = Math.Floor(minsLateUndertime / 60);
        //                double mins = minsLateUndertime % 60;

        //                if (hrs > 0)
        //                {
        //                    selection.Text = hrs.ToString("#,###");
        //                }

        //                selection.Move(WdUnits.wdCell, 1);

        //                if (mins > 0)
        //                {
        //                    selection.Text = mins.ToString("##");
        //                }

        //                totalminsLateUndertime += minsLateUndertime;
        //            }
        //        }
        //    }

        //    if (totalminsLateUndertime > 0)
        //    {
        //        wordDoc.Bookmarks["totalhr"].Select();

        //        double hrs = Math.Floor(totalminsLateUndertime / 60);
        //        double mins = totalminsLateUndertime % 60;

        //        if (hrs > 0)
        //        {
        //            selection.Text = hrs.ToString("#,###");
        //        }

        //        selection.Move(WdUnits.wdCell, 1);

        //        if (mins > 0)
        //        {
        //            selection.Text = mins.ToString("##");
        //        }
        //    }
        //}
    }
}