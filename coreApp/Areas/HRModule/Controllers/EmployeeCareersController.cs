using System.Linq;
using System.Web.Mvc;
using System;
using coreApp.DAL;
using coreApp.Filters;
using coreApp.Controllers;
using coreApp.Interfaces;
using coreLib.Objects;
using reports;
using System.Collections.Generic;
using coreLib.Extensions;
using coreApp.Enums;
using coreApp.Areas.TimeModule.Time;
using reports.AsposeLib;
using Aspose.Words;
using Module.DB;
using Module.DB.Enums;
using Module.Core;
using Module.Time;
using System.Data.Linq;

namespace coreApp.Areas.HRModule.Controllers
{
    [EmployeeInfoFilter]
    [EmployeeAuthorize]
    [UserAccessAuthorize(allowedAccess: "hr_emp_career")]
    public class EmployeeCareersController : HLBase_AuthorizedController, IEmployeeController
    {
        public tblEmployee employee { get; set; }

        public EmployeeCareersController()
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

                var model = context.tblEmployee_Careers.Where(x => x.EmployeeId == employee.EmployeeId).OrderBy(x => x.DateEffective).ToList();
                return View(model);
            }
        }

        public ActionResult _Details(int? id)
        {
            ViewBag.Title = "Career";
            

            ViewBag.uiIsReadOnly = true;
            ViewBag.uiIncludeId = true;

            using (dalDataContext context = new dalDataContext())
            {
                tblEmployee_Career Career = context.tblEmployee_Careers.Where(x => x.EmployeeId == employee.EmployeeId && x.CareerId == id).SingleOrDefault();
                if (Career == null)
                {
                    return new Raw_ActionResult(ModuleConstants.ID_NOT_FOUND);
                }
                else
                {

                    return PartialView("_Career", Career);
                }
            }
        }

        public ActionResult _Create()
        {
            ViewBag.Title = "Career";
            

            ViewBag.uiIsReadOnly = false;
            ViewBag.uiIncludeId = false;

            tblEmployee_Career model = new tblEmployee_Career
            {
                EmployeeId = employee.EmployeeId,
                DateEffective = DateTime.Now,
                DepartmentId = -1,
                PositionId = -1,
                PositionLevel = 1,
                Events = "",
                EmploymentType = (int)Module.DB.Enums.EmploymentType.Permanent
            };

            return PartialView("_Career", model);
        }

        public void _CreatePost(FormCollection _model, ref queryResult res)
        {
            tblEmployee_Career model = (tblEmployee_Career)UpdateModel(new tblEmployee_Career().GetType(), _model);
            model.Events = _model["EventSelection"] == null ? null : _model["EventSelection"].ToString();

            using (dalDataContext context = new dalDataContext())
            {
                if (model._Department == null)
                {
                    AddError("The Department field is required");
                }

                if (model.hasEndEvent())
                {
                    if (model.EndEventDate == null)
                    {
                        AddError("No End Event Date specified");
                    }
                    else if (model.EndEventDate < model.DateEffective)
                    {
                        AddError("End Event Date cannot be earlier than Date Effective");
                    }
                }
                
                tblEmployee_Career lastCareer = context.tblEmployee_Careers.Where(x => x.EmployeeId == model.EmployeeId).OrderByDescending(x => x.DateEffective).FirstOrDefault();
                if (lastCareer != null)
                {
                    if (!lastCareer.hasEndEvent())
                    {
                        AddError("Cannot add new career. Previous career hasn't ended yet");
                    }
                    else if (model.DateEffective <= lastCareer.EndEventDate)
                    {
                        AddError("Date Effective cannot be earlier or equal to the last career's End Date");
                    }
                }

                if (ModelState.IsValid)
                {
                    tblEmployee_Career Career = new tblEmployee_Career
                    {
                        EmployeeId = model.EmployeeId,
                        DateEffective = model.DateEffective,
                        DepartmentId = model.DepartmentId,
                        PositionId = model.PositionId,
                        PositionLevel = model.PositionLevel,
                        CustomRate = model.CustomRate,
                        CustomRate_IsDailyRate = model.CustomRate_IsDailyRate,
                        Events = model.Events,
                        EventRemarks = string.IsNullOrEmpty(model.EventRemarks) ? "" : model.EventRemarks,
                        EmploymentType = model.EmploymentType,
                        IsDesignated = model.IsDesignated,
                        EndEvent = model.EndEvent,
                        EndEventDate = model.EndEventDate,
                        EndEventRemarks = string.IsNullOrEmpty(model.EndEventRemarks) ? "" : model.EndEventRemarks,
                        SalaryGrade = model.SalaryGrade,
                        Fund = model.Fund
                    };
                    
                    if (FixedSettings.UseMFO)
                    {
                        Career.MFOId = model.MFOId;
                    }

                    context.tblEmployee_Careers.InsertOnSubmit(Career);
                    context.SubmitChanges();

                    Cache.Update_Tables(_employee_careers: true);

                    res.Remarks = "Record was successfully created";

                    MvcApplication.HLLog.InsertLog("Employee Career", Career.CareerId, employee.FullName, Career);

                }
                else
                {
                    throw new Exception(coreProcs.ShowErrors(ModelState));
                }
            }
        }

        public ActionResult _Edit(int? id)
        {
            ViewBag.Title = "Career";
            

            ViewBag.uiIsReadOnly = false;
            ViewBag.uiIncludeId = true;

            using (dalDataContext context = new dalDataContext())
            {
                tblEmployee_Career Career = context.tblEmployee_Careers.Where(x => x.EmployeeId == employee.EmployeeId && x.CareerId == id).Single();
                if (Career == null)
                {
                    return new Raw_ActionResult(ModuleConstants.ID_NOT_FOUND);
                }
                else
                {
                    return PartialView("_Career", Career);
                }
            }
        }


        public void _EditPost(FormCollection _model, ref queryResult res)
        {
            tblEmployee_Career model = (tblEmployee_Career)UpdateModel(new tblEmployee_Career().GetType(), _model);
            model.Events = _model["EventSelection"] == null ? null : _model["EventSelection"].ToString();

            using (dalDataContext context = new dalDataContext())
            {
                tblEmployee_Career Career = context.tblEmployee_Careers.Where(x => x.CareerId == model.CareerId).SingleOrDefault();
                if (Career == null)
                {
                    throw new Exception(ModuleConstants.ID_NOT_FOUND);
                }
                else
                {
                    if (model._Department == null)
                    {
                        AddError("The Department field is required");
                    }

                    if (model.hasEndEvent())
                    {
                        if (model.EndEventDate == null)
                        {
                            AddError("No End Event Date specified");
                        }
                        else if (model.EndEventDate < model.DateEffective)
                        {
                            AddError("End Event Date cannot be earlier than Date Effective");
                        }
                    }
                    else
                    {
                        if (model.EndEventDate != null)
                        {
                            AddError("No End Event specified");
                        }
                    }

                    tblEmployee_Career prevCareer = context.tblEmployee_Careers.Where(x => x.EmployeeId == model.EmployeeId && x.DateEffective < Career.DateEffective).OrderByDescending(x => x.DateEffective).FirstOrDefault();
                    if (prevCareer != null)
                    {
                        if (model.DateEffective <= prevCareer.EndEventDate)
                        {
                            AddError("Date Effective cannot be earlier or equal to the last career's End Date");
                        }
                    }

                    tblEmployee_Career nextCareer = context.tblEmployee_Careers.Where(x => x.EmployeeId == model.EmployeeId && x.DateEffective > Career.DateEffective).OrderBy(x => x.DateEffective).FirstOrDefault();
                    if (nextCareer != null)
                    {
                        if (!model.hasEndEvent())
                        {
                            AddError("Cannot remove end event. A succeeding career exists");
                        }
                        else if (model.EndEventDate >= nextCareer.DateEffective)
                        {
                            AddError("End Event Date cannot be later than or equal to the succeeding career's date of effectivity");
                        }
                    }

                    if (ModelState.IsValid)
                    {
                        Career.EmployeeId = model.EmployeeId;
                        Career.DateEffective = model.DateEffective;
                        Career.DepartmentId = model.DepartmentId;
                        Career.PositionId = model.PositionId;
                        Career.PositionLevel = model.PositionLevel;
                        Career.CustomRate = model.CustomRate;
                        Career.CustomRate_IsDailyRate = model.CustomRate_IsDailyRate;
                        Career.Events = model.Events;
                        Career.EventRemarks = string.IsNullOrEmpty(model.EventRemarks) ? "" : model.EventRemarks;
                        Career.EmploymentType = model.EmploymentType;
                        Career.IsDesignated = model.IsDesignated;
                        Career.EndEvent = model.EndEvent;
                        Career.EndEventDate = model.EndEventDate;
                        Career.EndEventRemarks = string.IsNullOrEmpty(model.EndEventRemarks) ? "" : model.EndEventRemarks;
                        Career.SalaryGrade = model.SalaryGrade;
                        Career.Fund = model.Fund;

                        if (FixedSettings.UseMFO)
                        {
                            Career.MFOId = model.MFOId;
                        }

                        ModifiedMemberInfo[] mmi = context.tblEmployee_Careers.GetModifiedMembers(Career);
                        context.SubmitChanges();

                        Cache.Update_Tables(_employee_careers: true);

                        res.Remarks = "Record was successfully updated";

                        MvcApplication.HLLog.UpdateLog("Employee Career", Career.CareerId, employee.FullName, mmi);
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

                if (ModelState.IsValid)
                {
                    tblEmployee_Career Career = context.tblEmployee_Careers.Where(x => x.CareerId == id).SingleOrDefault();
                    if (Career == null)
                    {
                        throw new Exception(ModuleConstants.ID_NOT_FOUND);
                    }
                    else
                    {
                        context.tblEmployee_Careers.DeleteOnSubmit(Career);
                        context.SubmitChanges();

                        Cache.Update_Tables(_employee_careers: true);

                        res.Remarks = "Record was successfully deleted";

                        MvcApplication.HLLog.DeleteLog("Employee Career", Career.CareerId, employee.FullName);

                    }

                }
                else
                {
                    throw new Exception(coreProcs.ShowErrors(ModelState));
                }
            }
        }
        
        [HttpPost]
        public JsonResult UpdateStartedInGovService(DateTime dt)
        {            
            queryResult res = new queryResult { IsSuccessful = true, Data = null, Err = "", Remarks = "" };

            try
            {
                using (dalDataContext context = new dalDataContext())
                {
                    tblEmployee emp = context.tblEmployees.Where(x => x.EmployeeId == employee.EmployeeId).SingleOrDefault();
                    if (emp == null)
                    {
                        throw new Exception(ModuleConstants.ID_NOT_FOUND);
                    }

                    if (dt > DateTime.Today)
                    {
                        AddError("Date cannot be later than the current date");
                    }
                                        
                    if (ModelState.IsValid)
                    {
                        emp.StartedInGovService = dt;

                        context.SubmitChanges();

                        res.Remarks = "Date Started in Government Service was successfully updated";

                        MvcApplication.HLLog.WriteLog("Employee Career", "Updated Date Started in Gov't. Service",
                                   string.Format("Employee: [{0}] {1}", employee.EmployeeId, employee.FullName));
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

        public ActionResult Print(bool dlWord = false)
        {
            using (dalDataContext context = new dalDataContext())
            {
                string fn = string.Format("service-record-{0}", employee.LastName.ToLower());

                objData CustomData = new objData
                {
                    ApplicationName = MvcApplication.ApplicationLicense.Info.ApplicationName,
                    ModuleName = "HR",
                    ReportName = "servicerecord"
                };

                string appPath = Server.MapPath("~/");

                return new asposeWordsTemplateReport(CustomizeDoc_Aspose, new ReportHeaderParams(appPath) { ReportLogo_Width = 40, ReportLogo_Height = 40 }, FixedSettings.ApplicationName, FixedSettings.Owner,
                    new ReportFooterParams(appPath), CustomData)
                    .Get(CustomData.ReportName, fn, employee, dlWord);
            }
        }

        public void CustomizeDoc_Aspose(object data, ref Aspose.Words.Document wordDoc)
        {
            using (dalDataContext context = new dalDataContext())
            {
                string[] fields = new string[] {
                    "surname", "givenname", "mi", "maidenname", "dob", "pob", "date"
                };

                tblEmployee employee = (tblEmployee)data;
                tblEmployee_Info info = context.tblEmployee_Infos.Where(x => x.EmployeeId == employee.EmployeeId).SingleOrDefault();

                string mi = string.IsNullOrEmpty(employee.MiddleName) ? "" : employee.MiddleName.Substring(0, 1);

                string[] fieldValues = new string[] {
                    employee.LastName.ToUpper(),
                    employee.FirstName.ToUpper(),
                    mi.ToUpper(),
                    "",
                    info == null ? null : (info.DOB == null ? "" : info.DOB.Value.ToString("dd MMM yyyy")),
                    info == null ? null : info.POB,
                    DateTime.Today.ToString("dd MMM yyyy")
                };

                Procs.ApplySignatories("hr", "servicerecord", ref fields, ref fieldValues);

                wordDoc.MailMerge.Execute(fields, fieldValues);

                Aspose.Words.Bookmark bookmark = wordDoc.Range.Bookmarks["tableRef"];
                Node _t = bookmark.BookmarkStart.GetAncestor(NodeType.Table);

                Aspose.Words.Tables.Table t = (Aspose.Words.Tables.Table)_t;

                Aspose.Words.Tables.Row r = null;
                Aspose.Words.Tables.Row row = (Aspose.Words.Tables.Row)t.Rows[t.Rows.Count - 1].Clone(true);

                List<double?> values = new List<double?>();

                List<tblEmployee_Career> careers = context.tblEmployee_Careers.Where(x => x.EmployeeId == employee.EmployeeId).OrderBy(x => x.DateEffective).ToList();


                for (int i = 1; i <= careers.Count; i++)
                {
                    tblEmployee_Career career = careers[i - 1];
                    bool isLastRecord = i == careers.Count;

                    if (i > 1)
                    {
                        t.Rows.Add(row.Clone(true));
                    }

                    r = t.Rows[t.Rows.Count - 1];

                    int cellIndex = 0;

                    etPeriod period = new etPeriod(employee.EmployeeId, career.DateEffective, isLastRecord ? DateTime.Today : career.EndEventDate.Value);

                    //From
                    r.Cells[cellIndex].FirstParagraph.AppendChild(new Run(wordDoc, career.DateEffective.ToString("MM/dd/yyyy")));

                    //To
                    r.Cells[++cellIndex].FirstParagraph.AppendChild(new Run(wordDoc, isLastRecord ? "Present" : career.EndEventDate.Value.ToString("MM/dd/yyyy")));

                    //Designation
                    r.Cells[++cellIndex].FirstParagraph.AppendChild(new Run(wordDoc, career.Position.Position));

                    //Status
                    r.Cells[++cellIndex].FirstParagraph.AppendChild(new Run(wordDoc, career.EmploymentType_Desc));

                    //Annual Salary
                    r.Cells[++cellIndex].FirstParagraph.AppendChild(new Run(wordDoc, (career.MonthlyRate * 12).ToCurrencyString()));

                    //Place of Assignment
                    r.Cells[++cellIndex].FirstParagraph.AppendChild(new Run(wordDoc, career.DepartmentPath()));

                    //Fund
                    r.Cells[++cellIndex].FirstParagraph.AppendChild(new Run(wordDoc, career.Fund ?? ""));

                    //Leave/Absences
                    double lwop = coreApp.Areas.LeaveModule.Common.EmployeeLWOP(employee.EmployeeId, career.DateEffective, isLastRecord ? DateTime.Today : career.EndEventDate.Value);
                    double lwp = coreApp.Areas.LeaveModule.Common.EmployeeLWP(employee.EmployeeId, career.DateEffective, isLastRecord ? DateTime.Today : career.EndEventDate.Value);

                    string l = "";
                    if (lwop > 0)
                    {
                        l += (l == "" ? "" : Environment.NewLine) + string.Format("{0} days w/o pay", lwop.ToString("#,##0.#"));
                    }

                    if (lwp > 0)
                    {
                        l += (l == "" ? "" : Environment.NewLine) + string.Format("{0} days w/ pay", lwp.ToString("#,##0.#"));
                    }

                    r.Cells[++cellIndex].FirstParagraph.AppendChild(new Run(wordDoc, l));

                    //Remarks
                    string rem = career.EndEvent == (int)CareerEvent.Others ? "" : career.EndEvent_Desc ?? "";
                    rem += (string.IsNullOrEmpty(rem) ? "" : ". ") + career.EndEventRemarks;

                    if (!string.IsNullOrEmpty(rem))
                    {
                        rem = rem.SentenceCase(true);
                    }

                   r.Cells[++cellIndex].FirstParagraph.AppendChild(new Run(wordDoc, rem));
                }

                t.GetChildNodes(NodeType.Run, true).ToArray().ToList().ForEach(x => ((Run)x).Font.Size = 10);
            }
        }

        
    }
}
