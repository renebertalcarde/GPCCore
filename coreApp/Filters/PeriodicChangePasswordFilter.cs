using System;
using System.Web.Mvc;

namespace coreApp.Filters
{

    public class PeriodicChangePasswordFilterAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            try
            {
                UserAccess access = coreApp.Cache.Get().userAccess;
                DateTime? lastPWChange = access.stakeholder.LastPasswordChange;

                if (lastPWChange == null)
                {
                    GotoChangePW(ref filterContext);
                }
                else if (FixedSettings.PasswordChangeInterval > 0)
                {
                    double days = (DateTime.Today - lastPWChange.Value).TotalDays;
                    if ( days > FixedSettings.PasswordChangeInterval)
                    {
                        GotoChangePW(ref filterContext);
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

        void GotoChangePW(ref ActionExecutingContext filterContext)
        {
            filterContext.Result = new RedirectToRouteResult(new System.Web.Routing.RouteValueDictionary(
                new
                {
                    area = "",
                    action = "ForceChangePassword",
                    controller = "Manage",
                    onMobile = false,
                    returnUrl = filterContext.HttpContext.Request.Url.AbsolutePath
                }
                ));
        }

    }
}