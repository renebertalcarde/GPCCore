using System.Web.Mvc;

namespace coreApp.Filters
{

    public class SSLAttribute : ActionFilterAttribute
    {

        public override void OnActionExecuted(ActionExecutedContext filterContext)
        { }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {   
            if (!filterContext.HttpContext.Request.IsSecureConnection && !filterContext.HttpContext.IsDebuggingEnabled && FixedSettings.UseSSL)
            {
                string securedUrl = filterContext.HttpContext.Request.Url.OriginalString.Replace("http", "https").Replace(":80", "");
                filterContext.HttpContext.Response.Redirect(securedUrl);
            }
        }

    }
}