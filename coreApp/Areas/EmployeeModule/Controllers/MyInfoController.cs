using System;
using System.Linq;
using System.Web.Mvc;
using coreApp.Filters;
using coreApp.Controllers;
using coreApp.DAL;
using coreApp.Interfaces;
using coreLib.Objects;
using reports;
using coreLib.Extensions;
using reports.AsposeLib;
using Module.DB;
using Module.Core;
using Aspose.Words;

namespace coreApp.Areas.EmployeeModule.Controllers
{
    [MyFilter]
    public class MyInfoController : Base_NoCoreEmployeeController, IMyController
    {
        public tblEmployee employee { get; set; }
        

        public ActionResult Details()
        {

            using (dalDataContext context = new dalDataContext())
            {
                tblEmployee_Info Info = context.tblEmployee_Infos.Where(x => x.EmployeeId == employee.EmployeeId).SingleOrDefault();
                if (Info == null)
                {
                    Info = new tblEmployee_Info
                    {
                        EmployeeId = employee.EmployeeId,
                        Email = employee.Email,
                        Home_CountryId = ModuleConstants.PHILIPPINES_COUNTRY_ID,
                        Nationality_CountryId = ModuleConstants.PHILIPPINES_COUNTRY_ID,
                        Perm_CountryId = ModuleConstants.PHILIPPINES_COUNTRY_ID,
                        POB_CountryId = ModuleConstants.PHILIPPINES_COUNTRY_ID,
                        Spouse_Employer_CountryId = ModuleConstants.PHILIPPINES_COUNTRY_ID,
                        BIRStatus = "S"
                    };

                    context.tblEmployee_Infos.InsertOnSubmit(Info);
                    context.SubmitChanges();
                }

                return View(Info);
            }

        }

        public ActionResult Edit()
        {
            ViewBag.Title = "Basic Information";
            

            ViewBag.uiIsReadOnly = false;
            ViewBag.uiIncludeId = true;

            using (dalDataContext context = new dalDataContext())
            {


                tblEmployee_Info Info = context.tblEmployee_Infos.Where(x => x.EmployeeId == employee.EmployeeId).Single();
                if (Info == null)
                {
                    return new Raw_ActionResult(ModuleConstants.ID_NOT_FOUND);
                }
                else
                {
                    return PartialView("~/Areas/HRModule/Views/EmployeeInfo/_Info.cshtml", Info);
                }
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(tblEmployee_Info model)
        {
            queryResult res = new queryResult { IsSuccessful = true, Data = null, Err = "", Remarks = "" };

            try
            {
                using (dalDataContext context = new dalDataContext())
                {
                    if (!Cache.Get().userAccess.HasPermission("EmployeeInfo"))
                    {
                        AddError(ModuleConstants_Authorization.USERACCESS_UNAUTHORIZED_ACTION);
                    }

                    if (ModelState.IsValid)
                    {
                        DBProcs.save_EmployeeInfo(model);

                        res.Remarks = "Record was successfully updated";

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

        public ActionResult PDSQuestions()
        {
            using (dalDataContext context = new dalDataContext())
            {
                tblEmployee_Info Info = context.tblEmployee_Infos.Where(x => x.EmployeeId == employee.EmployeeId).SingleOrDefault();
                if (Info == null)
                {
                    Info = new tblEmployee_Info
                    {
                        EmployeeId = employee.EmployeeId,
                        Email = employee.Email,
                        Home_CountryId = ModuleConstants.PHILIPPINES_COUNTRY_ID,
                        Nationality_CountryId = ModuleConstants.PHILIPPINES_COUNTRY_ID,
                        Perm_CountryId = ModuleConstants.PHILIPPINES_COUNTRY_ID,
                        POB_CountryId = ModuleConstants.PHILIPPINES_COUNTRY_ID,
                        Spouse_Employer_CountryId = ModuleConstants.PHILIPPINES_COUNTRY_ID,
                        BIRStatus = "S"
                    };

                    context.tblEmployee_Infos.InsertOnSubmit(Info);
                    context.SubmitChanges();
                }

                return View(Info);
            }
        }

        public ActionResult EditPDSQuestions()
        {
            ViewBag.Title = "Miscellaneous";
            

            ViewBag.uiIsReadOnly = false;
            ViewBag.uiIncludeId = true;

            using (dalDataContext context = new dalDataContext())
            {
                tblEmployee_Info Info = context.tblEmployee_Infos.Where(x => x.EmployeeId == employee.EmployeeId).SingleOrDefault();
                if (Info == null)
                {
                    return new Raw_ActionResult(ModuleConstants.ID_NOT_FOUND);
                }
                else
                {
                    return PartialView("_PDSQuestions", Info);
                }
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditPDSQuestions(tblEmployee_Info model)
        {
            return new coreApp.Areas.HRModule.Controllers.EmployeeInfoController().EditPDSQuestions(model, "EmployeeInfo");
        }

        public ActionResult Print(bool dlWord = false)
        {
            coreApp.Areas.HRModule.Controllers.EmployeeInfoController empInfo = new coreApp.Areas.HRModule.Controllers.EmployeeInfoController { employee = employee };

            using (dalDataContext context = new dalDataContext())
            {
                using (hr2017DataContext _context = new hr2017DataContext())
                {
                    tblEmployee_Info info = context.tblEmployee_Infos.Where(x => x.EmployeeId == employee.EmployeeId).SingleOrDefault();
                    if (info == null)
                    {
                        throw new Exception("Employee has no personal information defined");
                    }

                    string finalFilename = "", newFilename = "";
                    string fn = string.Format("pds-{0}", employee.FullName.ToCleanString());

                    SeparableList.GetList(_context.tblEmployee_Childrens.Where(x => x.EmployeeId == employee.EmployeeId).OrderBy(x => x.DateOfBirth).ThenBy(x => x.Name).ToList(), ref empInfo.children, ref empInfo.children_ext);
                    SeparableList.GetList(_context.tblEmployee_Educs.Where(x => x.EmployeeId == employee.EmployeeId).OrderBy(x => x.EducLevel).ThenBy(x => x.YearFrom).ToList(), ref empInfo.educs, ref empInfo.educs_ext);
                    SeparableList.GetList(_context.tblEmployee_CivilServices.Where(x => x.EmployeeId == employee.EmployeeId).OrderByDescending(x => x.DateOfExam).ToList(), ref empInfo.cs, ref empInfo.cs_ext);
                    SeparableList.GetList(_context.tblEmployee_WorkExps.Where(x => x.EmployeeId == employee.EmployeeId).OrderByDescending(x => x.DateTo_Present).ThenByDescending(x => x.DateTo).ToList(), ref empInfo.wexp, ref empInfo.wexp_ext);
                    SeparableList.GetList(_context.tblEmployee_Voluntaries.Where(x => x.EmployeeId == employee.EmployeeId).OrderByDescending(x => x.DateTo).ToList(), ref empInfo.vol, ref empInfo.vol_ext);
                    SeparableList.GetList(Procs.GetEmployeeTrainings(employee.EmployeeId).OrderByDescending(x => x.DateTo).ToList(), ref empInfo.trn, ref empInfo.trn_ext);
                    SeparableList.GetList(_context.tblEmployee_OtherInfo_Skills.Where(x => x.EmployeeId == employee.EmployeeId).OrderBy(x => x.Description).ToList(), ref empInfo.skill, ref empInfo.skill_ext);
                    SeparableList.GetList(_context.tblEmployee_OtherInfo_NonAcademics.Where(x => x.EmployeeId == employee.EmployeeId).OrderBy(x => x.Description).ToList(), ref empInfo.non, ref empInfo.non_ext);
                    SeparableList.GetList(_context.tblEmployee_OtherInfo_Memberships.Where(x => x.EmployeeId == employee.EmployeeId).OrderBy(x => x.Description).ToList(), ref empInfo.mem, ref empInfo.mem_ext);


                    asposeWordsTemplateReport obj = new asposeWordsTemplateReport(empInfo.CustomizeDoc_Aspose);
                    newFilename = obj.GetReference("pds", info, true);

                    if (empInfo.children_ext.Any())
                    {
                        asposeWordsTemplateReport obj2 = new asposeWordsTemplateReport(empInfo.CustomizeDoc_Aspose_PDSChildren)
                        {
                            customWorkingFilename = "pds-tmp"
                        };

                        string f2 = obj2.GetReference("pds-children", null, true, "out2");
                        obj.mergeFiles(newFilename, f2, true);
                    }

                    if (empInfo.educs_ext.Any())
                    {
                        asposeWordsTemplateReport obj2 = new asposeWordsTemplateReport(empInfo.CustomizeDoc_Aspose_PDSEduc)
                        {
                            customWorkingFilename = "pds-tmp"
                        };

                        string f2 = obj2.GetReference("pds-educ", null, true, "out2");
                        obj.mergeFiles(newFilename, f2, true);
                    }

                    if (empInfo.cs_ext.Any())
                    {
                        asposeWordsTemplateReport obj2 = new asposeWordsTemplateReport(empInfo.CustomizeDoc_Aspose_PDSCS)
                        {
                            customWorkingFilename = "pds-tmp"
                        };

                        string f2 = obj2.GetReference("pds-cs", null, true, "out2");
                        obj.mergeFiles(newFilename, f2, true);
                    }

                    if (empInfo.wexp_ext.Any())
                    {
                        asposeWordsTemplateReport obj2 = new asposeWordsTemplateReport(empInfo.CustomizeDoc_Aspose_PDSWorkExp)
                        {
                            customWorkingFilename = "pds-tmp"
                        };

                        string f2 = obj2.GetReference("pds-wexp", null, true, "out2");
                        obj.mergeFiles(newFilename, f2, true);
                    }

                    if (empInfo.vol_ext.Any())
                    {
                        asposeWordsTemplateReport obj2 = new asposeWordsTemplateReport(empInfo.CustomizeDoc_Aspose_PDSVoluntary)
                        {
                            customWorkingFilename = "pds-tmp"
                        };

                        string f2 = obj2.GetReference("pds-voluntary", null, true, "out2");
                        obj.mergeFiles(newFilename, f2, true);
                    }

                    if (empInfo.trn_ext.Any())
                    {
                        asposeWordsTemplateReport obj2 = new asposeWordsTemplateReport(empInfo.CustomizeDoc_Aspose_PDSTraining)
                        {
                            customWorkingFilename = "pds-tmp"
                        };

                        string f2 = obj2.GetReference("pds-trn", null, true, "out2");
                        obj.mergeFiles(newFilename, f2, true);
                    }

                    if (empInfo.skill_ext.Any() || empInfo.non_ext.Any() || empInfo.mem_ext.Any())
                    {
                        asposeWordsTemplateReport obj2 = new asposeWordsTemplateReport(empInfo.CustomizeDoc_Aspose_PDSOtherInfo)
                        {
                            customWorkingFilename = "pds-tmp"
                        };

                        string f2 = obj2.GetReference("pds-otherinfo", null, true, "out2");
                        obj.mergeFiles(newFilename, f2, true);
                    }



                    finalFilename = newFilename;
                    obj.dlWord = dlWord;
                    if (!dlWord)
                    {
                        finalFilename = newFilename.Replace(".docx", ".pdf");
                        Document doc = new Document(newFilename);
                        doc.Save(finalFilename, SaveFormat.Pdf);

                        System.IO.File.Delete(newFilename);
                    }

                    return obj.DownloadFile(finalFilename, fn);
                }
            }
        }

    }
}
