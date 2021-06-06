using coreApp.Interfaces;
using Module.Core;
using Module.DB;
using System;
using System.Linq;
using System.Web.Mvc;

namespace coreApp.Filters
{

    public class StakeholderFilterAttribute : ActionFilterAttribute
    {

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            try
            {
                string v = coreProcs.getRouteParam(filterContext.HttpContext.Request, "stakeholderId");
                
                if (v != null)
                {
                    using (dalDataContext context = new dalDataContext())
                    {
                        tblStakeholder stakeholder = context.tblStakeholders.Where(x => x.StakeholderId == int.Parse(v)).SingleOrDefault();
                        if (stakeholder == null)
                        {
                            throw new Exception(ModuleConstants.RECORD_ID_NOT_FOUND);
                        }
                        else
                        {
                            filterContext.Controller.ViewBag.Stakeholder = stakeholder;
                            filterContext.Controller.TempData["Stakeholder"] = stakeholder;
                            ((IStakeholderController)filterContext.Controller).stakeholder = stakeholder;
                        }
                    }
                }

                base.OnActionExecuting(filterContext);
            }
            catch(Exception e)
            {
                filterContext.HttpContext.Session["ApplicationErrorMessage"] = coreProcs.ShowErrors(e);
                filterContext.Result = new RedirectResult(Procs.vdUrl("/Error"));
            }
        }

    }
}