using System.Linq;
using System.Web.Mvc;

namespace coreApp.Controllers
{
    public class DefaultController : Base_NoCoreAuthorizedController
    {
        public ActionResult Index()
        {
            return View("~/Areas/AdminModule/Views/Offices/Index.cshtml");
        }
    }
}