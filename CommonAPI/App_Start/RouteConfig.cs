using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace CommonAPI
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
               name: "AddressList",
               url: "address/list/{type}",
               defaults: new { controller = "Address", action = "GetList", type = UrlParameter.Optional }
           );

            routes.MapRoute(
               name: "AddressRefresh",
               url: "address/refresh",
               defaults: new { controller = "Address", action = "Refresh" }
           );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
