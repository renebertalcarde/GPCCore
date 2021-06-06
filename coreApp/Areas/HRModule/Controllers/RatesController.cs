using System.Linq;
using System.Web.Mvc;
using System;
using coreApp.DAL;
using coreApp.Controllers;
using coreApp.Filters;
using coreApp.Interfaces;
using coreLib.Objects;
using Module.DB;
using Module.Core;
using System.Data.Linq;

namespace coreApp.Areas.HRModule.Controllers
{
    [UserAccessAuthorize(allowedAccess: "hr_config")]
    [PositionInfoFilter]
    [RateSettingsFilter]
    public class RatesController : HLBase_AuthorizedController, IPositionController
    {
        public tblPosition position { get; set; }

        public RatesController()
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
            ViewBag.Position = position;

            var model = Cache.Get_Tables().Rates.Where(x => x.PositionId == position.PositionId && x.OfficeId == -1).OrderByDescending(x => x.DateEffective).ToList();
            return View(model);
        }

        public ActionResult _Details(int? id)
        {
            ViewBag.Title = "Rate";
            

            ViewBag.uiIsReadOnly = true;
            ViewBag.uiIncludeId = true;

            tblRate Rate = Cache.Get_Tables().Rates.Where(x => x.RateId == id).SingleOrDefault();
            if (Rate == null)
            {
                return new Raw_ActionResult(ModuleConstants.ID_NOT_FOUND);
            }
            else
            {
                return PartialView("_Rate", Rate);
            }
        }

        public ActionResult _Create()
        {
            ViewBag.Title = "Rate";
            

            ViewBag.uiIsReadOnly = false;
            ViewBag.uiIncludeId = false;

            return PartialView("_Rate", new tblRate { PositionId = position.PositionId, PositionLevel = 1, DateEffective = System.DateTime.Now });
        }

        public void _CreatePost(FormCollection _model, ref queryResult res)
        {
            tblRate model = (tblRate)UpdateModel(new tblRate().GetType(), _model);

            using (dalDataContext context = new dalDataContext())
            {
                if (ModelState.IsValid)
                {

                    tblRate Rate = new tblRate
                    {
                        OfficeId = -1,
                        PositionId = model.PositionId,
                        PositionLevel = model.PositionLevel,
                        DateEffective = model.DateEffective,
                        Rate = model.Rate,
                        IsDailyRate = model.IsDailyRate,
                        SecondaryRate = model.SecondaryRate
                    };

                    context.tblRates.InsertOnSubmit(Rate);
                    context.SubmitChanges();

                    res.Remarks = "Record was successfully created";

                    Cache.Update_Tables(_rates: true);

                    var data = new
                    {
                        JobTitle = string.Format("[{0}] {1}", Rate.PositionId, Rate.Position)
                    };

                    MvcApplication.HLLog.WriteLog("Rates", "Insert Rate",
                             string.Format("Inserted record [{0}] {1}", Rate.RateId, Rate.DateEffective.ToShortDateString()),
                             data);
                }
                else
                {
                    throw new Exception(coreProcs.ShowErrors(ModelState));
                }
            }
        }

        public ActionResult _Edit(int? id)
        {
            ViewBag.Title = "Rate";
            

            ViewBag.uiIsReadOnly = false;
            ViewBag.uiIncludeId = true;

            tblRate Rate = Cache.Get_Tables().Rates.Where(x => x.RateId == id).Single();
            if (Rate == null)
            {
                return new Raw_ActionResult(ModuleConstants.ID_NOT_FOUND);
            }
            else
            {
                return PartialView("_Rate", Rate);
            }
        }


        public void _EditPost(FormCollection _model, ref queryResult res)
        {
            tblRate model = (tblRate)UpdateModel(new tblRate().GetType(), _model);

            using (dalDataContext context = new dalDataContext())
            {
                if (ModelState.IsValid)
                {

                    tblRate Rate = context.tblRates.Where(x => x.RateId == model.RateId).SingleOrDefault();
                    if (Rate == null)
                    {
                        throw new Exception(ModuleConstants.ID_NOT_FOUND);
                    }
                    else
                    {
                        Rate.OfficeId = -1;
                        Rate.PositionId = model.PositionId;
                        Rate.PositionLevel = model.PositionLevel;
                        Rate.DateEffective = model.DateEffective;
                        Rate.Rate = model.Rate;
                        Rate.IsDailyRate = model.IsDailyRate;
                        Rate.SecondaryRate = model.SecondaryRate;

                        ModifiedMemberInfo[] mmi = context.tblRates.GetModifiedMembers(Rate);
                        context.SubmitChanges();

                        res.Remarks = "Record was successfully updated";

                        Cache.Update_Tables(_rates: true);

                        var data = new
                        {
                            JobTitle = string.Format("[{0}] {1}", Rate.PositionId, Rate.Position),
                            Changes = mmi.Select(x => new { FieldName = x.Member.Name, Orig = x.OriginalValue, Current = x.CurrentValue })
                        };

                        MvcApplication.HLLog.WriteLog("Rates", "Update Rate",
                                 string.Format("Updated record [{0}] {1}", Rate.RateId, Rate.DateEffective.ToShortDateString()),
                                 data);
                    }
                }
                else
                {
                    throw new Exception(coreProcs.ShowErrors(ModelState));
                }
            }
        }


        public void _Delete(int id, ref queryResult res)
        {
            using (dalDataContext context = new dalDataContext())
            {
                if (ModelState.IsValid)
                {

                    tblRate Rate = context.tblRates.Where(x => x.RateId == id).SingleOrDefault();
                    if (Rate == null)
                    {
                        throw new Exception(ModuleConstants.ID_NOT_FOUND);
                    }
                    else
                    {
                        context.tblRates.DeleteOnSubmit(Rate);
                        context.SubmitChanges();

                        res.Remarks = "Record was successfully deleted";

                        Cache.Update_Tables(_rates: true);

                        var data = new
                        {
                            JobTitle = string.Format("[{0}] {1}", Rate.PositionId, Rate.Position)
                        };

                        MvcApplication.HLLog.WriteLog("Rates", "Delete Rate",
                                 string.Format("Deleted record [{0}] {1}", Rate.RateId, Rate.DateEffective.ToShortDateString()),
                                 data);
                    }

                }
                else
                {
                    throw new Exception(coreProcs.ShowErrors(ModelState));
                }
            }
        }
    }
}
