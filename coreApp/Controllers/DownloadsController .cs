using System.Web.Mvc;

namespace coreApp.Controllers
{
    [Authorize]
    public class DownloadsController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
    }
}