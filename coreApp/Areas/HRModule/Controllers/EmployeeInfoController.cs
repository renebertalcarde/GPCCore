using System;
using System.Linq;
using System.Web.Mvc;
using coreApp.Filters;
using coreApp.Controllers;
using coreApp.DAL;
using coreApp.Interfaces;
using coreLib.Objects;
using coreLib.Extensions;
using reports;
using coreApp.Enums;
using System.IO;
using System.Drawing;
using reports.AsposeLib;
using Aspose.Words;
using System.Collections.Generic;
using Module.DB;
using Module.DB.Enums;
using Module.Core;
using System.Globalization;

namespace coreApp.Areas.HRModule.Controllers
{
    [EmployeeInfoFilter]
    [EmployeeAuthorize]
    [UserAccessAuthorize(allowedAccess: "hr_emp_info")]
    public class EmployeeInfoController : HLBase_AuthorizedController, IEmployeeController
    {
        public tblEmployee employee { get; set; }

        public EmployeeInfoController()
        {
            DetailsProc = new DetailsDelegate(_Details);
            EditProc = new EditDelegate(_Edit);
            EditPostProc = new EditPostDelegate(_EditPost);
        }


        public ActionResult _Details(int? id)
        {
            

            using (dalDataContext context = new dalDataContext())
            {
                tblEmployee_Info Info = context.tblEmployee_Infos.Where(x => x.EmployeeId == employee.EmployeeId).FirstOrDefault();
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

                    Cache.Update_Tables(_employee_infos: true);
                }

                return View(Info);
            }

        }
        
        public ActionResult _Edit(int? id)
        {
            ViewBag.Title = "Basic Information";
            
            
            ViewBag.uiIsReadOnly = false;
            ViewBag.uiIncludeId = true;

            using (dalDataContext context = new dalDataContext())
            {
                

                tblEmployee_Info Info = context.tblEmployee_Infos.Where(x => x.InfoId == id).Single();
                if (Info == null)
                {
                    return new Raw_ActionResult(ModuleConstants.ID_NOT_FOUND);
                }
                else
                {
                    return PartialView("_Info", Info);
                }
            }
        }


        public void _EditPost(FormCollection _model, ref queryResult res)
        {
            tblEmployee_Info model = (tblEmployee_Info)UpdateModel(new tblEmployee_Info().GetType(), _model);

            using (dalDataContext context = new dalDataContext())
            {
                if (!Cache.Get().userAccess.HasPermission())
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

        public ActionResult PDSQuestions(int? id)
        {
            if (id == null)
            {
                return new Raw_ActionResult(ModuleConstants.BAD_REQUEST);
            }

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

                    Cache.Update_Tables(_employee_infos: true);
                }

                return View(Info);
            }
        }

        public ActionResult EditPDSQuestions(int? id)
        {
            if (id == null)
            {
                return new Raw_ActionResult(ModuleConstants.BAD_REQUEST);
            }

            ViewBag.Title = "Miscellaneous";
            

            ViewBag.uiIsReadOnly = false;
            ViewBag.uiIncludeId = true;

            using (dalDataContext context = new dalDataContext())
            {


                tblEmployee_Info Info = context.tblEmployee_Infos.Where(x => x.InfoId == id).SingleOrDefault();
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
        public ActionResult EditPDSQuestions(tblEmployee_Info model, string _controller = "")
        {
            queryResult res = new queryResult { IsSuccessful = true, Data = null, Err = "", Remarks = "" };

            ModelState.Remove("BIRStatus");
            ModelState.Remove("POB_CountryId");
            ModelState.Remove("Home_CountryId");

            try
            {

                using (dalDataContext context = new dalDataContext())
                {
                    if (!Cache.Get().userAccess.HasPermission(_controller))
                    {
                        AddError(ModuleConstants_Authorization.USERACCESS_UNAUTHORIZED_ACTION);
                    }

                    if (model.PDSQ34b == true && string.IsNullOrEmpty(model.PDSQ34b_Details))
                    {
                        AddError("Question #34-b details required");
                    }

                    if (model.PDSQ35a == true && string.IsNullOrEmpty(model.PDSQ35a_Details))
                    {
                        AddError("Question #35-a details required");
                    }

                    if (model.PDSQ35b == true && model.PDSQ35b_DateFiled == null)
                    {
                        AddError("Question #35-b date filed is required");
                    }

                    if (model.PDSQ35b == true && string.IsNullOrEmpty(model.PDSQ35b_Status))
                    {
                        AddError("Question #35-b status is required");
                    }

                    if (model.PDSQ36 == true && string.IsNullOrEmpty(model.PDSQ36_Details))
                    {
                        AddError("Question #36 details required");
                    }

                    if (model.PDSQ37 == true && string.IsNullOrEmpty(model.PDSQ37_Details))
                    {
                        AddError("Question #37 details required");
                    }

                    if (model.PDSQ38a == true && string.IsNullOrEmpty(model.PDSQ38a_Details))
                    {
                        AddError("Question #38-a details required");
                    }

                    if (model.PDSQ38b == true && string.IsNullOrEmpty(model.PDSQ38b_Details))
                    {
                        AddError("Question #38-b details required");
                    }

                    if (model.PDSQ39 == true && string.IsNullOrEmpty(model.PDSQ39_Details))
                    {
                        AddError("Question #39 details required");
                    }

                    if (model.PDSQ40a == true && string.IsNullOrEmpty(model.PDSQ40a_Details))
                    {
                        AddError("Question #40-a details required");
                    }

                    if (model.PDSQ40b == true && string.IsNullOrEmpty(model.PDSQ40b_Details))
                    {
                        AddError("Question #40-b details required");
                    }

                    if (model.PDSQ40c == true && string.IsNullOrEmpty(model.PDSQ40c_Details))
                    {
                        AddError("Question #40-c details required");
                    }

                    if (ModelState.IsValid)
                    {
                        tblEmployee_Info Info = context.tblEmployee_Infos.Where(x => x.InfoId == model.InfoId).SingleOrDefault();
                        if (Info == null)
                        {
                            throw new Exception(ModuleConstants.ID_NOT_FOUND);
                        }

                        Info.PDSQ34a = model.PDSQ34a;
                        Info.PDSQ34b = model.PDSQ34b;
                        Info.PDSQ34b_Details = model.PDSQ34b != true ? "" : model.PDSQ34b_Details;
                        Info.PDSQ35a = model.PDSQ35a;
                        Info.PDSQ35a_Details = model.PDSQ35a != true ? "" : model.PDSQ35a_Details;
                        Info.PDSQ35b = model.PDSQ35b;
                        Info.PDSQ35b_DateFiled = model.PDSQ35b != true ? null : model.PDSQ35b_DateFiled;
                        Info.PDSQ35b_Status = model.PDSQ35b != true ? "" : model.PDSQ35b_Status;
                        Info.PDSQ36 = model.PDSQ36;
                        Info.PDSQ36_Details = model.PDSQ36 != true ? "" : model.PDSQ36_Details;
                        Info.PDSQ37 = model.PDSQ37;
                        Info.PDSQ37_Details = model.PDSQ37 != true ? "" : model.PDSQ37_Details;
                        Info.PDSQ38a = model.PDSQ38a;
                        Info.PDSQ38a_Details = model.PDSQ38a != true ? "" : model.PDSQ38a_Details;
                        Info.PDSQ38b = model.PDSQ38b;
                        Info.PDSQ38b_Details = model.PDSQ38b != true ? "" : model.PDSQ38b_Details;
                        Info.PDSQ39 = model.PDSQ39;
                        Info.PDSQ39_Details = model.PDSQ39 != true ? "" : model.PDSQ39_Details;
                        Info.PDSQ40a = model.PDSQ40a;
                        Info.PDSQ40a_Details = model.PDSQ40a != true ? "" : model.PDSQ40a_Details;
                        Info.PDSQ40b = model.PDSQ40b;
                        Info.PDSQ40b_Details = model.PDSQ40b != true ? "" : model.PDSQ40b_Details;
                        Info.PDSQ40c = model.PDSQ40c;
                        Info.PDSQ40c_Details = model.PDSQ40c != true ? "" : model.PDSQ40c_Details;

                        context.SubmitChanges();

                        Cache.Update_Tables(_employee_infos: true);

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

        public List<tblEmployee_Children> children = new List<tblEmployee_Children>();
        public List<tblEmployee_Children> children_ext = new List<tblEmployee_Children>();
        public List<tblEmployee_Educ> educs = new List<tblEmployee_Educ>();
        public List<tblEmployee_Educ> educs_ext = new List<tblEmployee_Educ>();
        public List<tblEmployee_CivilService> cs = new List<tblEmployee_CivilService>();
        public List<tblEmployee_CivilService> cs_ext = new List<tblEmployee_CivilService>();
        public List<tblEmployee_WorkExp> wexp = new List<tblEmployee_WorkExp>();
        public List<tblEmployee_WorkExp> wexp_ext = new List<tblEmployee_WorkExp>();
        public List<tblEmployee_Voluntary> vol = new List<tblEmployee_Voluntary>();
        public List<tblEmployee_Voluntary> vol_ext = new List<tblEmployee_Voluntary>();
        public List<tblEmployee_Training> trn = new List<tblEmployee_Training>();
        public List<tblEmployee_Training> trn_ext = new List<tblEmployee_Training>();
        public List<tblEmployee_OtherInfo_Skill> skill = new List<tblEmployee_OtherInfo_Skill>();
        public List<tblEmployee_OtherInfo_Skill> skill_ext = new List<tblEmployee_OtherInfo_Skill>();
        public List<tblEmployee_OtherInfo_NonAcademic> non = new List<tblEmployee_OtherInfo_NonAcademic>();
        public List<tblEmployee_OtherInfo_NonAcademic> non_ext = new List<tblEmployee_OtherInfo_NonAcademic>();
        public List<tblEmployee_OtherInfo_Membership> mem = new List<tblEmployee_OtherInfo_Membership>();
        public List<tblEmployee_OtherInfo_Membership> mem_ext = new List<tblEmployee_OtherInfo_Membership>();

        public ActionResult Print(bool dlWord = false)
        {
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

                    SeparableList.GetList(_context.tblEmployee_Childrens.Where(x => x.EmployeeId == employee.EmployeeId).OrderBy(x => x.DateOfBirth).ThenBy(x => x.Name).ToList(), ref children, ref children_ext);
                    SeparableList.GetList(_context.tblEmployee_Educs.Where(x => x.EmployeeId == employee.EmployeeId).OrderBy(x => x.EducLevel).ThenBy(x => x.YearFrom).ToList(), ref educs, ref educs_ext);
                    SeparableList.GetList(_context.tblEmployee_CivilServices.Where(x => x.EmployeeId == employee.EmployeeId).OrderByDescending(x => x.DateOfExam).ToList(), ref cs, ref cs_ext);
                    SeparableList.GetList(_context.tblEmployee_WorkExps.Where(x => x.EmployeeId == employee.EmployeeId).OrderByDescending(x => x.DateTo_Present).ThenByDescending(x => x.DateTo).ToList(), ref wexp, ref wexp_ext);
                    SeparableList.GetList(_context.tblEmployee_Voluntaries.Where(x => x.EmployeeId == employee.EmployeeId).OrderByDescending(x => x.DateTo).ToList(), ref vol, ref vol_ext);
                    SeparableList.GetList(Procs.GetEmployeeTrainings(employee.EmployeeId).OrderByDescending(x => x.DateTo).ToList(), ref trn, ref trn_ext);
                    SeparableList.GetList(_context.tblEmployee_OtherInfo_Skills.Where(x => x.EmployeeId == employee.EmployeeId).OrderBy(x => x.Description).ToList(), ref skill, ref skill_ext);
                    SeparableList.GetList(_context.tblEmployee_OtherInfo_NonAcademics.Where(x => x.EmployeeId == employee.EmployeeId).OrderBy(x => x.Description).ToList(), ref non, ref non_ext);
                    SeparableList.GetList(_context.tblEmployee_OtherInfo_Memberships.Where(x => x.EmployeeId == employee.EmployeeId).OrderBy(x => x.Description).ToList(), ref mem, ref mem_ext);

                    asposeWordsTemplateReport obj = new asposeWordsTemplateReport(CustomizeDoc_Aspose, null, FixedSettings.ApplicationName, FixedSettings.Owner);
                    newFilename = obj.GetReference("pds", info, true);
                    
                    if (children_ext.Any())
                    {
                        asposeWordsTemplateReport obj2 = new asposeWordsTemplateReport(CustomizeDoc_Aspose_PDSChildren, null, FixedSettings.ApplicationName, FixedSettings.Owner)
                        {
                            customWorkingFilename = "pds-tmp"
                        };

                        string f2 = obj2.GetReference("pds-children", null, true, "out2");
                        obj.mergeFiles(newFilename, f2, true);
                    }
                    
                    if (educs_ext.Any())
                    {
                        asposeWordsTemplateReport obj2 = new asposeWordsTemplateReport(CustomizeDoc_Aspose_PDSEduc, null, FixedSettings.ApplicationName, FixedSettings.Owner)
                        {
                            customWorkingFilename = "pds-tmp"
                        };

                        string f2 = obj2.GetReference("pds-educ", null, true, "out2");
                        obj.mergeFiles(newFilename, f2, true);
                    }

                    
                    if (cs_ext.Any())
                    {
                        asposeWordsTemplateReport obj2 = new asposeWordsTemplateReport(CustomizeDoc_Aspose_PDSCS, null, FixedSettings.ApplicationName, FixedSettings.Owner)
                        {
                            customWorkingFilename = "pds-tmp"
                        };

                        string f2 = obj2.GetReference("pds-cs", null, true, "out2");
                        obj.mergeFiles(newFilename, f2, true);
                    }
                    
                    if (wexp_ext.Any())
                    {
                        asposeWordsTemplateReport obj2 = new asposeWordsTemplateReport(CustomizeDoc_Aspose_PDSWorkExp, null, FixedSettings.ApplicationName, FixedSettings.Owner)
                        {
                            customWorkingFilename = "pds-tmp"
                        };

                        string f2 = obj2.GetReference("pds-wexp", null, true, "out2");
                        obj.mergeFiles(newFilename, f2, true);
                    }
                    
                    if (vol_ext.Any())
                    {
                        asposeWordsTemplateReport obj2 = new asposeWordsTemplateReport(CustomizeDoc_Aspose_PDSVoluntary, null, FixedSettings.ApplicationName, FixedSettings.Owner)
                        {
                            customWorkingFilename = "pds-tmp"
                        };

                        string f2 = obj2.GetReference("pds-voluntary", null, true, "out2");
                        obj.mergeFiles(newFilename, f2, true);
                    }

                    if (trn_ext.Any())
                    {
                        asposeWordsTemplateReport obj2 = new asposeWordsTemplateReport(CustomizeDoc_Aspose_PDSTraining, null, FixedSettings.ApplicationName, FixedSettings.Owner)
                        {
                            customWorkingFilename = "pds-tmp"
                        };

                        string f2 = obj2.GetReference("pds-trn", null, true, "out2");
                        obj.mergeFiles(newFilename, f2, true);
                    }

                    if (skill_ext.Any() || non_ext.Any() || mem_ext.Any())
                    {
                        asposeWordsTemplateReport obj2 = new asposeWordsTemplateReport(CustomizeDoc_Aspose_PDSOtherInfo)
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

                        reports.AsposeLib.Procs.SetDocProperties(ref doc, FixedSettings.ApplicationName, FixedSettings.Owner);

                        doc.Save(finalFilename, SaveFormat.Pdf);

                        System.IO.File.Delete(newFilename);
                    }

                    return obj.DownloadFile(finalFilename, fn);
                }
            }
        }

        public void CustomizeDoc_Aspose(object data, ref Aspose.Words.Document wordDoc)
        {
            using (hr2017DataContext context = new hr2017DataContext())
            {
                string[] fields = new string[] {
              "surname", "firstname", "middlename", "nameext", "dob", "pob", "dual-citizenship-country", "citizenship-details", "height", "weight", "bloodtype",
                    "gsis", "pagibig", "ph", "sss", "tin", "employeeno", "res_address", "res_zipcode", "perm_address", "perm_zipcode", "telephoneno", "mobileno", "email",
                    "spouse_surname", "spouse_firstname", "spouse_middlename", "spouse_nameext", "spouse_occupation", "spouse_employer", "spouse_businessaddress", "spouse_telephoneno",
                    "father_surname", "father_firstname", "father_middlename", "father_nameext",
                    "mother_surname", "mother_firstname", "mother_middlename",
                    "q34b", "q35a", "q35bdatefiled", "q35bstatus", "q36", "q37", "q38a", "q38b", "q39", "q40a", "q40b", "q40c",
                    "id_gov_issued", "id_license_passport", "date_of_issuance", "place_of_issuance"
                };

                tblEmployee_Info info = (tblEmployee_Info)data;

                string country = "";
                var _country = SelectItems.getCountries().Where(x => x.Value == info.Nationality_CountryId.ToString()).SingleOrDefault();
                if (_country != null)
                {
                    country = _country.Text.ToLower().SentenceCase(true);
                }

                string[] fieldValues = new string[] {
                    employee.LastName.ToUpper(),
                    employee.FirstName.ToUpper(),
                    (employee.MiddleName ?? "").ToUpper(),
                    (employee.NameExt ?? "").ToUpper(),
                    info == null ? "" : (info.DOB == null ? "" : info.DOB.Value.ToString("MM/dd/yyyy")),
                    info == null ? "" : info.POB.ToUpper(),
                    country.ToUpper(),
                    info.Citizenship_Details,
                    info.Height == null ? "" : info.Height.Value.ToString("#,##0.#"),
                    info.Weight == null ? "" : info.Weight.Value.ToString("#,##0.#"),
                    info.BloodType_Desc,
                    info.GSIS ?? "",
                    info.PAGIBIG ?? "",
                    info.PHILHEALTH ?? "",
                    info.SSS ?? "",
                    info.TIN ?? "",
                    employee.IdNo ?? "",
                    Module.DB.Procs.Common.getStatAddress(info.Home_Address, info.Home_BrgyId, null),
                    info.Home_PostalCode == null ? "" : info.Home_PostalCode.Value.ToString(),
                    Module.DB.Procs.Common.getStatAddress(info.Perm_Address, info.Perm_BrgyId, null),
                    info.Perm_PostalCode == null ? "" : info.Perm_PostalCode.Value.ToString(),
                    info.Home_TelephoneNo ?? "",
                    info.MobileNo ?? "",
                    info.Email ?? employee.Email ?? "",
                    (info.Spouse_LastName ?? "").ToUpper(),
                    (info.Spouse_FirstName ?? "").ToUpper(),
                    (info.Spouse_MiddleName ?? "").ToUpper(),
                    (info.Spouse_NameExt ?? "").ToUpper(),
                    (info.Spouse_Occupation ?? "").ToUpper(),
                    (info.Spouse_Employer ?? "").ToUpper(),
                    info.Spouse_EmployerAddress ?? "",
                    info.Spouse_Employer_TelephoneNo ?? "",
                    (info.Father_LastName ?? "").ToUpper(),
                    (info.Father_FirstName ?? "").ToUpper(),
                    (info.Father_MiddleName ?? "").ToUpper(),
                    (info.Father_NameExt ?? "").ToUpper(),
                    (info.Mother_LastName ?? "").ToUpper(),
                    (info.Mother_FirstName ?? "").ToUpper(),
                    (info.Mother_MiddleName ?? "").ToUpper(),
                    info.PDSQ34b_Details ?? "",
                    info.PDSQ35a_Details ?? "",
                    info.PDSQ35b_DateFiled == null ? "" : info.PDSQ35b_DateFiled.Value.ToString("MM/dd/yyyy"),
                    info.PDSQ35b_Status ?? "",
                    info.PDSQ36_Details ?? "",
                    info.PDSQ37_Details ?? "",
                    info.PDSQ38a_Details ?? "",
                    info.PDSQ38b_Details ?? "",
                    info.PDSQ39_Details ?? "",
                    info.PDSQ40a_Details ?? "",
                    info.PDSQ40b_Details ?? "",
                    info.PDSQ40c_Details ?? "",
                    info.PDS_GovIssuedId ?? "",
                    info.PDS_IDLIcensePPNo ?? "",
                    info.PDS_DateOfIssuance == null ? "" : info.PDS_DateOfIssuance.Value.ToString("MM/dd/yyyy"),
                    info.PDS_PlaceOfIssuance ?? ""
                };

                wordDoc.MailMerge.Execute(fields, fieldValues);

                DocumentBuilder builder = new DocumentBuilder(wordDoc);

                //BOOKMARKS

                Procs.setTick(info.Nationality_CountryId == ModuleConstants.PHILIPPINES_COUNTRY_ID, "tickFilipino", wordDoc);
                Procs.setTick(info.Citizenship_Dual == true, "tickDualCitizenship", wordDoc);
                Procs.setTick(info.Citizenship_ByBirth == true, "tickByBirth", wordDoc);
                Procs.setTick(info.Citizenship_ByNaturalization == true, "tickByNaturalization", wordDoc);
                Procs.setTick(info.Gender == (int)Gender.Male, "tickMale", wordDoc);
                Procs.setTick(info.Gender == (int)Gender.Female, "tickFemale", wordDoc);
                Procs.setTick(info.CivilStatus == (int)CivilStatus.Single, "tickSingle", wordDoc);
                Procs.setTick(info.CivilStatus == (int)CivilStatus.Married, "tickMarried", wordDoc);
                Procs.setTick(info.CivilStatus == (int)CivilStatus.Widow, "tickWidowed", wordDoc);
                Procs.setTick(info.CivilStatus == (int)CivilStatus.Widower, "tickWidower", wordDoc);
                Procs.setTick(info.CivilStatus == (int)CivilStatus.Separated, "tickSeparated", wordDoc);
                Procs.setTick(false, "tickOthers", wordDoc);
                Procs.setTick(info.PDSQ34a == true, "tickQ34aYes", wordDoc);
                Procs.setTick(info.PDSQ34a != true, "tickQ34aNo", wordDoc);
                Procs.setTick(info.PDSQ34b == true, "tickQ34bYes", wordDoc);
                Procs.setTick(info.PDSQ34b != true, "tickQ34bNo", wordDoc);
                Procs.setTick(info.PDSQ35a == true, "tickQ35aYes", wordDoc);
                Procs.setTick(info.PDSQ35a != true, "tickQ35aNo", wordDoc);
                Procs.setTick(info.PDSQ35b == true, "tickQ35bYes", wordDoc);
                Procs.setTick(info.PDSQ35b != true, "tickQ35bNo", wordDoc);
                Procs.setTick(info.PDSQ36 == true, "tickQ36Yes", wordDoc);
                Procs.setTick(info.PDSQ36 != true, "tickQ36No", wordDoc);
                Procs.setTick(info.PDSQ37 == true, "tickQ37Yes", wordDoc);
                Procs.setTick(info.PDSQ37 != true, "tickQ37No", wordDoc);
                Procs.setTick(info.PDSQ38a == true, "tickQ38aYes", wordDoc);
                Procs.setTick(info.PDSQ38a != true, "tickQ38aNo", wordDoc);
                Procs.setTick(info.PDSQ38b == true, "tickQ38bYes", wordDoc);
                Procs.setTick(info.PDSQ38b != true, "tickQ38bNo", wordDoc);
                Procs.setTick(info.PDSQ39 == true, "tickQ39Yes", wordDoc);
                Procs.setTick(info.PDSQ39 != true, "tickQ39No", wordDoc);
                Procs.setTick(info.PDSQ40a == true, "tickQ40aYes", wordDoc);
                Procs.setTick(info.PDSQ40a != true, "tickQ40aNo", wordDoc);
                Procs.setTick(info.PDSQ40b == true, "tickQ40bYes", wordDoc);
                Procs.setTick(info.PDSQ40b != true, "tickQ40bNo", wordDoc);
                Procs.setTick(info.PDSQ40c == true, "tickQ40cYes", wordDoc);
                Procs.setTick(info.PDSQ40c != true, "tickQ40cNo", wordDoc);

                if (info.PDSPhoto != null)
                {
                    string path = Path.Combine(Procs.getServerPath(), "Temp", string.Format("{0}-{1}.jpg", employee.LastName.ToCleanString(), DateTime.Now.ToString("yyyyMMddhhmmss")));

                    byte[] arr = info.PDSPhoto.ToArray();
                    using (var ms = new MemoryStream(arr))
                    {
                        Image.FromStream(ms).Save(path);

                        builder.MoveToBookmark("imgPhoto");
                        builder.InsertImage(ms, 100, 128);
                    }

                    System.IO.File.Delete(path);
                }


                //Children
                Aspose.Words.Bookmark bookmark;
                Aspose.Words.Tables.Table table;
                Aspose.Words.Tables.Cell cell;
                Aspose.Words.Tables.Row row;

                int tableIndex, rowIndex;

                bookmark = wordDoc.Range.Bookmarks["child1"];
                cell = (Aspose.Words.Tables.Cell)bookmark.BookmarkStart.GetAncestor(NodeType.Cell);
                row = (Aspose.Words.Tables.Row)bookmark.BookmarkStart.GetAncestor(NodeType.Row);
                table = (Aspose.Words.Tables.Table)bookmark.BookmarkStart.GetAncestor(NodeType.Table);

                tableIndex = table.ParentNode.GetChildNodes(NodeType.Table, false).IndexOf(table);
                rowIndex = table.IndexOf(row) - 1;
                
                for (int i = 1; i <= children.Count; i++)
                {
                    tblEmployee_Children child = children[i - 1];

                    builder.MoveToBookmark("child" + i);
                    builder.Write(child.Name.ToUpper());

                    builder.MoveToBookmark("child_dob" + i);
                    builder.Write(child.DateOfBirth.Value.ToString("MM/dd/yyyy"));
                }

                int rowOffset, cellIndex;
                Aspose.Words.Tables.Row r = null;
                Aspose.Words.Tables.Table t;

                //Education                
                bookmark = wordDoc.Range.Bookmarks["tableRef_Educ"];
                Node _t = bookmark.BookmarkStart.GetAncestor(NodeType.Table);

                t = (Aspose.Words.Tables.Table)_t;
                rowOffset = 3;

                for (int i = 1; i <= 5; i++)
                {
                    tblEmployee_Educ educ = educs.Where(x => x.EducLevel == i - 1).SingleOrDefault();
                    if (educ == null) continue;

                    r = t.Rows[rowOffset + i - 1];
                    cellIndex = 0;

                    r.Cells[++cellIndex].FirstParagraph.AppendChild(new Run(wordDoc, educ.School.ToUpper()));
                    r.Cells[++cellIndex].FirstParagraph.AppendChild(new Run(wordDoc, educ.Degree.ToUpper()));
                    r.Cells[++cellIndex].FirstParagraph.AppendChild(new Run(wordDoc, educ.YearFrom == null ? "" : educ.YearFrom.Value.ToString().ToUpper()));
                    r.Cells[++cellIndex].FirstParagraph.AppendChild(new Run(wordDoc, educ.YearTo_Desc.ToUpper()));
                    r.Cells[++cellIndex].FirstParagraph.AppendChild(new Run(wordDoc, educ.UnitsEarned == null ? "" : educ.UnitsEarned.Value.ToString("0.#")));
                    r.Cells[++cellIndex].FirstParagraph.AppendChild(new Run(wordDoc, educ.YearGraduated == null ? "" : educ.YearGraduated.ToString()));
                    r.Cells[++cellIndex].FirstParagraph.AppendChild(new Run(wordDoc, (educ.HonorsReceived ?? "").ToUpper()));

                }

                t.GetChildNodes(NodeType.Run, true).ToArray().ToList().ForEach(x => ((Run)x).Font.Size = 7);


                //Civil Service
                bookmark = wordDoc.Range.Bookmarks["tableRef_CS"];
                _t = bookmark.BookmarkStart.GetAncestor(NodeType.Table);

                t = (Aspose.Words.Tables.Table)_t;
                rowOffset = 3;

                for (int i = 1; i <= cs.Count; i++)
                {
                    tblEmployee_CivilService _cs = cs[i - 1];

                    r = t.Rows[rowOffset + i - 1];
                    cellIndex = 0;

                    r.Cells[cellIndex].FirstParagraph.AppendChild(new Run(wordDoc, (_cs.CareerService ?? "").ToUpper()));
                    r.Cells[++cellIndex].FirstParagraph.AppendChild(new Run(wordDoc, _cs.Rating == null ? "" : _cs.Rating.Value.ToString("0.##")));
                    r.Cells[++cellIndex].FirstParagraph.AppendChild(new Run(wordDoc, _cs.DateOfExam == null ? "" : _cs.DateOfExam.Value.ToString("MM/dd/yyyy").ToUpper()));
                    r.Cells[++cellIndex].FirstParagraph.AppendChild(new Run(wordDoc, _cs.POE.ToUpper()));
                    r.Cells[++cellIndex].FirstParagraph.AppendChild(new Run(wordDoc, _cs.LicenseNo ?? ""));
                    r.Cells[++cellIndex].FirstParagraph.AppendChild(new Run(wordDoc, _cs.LicenseReleaseDate == null ? "" : _cs.LicenseReleaseDate.Value.ToString("MM/dd/yyyy").ToUpper()));
                }

                t.GetChildNodes(NodeType.Run, true).ToArray().ToList().ForEach(x => ((Run)x).Font.Size = 7);

                //Work Experience
                bookmark = wordDoc.Range.Bookmarks["tableRef_WExp"];
                _t = bookmark.BookmarkStart.GetAncestor(NodeType.Table);

                t = (Aspose.Words.Tables.Table)_t;
                rowOffset = 3;

                for (int i = 1; i <= wexp.Count; i++)
                {
                    tblEmployee_WorkExp _wexp = wexp[i - 1];

                    r = t.Rows[rowOffset + i - 1];
                    cellIndex = 0;

                    r.Cells[cellIndex].FirstParagraph.AppendChild(new Run(wordDoc, _wexp.DateFrom == null ? "" : _wexp.DateFrom.Value.ToString("MM/dd/yyyy")));
                    r.Cells[++cellIndex].FirstParagraph.AppendChild(new Run(wordDoc, _wexp.DateTo_Present == true ? "Present" : (_wexp.DateTo == null ? "" : _wexp.DateTo.Value.ToString("MM/dd/yyyy"))));
                    r.Cells[++cellIndex].FirstParagraph.AppendChild(new Run(wordDoc, _wexp.Position.ToUpper()));
                    r.Cells[++cellIndex].FirstParagraph.AppendChild(new Run(wordDoc, _wexp.Company.ToUpper()));
                    r.Cells[++cellIndex].FirstParagraph.AppendChild(new Run(wordDoc, _wexp.Salary == null ? "" : _wexp.Salary.Value.ToCurrencyString()));
                    r.Cells[++cellIndex].FirstParagraph.AppendChild(new Run(wordDoc, _wexp.SalaryGrade ?? ""));
                    r.Cells[++cellIndex].FirstParagraph.AppendChild(new Run(wordDoc, _wexp.StatusOfAppointment_Desc));
                    r.Cells[++cellIndex].FirstParagraph.AppendChild(new Run(wordDoc, _wexp.IsGovService ? "Y" : "N"));

                }

                t.GetChildNodes(NodeType.Run, true).ToArray().ToList().ForEach(x => ((Run)x).Font.Size = 7);

                //Voluntary Works
                bookmark = wordDoc.Range.Bookmarks["tableRef_Voluntary"];
                _t = bookmark.BookmarkStart.GetAncestor(NodeType.Table);

                t = (Aspose.Words.Tables.Table)_t;
                rowOffset = 3;

                for (int i = 1; i <= vol.Count; i++)
                {
                    tblEmployee_Voluntary _vol = vol[i - 1];

                    r = t.Rows[rowOffset + i - 1];
                    cellIndex = 0;

                    string orgadd = _vol.Organization.ToUpper() + (string.IsNullOrEmpty(_vol.Address) ? "" : " / " + _vol.Address);

                    r.Cells[cellIndex].FirstParagraph.AppendChild(new Run(wordDoc, orgadd));
                    r.Cells[++cellIndex].FirstParagraph.AppendChild(new Run(wordDoc, _vol.DateFrom == null ? "" : _vol.DateFrom.Value.ToString("MM/dd/yyyy")));
                    r.Cells[++cellIndex].FirstParagraph.AppendChild(new Run(wordDoc, _vol.DateTo_Present == true ? "Present" : (_vol.DateTo == null ? "" : _vol.DateTo.Value.ToString("MM/dd/yyyy"))));
                    r.Cells[++cellIndex].FirstParagraph.AppendChild(new Run(wordDoc, _vol.NoOfHours.ToString()));
                    r.Cells[++cellIndex].FirstParagraph.AppendChild(new Run(wordDoc, _vol.PositionNatureOfWork.ToMultilineString()));
                }

                t.GetChildNodes(NodeType.Run, true).ToArray().ToList().ForEach(x => ((Run)x).Font.Size = 7);

                //Trainings
                bookmark = wordDoc.Range.Bookmarks["tableRef_Trainings"];
                _t = bookmark.BookmarkStart.GetAncestor(NodeType.Table);

                t = (Aspose.Words.Tables.Table)_t;
                rowOffset = 3;

                for (int i = 1; i <= trn.Count; i++)
                {
                    tblEmployee_Training _trn = trn[i - 1];

                    r = t.Rows[rowOffset + i - 1];
                    cellIndex = 0;

                    r.Cells[cellIndex].FirstParagraph.AppendChild(new Run(wordDoc, _trn.Title));
                    r.Cells[++cellIndex].FirstParagraph.AppendChild(new Run(wordDoc, _trn.DateFrom == null ? "" : _trn.DateFrom.Value.ToString("MM/dd/yyyy")));
                    r.Cells[++cellIndex].FirstParagraph.AppendChild(new Run(wordDoc, _trn.DateTo == null ? "" : _trn.DateTo.Value.ToString("MM/dd/yyyy")));
                    r.Cells[++cellIndex].FirstParagraph.AppendChild(new Run(wordDoc, _trn.NoOfHours == null ? "" : _trn.NoOfHours.Value.ToString("0.#")));
                    r.Cells[++cellIndex].FirstParagraph.AppendChild(new Run(wordDoc, _trn.Type_Desc));
                    r.Cells[++cellIndex].FirstParagraph.AppendChild(new Run(wordDoc, _trn.ConductedBy));

                }

                t.GetChildNodes(NodeType.Run, true).ToArray().ToList().ForEach(x => ((Run)x).Font.Size = 7);


                //Other Info - Skills
                bookmark = wordDoc.Range.Bookmarks["tableRef_Skills"];
                _t = bookmark.BookmarkStart.GetAncestor(NodeType.Table);

                t = (Aspose.Words.Tables.Table)_t;
                rowOffset = 2;

                for (int i = 1; i <= skill.Count; i++)
                {
                    tblEmployee_OtherInfo_Skill oi = skill[i - 1];

                    r = t.Rows[rowOffset + i - 1];
                    cellIndex = 0;

                    r.Cells[cellIndex].FirstParagraph.AppendChild(new Run(wordDoc, oi.Description));
                }

                //Other Info - NonAcademics
                bookmark = wordDoc.Range.Bookmarks["tableRef_NonAcademics"];
                _t = bookmark.BookmarkStart.GetAncestor(NodeType.Table);

                t = (Aspose.Words.Tables.Table)_t;
                rowOffset = 2;

                for (int i = 1; i <= non.Count; i++)
                {
                    tblEmployee_OtherInfo_NonAcademic oi = non[i - 1];

                    r = t.Rows[rowOffset + i - 1];
                    cellIndex = 1;

                    r.Cells[cellIndex].FirstParagraph.AppendChild(new Run(wordDoc, oi.Description));
                }

                //Other Info - Memberships
                bookmark = wordDoc.Range.Bookmarks["tableRef_Memberships"];
                _t = bookmark.BookmarkStart.GetAncestor(NodeType.Table);

                t = (Aspose.Words.Tables.Table)_t;
                rowOffset = 2;

                for (int i = 1; i <= mem.Count; i++)
                {
                    tblEmployee_OtherInfo_Membership oi = mem[i - 1];

                    r = t.Rows[rowOffset + i - 1];
                    cellIndex = 2;

                    r.Cells[cellIndex].FirstParagraph.AppendChild(new Run(wordDoc, oi.Description));
                }

                t.GetChildNodes(NodeType.Run, true).ToArray().ToList().ForEach(x => ((Run)x).Font.Size = 7);
                
                //References
                var references = context.tblEmployee_References.Where(x => x.EmployeeId == employee.EmployeeId).OrderBy(x => x.Name).ToList();

                for (int i = 1; i <= references.Count; i++)
                {
                    tblEmployee_Reference reference = references[i - 1];

                    bookmark = wordDoc.Range.Bookmarks["ref" + i];
                    row = (Aspose.Words.Tables.Row)bookmark.BookmarkStart.GetAncestor(NodeType.Row);

                    builder.MoveTo(row.Cells[0].FirstParagraph);
                    builder.Write(reference.Name);

                    builder.MoveTo(row.Cells[1].FirstParagraph);
                    builder.Write(reference.Address);

                    builder.MoveTo(row.Cells[2].FirstParagraph);
                    builder.Write(reference.TelephoneNo);
                }
            }
        }

        public void CustomizeDoc_Aspose_PDSChildren(object data, ref Aspose.Words.Document wordDoc)
        {
            DocumentBuilder builder = new DocumentBuilder(wordDoc);
            
            Aspose.Words.Bookmark bookmark;
            Aspose.Words.Tables.Table t;
            Aspose.Words.Tables.Row row, r;

            bookmark = wordDoc.Range.Bookmarks["child1"];
            Node _t = bookmark.BookmarkStart.GetAncestor(NodeType.Table);

            t = (Aspose.Words.Tables.Table)_t;

            r = null;
            row = (Aspose.Words.Tables.Row)t.Rows[t.Rows.Count - 1].Clone(true);

            for (int i = 1; i <= children_ext.Count; i++)
            {
                tblEmployee_Children child = children_ext[i - 1];

                if (i > 1)
                {
                    t.Rows.Add(row.Clone(true));
                }

                r = t.Rows[t.Rows.Count - 1];

                int cellIndex = 0;
                
                r.Cells[cellIndex].FirstParagraph.AppendChild(new Run(wordDoc, child.Name.ToUpper()));
                r.Cells[++cellIndex].FirstParagraph.AppendChild(new Run(wordDoc, child.DateOfBirth.Value.ToString("MM/dd/yyyy")));

            }

            t.GetChildNodes(NodeType.Run, true).ToArray().ToList().ForEach(x => ((Run)x).Font.Size = 7);
        }

        public void CustomizeDoc_Aspose_PDSEduc(object data, ref Aspose.Words.Document wordDoc)
        {
            DocumentBuilder builder = new DocumentBuilder(wordDoc);

            Aspose.Words.Bookmark bookmark;
            Aspose.Words.Tables.Table t;
            Aspose.Words.Tables.Row row, r;

            bookmark = wordDoc.Range.Bookmarks["tableRef_Educ"];
            Node _t = bookmark.BookmarkStart.GetAncestor(NodeType.Table);

            t = (Aspose.Words.Tables.Table)_t;

            r = null;
            row = (Aspose.Words.Tables.Row)t.Rows[t.Rows.Count - 1].Clone(true);

            for (int i = 1; i <= educs_ext.Count; i++)
            {
                tblEmployee_Educ educ = educs_ext[i - 1];

                if (i > 1)
                {
                    t.Rows.Add(row.Clone(true));
                }

                r = t.Rows[t.Rows.Count - 1];

                int cellIndex = 0;

                r.Cells[cellIndex].FirstParagraph.AppendChild(new Run(wordDoc, educ.EducLevel_Desc.Replace("_", " ").ToUpper()));
                r.Cells[++cellIndex].FirstParagraph.AppendChild(new Run(wordDoc, educ.School.ToUpper()));
                r.Cells[++cellIndex].FirstParagraph.AppendChild(new Run(wordDoc, educ.Degree.ToUpper()));
                r.Cells[++cellIndex].FirstParagraph.AppendChild(new Run(wordDoc, educ.YearFrom == null ? "" : educ.YearFrom.Value.ToString().ToUpper()));
                r.Cells[++cellIndex].FirstParagraph.AppendChild(new Run(wordDoc, educ.YearTo_Desc.ToUpper()));
                r.Cells[++cellIndex].FirstParagraph.AppendChild(new Run(wordDoc, educ.UnitsEarned == null ? "" : educ.UnitsEarned.Value.ToString("0.#")));
                r.Cells[++cellIndex].FirstParagraph.AppendChild(new Run(wordDoc, educ.YearGraduated == null ? "" : educ.YearGraduated.ToString()));
                r.Cells[++cellIndex].FirstParagraph.AppendChild(new Run(wordDoc, (educ.HonorsReceived ?? "").ToUpper()));

            }

            t.GetChildNodes(NodeType.Run, true).ToArray().ToList().ForEach(x => ((Run)x).Font.Size = 7);
        }

        public void CustomizeDoc_Aspose_PDSCS(object data, ref Aspose.Words.Document wordDoc)
        {
            DocumentBuilder builder = new DocumentBuilder(wordDoc);

            Aspose.Words.Bookmark bookmark;
            Aspose.Words.Tables.Table t;
            Aspose.Words.Tables.Row row, r;

            bookmark = wordDoc.Range.Bookmarks["tableRef_CS"];
            Node _t = bookmark.BookmarkStart.GetAncestor(NodeType.Table);

            t = (Aspose.Words.Tables.Table)_t;

            r = null;
            row = (Aspose.Words.Tables.Row)t.Rows[t.Rows.Count - 1].Clone(true);

            for (int i = 1; i <= cs_ext.Count; i++)
            {
                tblEmployee_CivilService cs = cs_ext[i - 1];

                if (i > 1)
                {
                    t.Rows.Add(row.Clone(true));
                }

                r = t.Rows[t.Rows.Count - 1];

                int cellIndex = 0;

                r.Cells[cellIndex].FirstParagraph.AppendChild(new Run(wordDoc, (cs.CareerService ?? "").ToUpper()));
                r.Cells[++cellIndex].FirstParagraph.AppendChild(new Run(wordDoc, cs.Rating == null ? "" : cs.Rating.Value.ToString("0.#")));
                r.Cells[++cellIndex].FirstParagraph.AppendChild(new Run(wordDoc, cs.DateOfExam == null ? "" : cs.DateOfExam.Value.ToString("MM/dd/yyyy").ToUpper()));
                r.Cells[++cellIndex].FirstParagraph.AppendChild(new Run(wordDoc, cs.POE.ToUpper()));
                r.Cells[++cellIndex].FirstParagraph.AppendChild(new Run(wordDoc, cs.LicenseNo ?? ""));
                r.Cells[++cellIndex].FirstParagraph.AppendChild(new Run(wordDoc, cs.LicenseReleaseDate == null ? "" : cs.LicenseReleaseDate.Value.ToString("MM/dd/yyyy").ToUpper()));

            }

            t.GetChildNodes(NodeType.Run, true).ToArray().ToList().ForEach(x => ((Run)x).Font.Size = 7);
        }

        public void CustomizeDoc_Aspose_PDSTraining(object data, ref Aspose.Words.Document wordDoc)
        {
            DocumentBuilder builder = new DocumentBuilder(wordDoc);

            Aspose.Words.Bookmark bookmark;
            Aspose.Words.Tables.Table t;
            Aspose.Words.Tables.Row row, r;

            bookmark = wordDoc.Range.Bookmarks["tableRef_Trainings"];
            Node _t = bookmark.BookmarkStart.GetAncestor(NodeType.Table);

            t = (Aspose.Words.Tables.Table)_t;

            r = null;
            row = (Aspose.Words.Tables.Row)t.Rows[t.Rows.Count - 1].Clone(true);

            for (int i = 1; i <= trn_ext.Count; i++)
            {
                tblEmployee_Training _trn = trn_ext[i - 1];

                if (i > 1)
                {
                    t.Rows.Add(row.Clone(true));
                }

                r = t.Rows[t.Rows.Count - 1];

                int cellIndex = 0;

                r.Cells[cellIndex].FirstParagraph.AppendChild(new Run(wordDoc, _trn.Title));
                r.Cells[++cellIndex].FirstParagraph.AppendChild(new Run(wordDoc, _trn.DateFrom == null ? "" : _trn.DateFrom.Value.ToString("MM/dd/yyyy")));
                r.Cells[++cellIndex].FirstParagraph.AppendChild(new Run(wordDoc, _trn.DateTo == null ? "" : _trn.DateTo.Value.ToString("MM/dd/yyyy")));
                r.Cells[++cellIndex].FirstParagraph.AppendChild(new Run(wordDoc, _trn.NoOfHours == null ? "" : _trn.NoOfHours.Value.ToString("0.#")));
                r.Cells[++cellIndex].FirstParagraph.AppendChild(new Run(wordDoc, _trn.Type_Desc));
                r.Cells[++cellIndex].FirstParagraph.AppendChild(new Run(wordDoc, _trn.ConductedBy));

            }

            t.GetChildNodes(NodeType.Run, true).ToArray().ToList().ForEach(x => ((Run)x).Font.Size = 7);
        }

        public void CustomizeDoc_Aspose_PDSWorkExp(object data, ref Aspose.Words.Document wordDoc)
        {
            DocumentBuilder builder = new DocumentBuilder(wordDoc);

            Aspose.Words.Bookmark bookmark;
            Aspose.Words.Tables.Table t;
            Aspose.Words.Tables.Row row, r;

            bookmark = wordDoc.Range.Bookmarks["tableRef_WExp"];
            Node _t = bookmark.BookmarkStart.GetAncestor(NodeType.Table);

            t = (Aspose.Words.Tables.Table)_t;

            r = null;
            row = (Aspose.Words.Tables.Row)t.Rows[t.Rows.Count - 1].Clone(true);

            for (int i = 1; i <= wexp_ext.Count; i++)
            {
                tblEmployee_WorkExp wexp = wexp_ext[i - 1];

                if (i > 1)
                {
                    t.Rows.Add(row.Clone(true));
                }

                r = t.Rows[t.Rows.Count - 1];

                int cellIndex = 0;

                r.Cells[cellIndex].FirstParagraph.AppendChild(new Run(wordDoc, wexp.DateFrom == null ? "" : wexp.DateFrom.Value.ToString("MM/dd/yyyy")));
                r.Cells[++cellIndex].FirstParagraph.AppendChild(new Run(wordDoc, wexp.DateTo_Present == true ? "Present" : (wexp.DateTo == null ? "" : wexp.DateTo.Value.ToString("MM/dd/yyyy"))));
                r.Cells[++cellIndex].FirstParagraph.AppendChild(new Run(wordDoc, wexp.Position.ToUpper()));
                r.Cells[++cellIndex].FirstParagraph.AppendChild(new Run(wordDoc, wexp.Company.ToUpper()));
                r.Cells[++cellIndex].FirstParagraph.AppendChild(new Run(wordDoc, wexp.Salary == null ? "" : wexp.Salary.Value.ToCurrencyString()));
                r.Cells[++cellIndex].FirstParagraph.AppendChild(new Run(wordDoc, wexp.SalaryGrade ?? ""));
                r.Cells[++cellIndex].FirstParagraph.AppendChild(new Run(wordDoc, wexp.StatusOfAppointment_Desc));
                r.Cells[++cellIndex].FirstParagraph.AppendChild(new Run(wordDoc, wexp.IsGovService ? "Y" : "N"));

            }

            t.GetChildNodes(NodeType.Run, true).ToArray().ToList().ForEach(x => ((Run)x).Font.Size = 7);
        }

        public void CustomizeDoc_Aspose_PDSVoluntary(object data, ref Aspose.Words.Document wordDoc)
        {
            DocumentBuilder builder = new DocumentBuilder(wordDoc);

            Aspose.Words.Bookmark bookmark;
            Aspose.Words.Tables.Table t;
            Aspose.Words.Tables.Row row, r;

            bookmark = wordDoc.Range.Bookmarks["tableRef_Voluntary"];
            Node _t = bookmark.BookmarkStart.GetAncestor(NodeType.Table);

            t = (Aspose.Words.Tables.Table)_t;

            r = null;
            row = (Aspose.Words.Tables.Row)t.Rows[t.Rows.Count - 1].Clone(true);

            for (int i = 1; i <= vol_ext.Count; i++)
            {
                tblEmployee_Voluntary vol = vol_ext[i - 1];

                if (i > 1)
                {
                    t.Rows.Add(row.Clone(true));
                }

                r = t.Rows[t.Rows.Count - 1];

                int cellIndex = 0;

                string orgadd = vol.Organization.ToUpper() + (string.IsNullOrEmpty(vol.Address) ? "" : " / " + vol.Address);

                r.Cells[cellIndex].FirstParagraph.AppendChild(new Run(wordDoc, orgadd));
                r.Cells[++cellIndex].FirstParagraph.AppendChild(new Run(wordDoc, vol.DateFrom == null ? "" : vol.DateFrom.Value.ToString("MM/dd/yyyy")));
                r.Cells[++cellIndex].FirstParagraph.AppendChild(new Run(wordDoc, vol.DateTo_Present == true ? "Present" : (vol.DateTo == null ? "" : vol.DateTo.Value.ToString("MM/dd/yyyy"))));
                r.Cells[++cellIndex].FirstParagraph.AppendChild(new Run(wordDoc, vol.NoOfHours.ToString()));
                r.Cells[++cellIndex].FirstParagraph.AppendChild(new Run(wordDoc, vol.PositionNatureOfWork.ToMultilineString()));
            }

            t.GetChildNodes(NodeType.Run, true).ToArray().ToList().ForEach(x => ((Run)x).Font.Size = 7);
        }

        public void CustomizeDoc_Aspose_PDSOtherInfo(object data, ref Aspose.Words.Document wordDoc)
        {
            DocumentBuilder builder = new DocumentBuilder(wordDoc);

            Aspose.Words.Bookmark bookmark;
            Aspose.Words.Tables.Table t;
            Aspose.Words.Tables.Row row, r;

            bookmark = wordDoc.Range.Bookmarks["tableRef_Skills"];
            Node _t = bookmark.BookmarkStart.GetAncestor(NodeType.Table);

            t = (Aspose.Words.Tables.Table)_t;

            r = null;
            row = (Aspose.Words.Tables.Row)t.Rows[t.Rows.Count - 1].Clone(true);

            for (int i = 1; i <= skill_ext.Count; i++)
            {
                tblEmployee_OtherInfo_Skill oi = skill_ext[i - 1];

                if (i > 1)
                {
                    t.Rows.Add(row.Clone(true));
                }

                r = t.Rows[t.Rows.Count - 1];

                int cellIndex = 0;

                r.Cells[cellIndex].FirstParagraph.AppendChild(new Run(wordDoc, oi.Description));
            }

            bookmark = wordDoc.Range.Bookmarks["tableRef_NonAcademics"];
            _t = bookmark.BookmarkStart.GetAncestor(NodeType.Table);

            t = (Aspose.Words.Tables.Table)_t;

            r = null;

            for (int i = 1; i <= non_ext.Count; i++)
            {
                tblEmployee_OtherInfo_NonAcademic oi = non_ext[i - 1];

                if ((i + 2) > t.Rows.Count)
                {
                    t.Rows.Add(row.Clone(true));
                    r = t.Rows[t.Rows.Count - 1];
                }
                else
                {
                    r = t.Rows[i + 1];
                }

                int cellIndex = 1;

                r.Cells[cellIndex].FirstParagraph.AppendChild(new Run(wordDoc, oi.Description));
            }

            bookmark = wordDoc.Range.Bookmarks["tableRef_Memberships"];
            _t = bookmark.BookmarkStart.GetAncestor(NodeType.Table);

            t = (Aspose.Words.Tables.Table)_t;

            r = null;

            for (int i = 1; i <= mem_ext.Count; i++)
            {
                tblEmployee_OtherInfo_Membership oi = mem_ext[i - 1];

                if ((i + 2) > t.Rows.Count)
                {
                    t.Rows.Add(row.Clone(true));
                    r = t.Rows[t.Rows.Count - 1];
                }
                else
                {
                    r = t.Rows[i + 1];
                }

                int cellIndex = 2;

                r.Cells[cellIndex].FirstParagraph.AppendChild(new Run(wordDoc, oi.Description));
            }

            t.GetChildNodes(NodeType.Run, true).ToArray().ToList().ForEach(x => ((Run)x).Font.Size = 7);
        }

    }
}
