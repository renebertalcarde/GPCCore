using System.Web;
using System.Web.Mvc;

namespace coreApp.Controllers
{
    public class ErrorController : Base
    {
        public ActionResult Index()
        {
            return RedirectToAction("GenericError", new HandleErrorInfo(new HttpException(""), "ErrorController", "GenericError"));
        }

        public ActionResult GenericError(HandleErrorInfo exception)
        {
            return View("Error", exception);
        }

        public ViewResult NotFound(HandleErrorInfo exception)
        {
            ViewBag.Title = "Page Not Found";
            return View("Error", exception);
        }
    }
}