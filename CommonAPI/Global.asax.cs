using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace CommonAPI
{
    public class MvcApplication : System.Web.HttpApplication
    {
        public static AddressData ADDR;

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            ADDR = new AddressData();
        }
    }
}
