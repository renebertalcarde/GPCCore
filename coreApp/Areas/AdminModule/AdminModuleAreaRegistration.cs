using System.Web.Mvc;

namespace coreApp.Areas.AdminModule
{
    public class AdminModuleAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "AdminModule";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {

            context.MapRoute(
                "ForceUpdate",
                "Admin/ForceUpdate",
                new { controller = "Main", action = "ForceUpdate" }
            );

            context.MapRoute(
                "UpdateLogs",
                "Admin/UpdateLogs/{startDate}/{endDate}",
                new { controller = "Main", action = "UpdateLogs", startDate = UrlParameter.Optional, endDate = UrlParameter.Optional }
            );

            context.MapRoute(
                "AdminModule_default",
                "AdminModule/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}