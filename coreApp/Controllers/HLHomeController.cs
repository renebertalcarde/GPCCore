using System.Web.Mvc;

namespace coreApp.Controllers
{
    public class HLHomeController : HLBase_NoCoreAuthorizedController
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetStakeholderPhoto(string type, int stakeholderId)
        {
            return GetPhoto(type, stakeholderId);
        }
    }
}