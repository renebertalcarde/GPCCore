using System.Linq;
using System.Net;
using System.Web.Mvc;
using System;
using coreApp.Controllers;
using coreApp;
using coreLib.Objects;
using Module.DB;
using Module.Core;
using System.Collections.Generic;
using coreApp.DAL;

namespace coreApp.Areas.HRModule.Controllers
{
    [UserAccessAuthorize(allowedAccess: "hr_config")]
    public class SignatoriesController : Base_NoCoreHRStaffController
    {
        public ActionResult Index()
        {
            List<tblSignatory> model = getModel();
            return View(model);
        }

        public ActionResult Edit()
        {
            List<tblSignatory> model = getModel();
            return View(model);
        }

        private List<tblSignatory> getModel()
        {
            using (hr2017DataContext context = new hr2017DataContext())
            {
                List<tblSignatory> model = context.tblSignatories
                    .OrderBy(x => x.ModuleName)
                    .ThenBy(x => x.ReportName)
                    .ThenBy(x => x.FieldNo)
                    .ToList();
                return model;
            }
        }

        [HttpGet]
        public ActionResult GetEmployeeInfo(int id)
        {
            queryResult res = new queryResult { IsSuccessful = true, Data = null, Err = "", Remarks = "" };

            try
            {
                using (dalDataContext context = new dalDataContext())
                {
                    if (ModelState.IsValid)
                    {

                        tblEmployee employee = context.tblEmployees.Where(x => x.EmployeeId == id).SingleOrDefault();
                        if (employee != null)
                        {
                            tblEmployee_Career career = new Module.DB.Procs.procs_tblEmployee(employee).LatestCareer();
                            if (career != null)
                            {
                                if (career.Position != null)
                                {
                                    res.Data = new string[] {
                                        employee.FullName_FN,
                                        career.Position.Position
                                    };
                                }
                            }
                        }
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

            return Json(res, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Save(string modifiedData, string addedData, string deletedData)
        {
            queryResult res = new queryResult { IsSuccessful = true, Data = null, Err = "", Remarks = "" };

            try
            {
                using (hr2017DataContext context = new hr2017DataContext())
                {
                    if (ModelState.IsValid)
                    {
                        tblSignatory[] added = new tblSignatory[] { };
                        tblSignatory[] modified = new tblSignatory[] { };
                        List<tblSignatory> deleted = new List<tblSignatory>();
                        string[] _deleted = new string[] { };

                        Dictionary<string, object[]> mmis = new Dictionary<string, object[]>();

                        if (!string.IsNullOrEmpty(modifiedData))
                        {
                            modified = Newtonsoft.Json.JsonConvert.DeserializeObject<tblSignatory[]>(modifiedData);
                            foreach (tblSignatory m in modified)
                            {
                                tblSignatory sig = context.tblSignatories.Where(x => x.ModuleName == m.ModuleName && x.ReportName == m.ReportName && x.FieldNo == m.FieldNo).SingleOrDefault();
                                if (sig != null)
                                {
                                    sig.EmployeeId = m.EmployeeId;
                                    sig.TitleMode = m.TitleMode;
                                    sig.CustomTitle = m.IsCustomTitle ? m.CustomTitle : null;

                                    mmis.Add(string.Format("[{0}] {1}, Field #{2}", sig.Id, sig.ReportName, sig.FieldNo),
                                        context.tblSignatories.GetModifiedMembers(sig).Select(x => new { FieldName = x.Member.Name, Orig = x.OriginalValue, Current = x.CurrentValue }).ToArray());

                                }
                            }
                        }
                        
                        if (!string.IsNullOrEmpty(addedData))
                        {
                            added = Newtonsoft.Json.JsonConvert.DeserializeObject<tblSignatory[]>(addedData);
                            foreach (tblSignatory m in added)
                            {
                                if (!m.IsCustomTitle)
                                {
                                    m.CustomTitle = null;
                                }
                            }

                            context.tblSignatories.InsertAllOnSubmit(added);
                        }

                        if (!string.IsNullOrEmpty(deletedData))
                        {
                            _deleted = Newtonsoft.Json.JsonConvert.DeserializeObject<string[]>(deletedData);
                            foreach (string s in _deleted)
                            {
                                string[] tmp = s.Split('|');
                                tblSignatory sig = context.tblSignatories.Where(x => x.ModuleName == tmp[0] && x.ReportName == tmp[1] && x.FieldNo == int.Parse(tmp[2])).SingleOrDefault();
                                context.tblSignatories.DeleteOnSubmit(sig);

                                deleted.Add(sig);
                            }
                        }

                        context.SubmitChanges();

                        res.Remarks = "Signatories were successfully saved";

                        if (added.Any() || modified.Any() || deleted.Any())
                        {
                            var _data = new
                            {
                                Added = added,
                                Modified = mmis.Where(x => ((object[])x.Value).Any())
                                            .Select(x => new
                                            {
                                                Record = x.Key,
                                                Changes = x.Value
                                            })
                                            .ToArray(),
                                Deleted = deleted.ToArray()
                            };

                            MvcApplication.HLLog.WriteLog("Signatories", "Update Signatories",
                                            string.Format("Updated {0} records", _data.Added.Length + _data.Modified.Length + _data.Deleted.Length),    
                                            _data);
                        }
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
    }
}