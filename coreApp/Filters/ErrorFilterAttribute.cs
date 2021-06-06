using Module.Core;
using System.Web.Mvc;

namespace coreApp.Filters
{
    public class ErrorFilterAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            if (filterContext.Exception != null)
            {
                coreProcs.SaveExceptionLog(filterContext.HttpContext.Server.MapPath("~/"), filterContext.Exception);
                
                string msg = coreProcs.ShowErrors(filterContext.Exception);

                filterContext.HttpContext.Session["ApplicationErrorMessage"] = msg;
                if (filterContext.HttpContext.Request.UrlReferrer != null)
                {
                    filterContext.HttpContext.Session["error_backurl"] = filterContext.HttpContext.Request.UrlReferrer.AbsoluteUri;
                }
            }
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {

        }

    }
}