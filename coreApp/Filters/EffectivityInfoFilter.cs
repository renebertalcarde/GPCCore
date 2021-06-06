using coreApp.Interfaces;
using Module.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace coreApp.Filters
{

    public class EffectivityInfoFilterAttribute : ActionFilterAttribute
    {
        string tableName = "";

        public EffectivityInfoFilterAttribute(string tableName)
        {
            this.tableName = tableName;
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            try
            {
                string v = coreProcs.getRouteParam(filterContext.HttpContext.Request, "dt", "").Trim();

                string effectivity = TMSettings.DEFAULT_EFFECTIVITY;
                List<DateTime> dates = new List<DateTime>();

                if (tableName == "TMSettings_Effectivities")
                {
                    dates = Cache.Get_Tables().TMSettings_Effectivities.Select(x => x.DateEffective).Distinct().OrderByDescending(x => x).ToList();
                }
                
                if (dates.Count > 0)
                {
                    effectivity = dates.First().ToString("MM-dd-yyyy");
                }

                filterContext.Controller.ViewBag.Effectivities = dates;

                if (string.IsNullOrEmpty(v))
                {
                    effectivity = DateTime.Today.ToString("MM-dd-yyyy");
                }
                else if (v.ToUpper() == TMSettings.DEFAULT_EFFECTIVITY)
                {
                    effectivity = TMSettings.DEFAULT_EFFECTIVITY;
                }
                else if (v != null)
                {
                    DateTime dt;

                    if (DateTime.TryParse(v, out dt))
                    {
                        effectivity = dt.ToString("MM-dd-yyyy");
                    }
                }

                ((IEffectivityController)filterContext.Controller).effectivity = effectivity;
                filterContext.Controller.ViewBag.Effectivity = effectivity;

                base.OnActionExecuting(filterContext);
            }
            catch (Exception e)
            {
                filterContext.HttpContext.Session["ApplicationErrorMessage"] = coreProcs.ShowErrors(e);
                filterContext.Result = new RedirectResult(Procs.vdUrl("/Error"));
            }
        }

    }
}