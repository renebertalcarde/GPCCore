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
using coreApp.Filters;
using coreApp.Interfaces;

namespace coreApp.Areas.HRModule.Controllers
{
    [UserAccessAuthorize(allowedAccess: "hr_config")]
    [EffectivityInfoFilter("tblSalaryGrades")]
    [RateSettingsFilter]
    public class SalaryGradesController : Base_NoCoreHRStaffController, IEffectivityController
    {
        public string effectivity { get; set; }

        public ActionResult Details()
        {
            return View(getModel());
        }

        public ActionResult Edit(DateTime? dt = null)
        {
            ViewBag.IsNew = false;
            return PartialView("_SalaryGrades", getModel());
        }

        public ActionResult Create()
        {
            ViewBag.IsNew = true;
            return PartialView("_SalaryGrades", newModel());
        }

        public ActionResult CreateCopy()
        {
            ViewBag.IsNew = true;
            return PartialView("_SalaryGrades", getModel());
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Save(FormCollection form, bool IsNew)
        {
            queryResult res = new queryResult { IsSuccessful = true, Data = null, Err = "", Remarks = "" };
            
            try
            {
                DateTime effective = DateTime.Parse(form["DateEffective"]);
                List<tblSalaryGrade> model = newModel(effective);

                foreach (tblSalaryGrade item in model)
                {
                    double tmp;

                    string fn = string.Format("SG_{0}_1", item.SalaryGrade);
                    if (double.TryParse(form[fn], out tmp)) item.Step1 = tmp; else AddError(string.Format("Invalid value at Salary Grade {0} > Step 1", item.SalaryGrade));

                    fn = string.Format("SG_{0}_2", item.SalaryGrade);
                    if (double.TryParse(form[fn], out tmp)) item.Step2 = tmp; else AddError(string.Format("Invalid value at Salary Grade {0} > Step 2", item.SalaryGrade));

                    fn = string.Format("SG_{0}_3", item.SalaryGrade);
                    if (double.TryParse(form[fn], out tmp)) item.Step3 = tmp; else AddError(string.Format("Invalid value at Salary Grade {0} > Step 3", item.SalaryGrade));

                    fn = string.Format("SG_{0}_4", item.SalaryGrade);
                    if (double.TryParse(form[fn], out tmp)) item.Step4 = tmp; else AddError(string.Format("Invalid value at Salary Grade {0} > Step 4", item.SalaryGrade));

                    fn = string.Format("SG_{0}_5", item.SalaryGrade);
                    if (double.TryParse(form[fn], out tmp)) item.Step5 = tmp; else AddError(string.Format("Invalid value at Salary Grade {0} > Step 5", item.SalaryGrade));

                    fn = string.Format("SG_{0}_6", item.SalaryGrade);
                    if (double.TryParse(form[fn], out tmp)) item.Step6 = tmp; else AddError(string.Format("Invalid value at Salary Grade {0} > Step 6", item.SalaryGrade));

                    fn = string.Format("SG_{0}_7", item.SalaryGrade);
                    if (double.TryParse(form[fn], out tmp)) item.Step7 = tmp; else AddError(string.Format("Invalid value at Salary Grade {0} > Step 7", item.SalaryGrade));

                    fn = string.Format("SG_{0}_8", item.SalaryGrade);
                    if (double.TryParse(form[fn], out tmp)) item.Step8 = tmp; else AddError(string.Format("Invalid value at Salary Grade {0} > Step 8", item.SalaryGrade));
                }

                using (dalDataContext context = new dalDataContext())
                {
                    var existing = context.tblSalaryGrades.Where(x => x.DateEffective == effective);

                    if (IsNew)
                    {
                        if (existing.Any())
                        {
                            AddError("Table already exists with this effectivity");
                        }
                    }

                    if (ModelState.IsValid)
                    {   
                        context.tblSalaryGrades.DeleteAllOnSubmit(existing);

                        context.tblSalaryGrades.InsertAllOnSubmit(model);
                        context.SubmitChanges();

                        res.Remarks = "Table was successfully updated";

                        Cache.Update_Tables(_salarygrades: true);

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
                
        [HttpPost]
        public ActionResult Delete(string dt)
        {
            queryResult res = new queryResult { IsSuccessful = true, Data = null, Err = "", Remarks = "" };

            try
            {
                
                using (dalDataContext context = new dalDataContext())
                {

                    if (dt == TMSettings.DEFAULT_EFFECTIVITY)
                    {
                        AddError("Cannot delete default table");
                    }

                    if (ModelState.IsValid)
                    {
                        DateTime effective = coreLib.Constants.DEFAULT_DATETIME;

                        effective = DateTime.Parse(dt);

                        var existing = context.tblSalaryGrades.Where(x => x.DateEffective == effective);
                        context.tblSalaryGrades.DeleteAllOnSubmit(existing);


                        context.SubmitChanges();

                        res.Remarks = "Table was successfully deleted";

                        Cache.Update_Tables(_salarygrades: true);

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

        private List<tblSalaryGrade> getModel()
        {
            List<tblSalaryGrade> model;
            DateTime _effective = coreLib.Constants.DEFAULT_DATETIME;

            List<DateTime> dates = ((List<DateTime>)ViewBag.Effectivities).ToList();
            dates.Add(coreLib.Constants.DEFAULT_DATETIME);

            using (dalDataContext context = new dalDataContext())
            {
                if (effectivity != TMSettings.DEFAULT_EFFECTIVITY)
                {
                    _effective = Convert.ToDateTime(effectivity);
                }

                if (!dates.Contains(_effective))
                {
                    _effective = dates.Where(x => x <= _effective).OrderByDescending(x => x).First();

                    ViewBag.Effectivity = _effective == coreLib.Constants.DEFAULT_DATETIME ? TMSettings.DEFAULT_EFFECTIVITY : _effective.ToString("MM-dd-yyyy");
                }

                model = context.tblSalaryGrades.Where(x => x.DateEffective == _effective).OrderBy(x => x.SalaryGrade).ToList();

                if (model.Count == 0)
                {
                    model = newModel();
                }
            }

            return model;
        }

        private List<tblSalaryGrade> newModel(DateTime? effective = null)
        {
            List<tblSalaryGrade> model = new List<tblSalaryGrade>();

            for (int i = 1; i <= 33; i++)
            {
                model.Add(new tblSalaryGrade
                {
                    DateEffective = effective == null ? coreLib.Constants.DEFAULT_DATETIME : effective.Value,
                    SalaryGrade = i,
                    Step1 = 0,
                    Step2 = 0,
                    Step3 = 0,
                    Step4 = 0,
                    Step5 = 0,
                    Step6 = 0,
                    Step7 = 0,
                    Step8 = 0
                });
            }

            return model;
        }
    }
}