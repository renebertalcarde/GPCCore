using System.Linq;
using System.Web.Mvc;
using System;
using coreApp.Controllers;
using System.Collections.Generic;
using Module.DB;
using coreLib.Objects;
using coreLib.Enums;
using Module.Core;

namespace coreApp.Areas.HRModule.Controllers
{
    [UserAccessAuthorize("admin")]
    public class ActivityLogsController : Base_NoCoreHRStaffController
    {
        public ActionResult Index(string module, string startDate, string endDate)
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

            using (accountabilityDataContext context = new accountabilityDataContext())
            {
                ViewBag.Module = string.IsNullOrEmpty(module) ? "Sys" : module;
                ViewBag.Period = pm;

                List<tblAccountability> model = context.tblAccountabilities.Where(x => x.Module == module && x.LogDate >= pm.StartDate && x.LogDate <= pm.EndDate).OrderByDescending(x => x.LogDate).ToList();

                return View(model);
            }
        }

        public ActionResult Details(int id)
        {
            using (accountabilityDataContext context = new accountabilityDataContext())
            {
                tblAccountability model = context.tblAccountabilities.Where(x => x.Id == id).SingleOrDefault();
                if (model == null)
                {
                    return new Raw_ActionResult(ModuleConstants.ID_NOT_FOUND);
                }

                return PartialView(model);
            }
        }
    }
}