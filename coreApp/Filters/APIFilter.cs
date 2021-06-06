using System;
using System.Web.Mvc;

namespace coreApp.Filters
{

    public class APIFilterAttribute : ActionFilterAttribute
    {

        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            var result = filterContext.Result as JsonResult;
            if (result == null)
            {
                // The controller action didn't return a view result 
                // => no need to continue any further
                return;
            }

            var model = result.Data as queryResult;
            if (model == null)
            {
                // there's no model or the model was not of the expected type 
                // => no need to continue any further
                return;
            }

            model.ServerTime = DateTime.Now;

            base.OnActionExecuted(filterContext);
        }

    }
}