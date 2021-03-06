using System.Web.Mvc;

namespace coreApp.Areas.API
{
    public class APIAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "API";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "API_AddressList",
                "api/address/list/{type}/{parentId}",
                new { controller = "Address", action = "GetList", type = UrlParameter.Optional, parentId = UrlParameter.Optional }
            );

            context.MapRoute(
                "API_default",
                "API/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}