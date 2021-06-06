using Module.DB;
using System;
using System.Globalization;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace coreApp
{
    public class MvcApplication : System.Web.HttpApplication
    {
        public static cachedTables CachedTables;

        public static string VirtualDirectory;

        public static objAccountabilityExt SysLog;

        protected void Application_Start()
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");

            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            Cache.Get_Tables();

            SysLog = new objAccountabilityExt("Sys");

            string tmp = HttpRuntime.AppDomainAppVirtualPath;
            VirtualDirectory = tmp != "/" && tmp != "" ? tmp : "";

        }

        protected void Session_Start()
        {
            FixedSettings.Init();
        }

        void Application_Error(object sender, EventArgs e)
        {
            var httpContext = ((MvcApplication)sender).Context;
            var currentController = " ";
            var currentAction = " ";
            var currentRouteData = RouteTable.Routes.GetRouteData(new HttpContextWrapper(httpContext));

            if (currentRouteData != null)
            {
                if (currentRouteData.Values["controller"] != null && !String.IsNullOrEmpty(currentRouteData.Values["controller"].ToString()))
                {
                    currentController = currentRouteData.Values["controller"].ToString();
                }

                if (currentRouteData.Values["action"] != null && !String.IsNullOrEmpty(currentRouteData.Values["action"].ToString()))
                {
                    currentAction = currentRouteData.Values["action"].ToString();
                }
            }

            var ex = Server.GetLastError();

            coreProcs.SaveExceptionLog(Server.MapPath("~/"), ex);

            bool skiperrorredirect = false;
            if (Session["skiperrorredirect"] != null)
            {
                skiperrorredirect = (bool)Session["skiperrorredirect"];
            }

            if (skiperrorredirect)
            {
                Session["skiperrorredirect"] = null;
                return;
            };

            var routeData = new RouteData();
            var action = "GenericError";

            if (ex is HttpException)
            {
                var httpEx = ex as HttpException;

                switch (httpEx.GetHttpCode())
                {
                    case 404:
                        action = "NotFound";
                        break;

                        // others if any
                }
            }

            httpContext.ClearError();
            httpContext.Response.Clear();
            httpContext.Response.TrySkipIisCustomErrors = true;

            routeData.Values["controller"] = "Error";
            routeData.Values["action"] = action;
            routeData.Values["exception"] = new HandleErrorInfo(ex, currentController, currentAction);

            IController errormanagerController = new coreApp.Controllers.ErrorController();
            HttpContextWrapper wrapper = new HttpContextWrapper(httpContext);
            var rc = new RequestContext(wrapper, routeData);
            errormanagerController.Execute(rc);
        }
    }
}
