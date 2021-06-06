using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace coreApp
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
               name: "ActivityLogs",
               url: "ActivityLogs/{module}/{startDate}/{endDate}",
               defaults: new { controller = "ActivityLogs", action = "Index", module = UrlParameter.Optional, startDate = UrlParameter.Optional, endDate = UrlParameter.Optional }
           );

            routes.MapRoute(
               name: "ActivityLogsDetails",
               url: "ActivityLog/Details/{id}",
               defaults: new { controller = "ActivityLogs", action = "Details", id = UrlParameter.Optional }
           );

            routes.MapRoute(
               name: "HLSettings",
               url: "HL/Settings/{action}/{id}",
               defaults: new { controller = "Settings", action = "Index", id = UrlParameter.Optional }
           );

            routes.MapRoute(
               name: "HLHome",
               url: "HL/{action}/{id}",
               defaults: new { controller = "HLHome", action = "Index", id = UrlParameter.Optional }
           );

            routes.MapRoute(
            name: "IdVerify",
            url: "IdVerify/{codeData}",
            defaults: new { area = "", controller = "Home", action = "IdVerify", codeData = UrlParameter.Optional }
        );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
