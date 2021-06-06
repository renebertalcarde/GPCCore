using CommonAPI.Common;
using HRLogixMobileLib;
using System;
using System.Linq;
using System.Web.Mvc;

namespace CommonAPI.Controllers
{
    public class AddressController : Controller
    {
        public ActionResult Refresh()
        {
            queryResult res = new queryResult { IsSuccessful = true, Data = null, Err = "", Remarks = "" };

            try
            {
                Procs.Refresh();
            }
            catch (Exception ex)
            {
                res.IsSuccessful = false;
                res.Err = coreLib.Procs.ShowErrors(ex);
            }

            return Json(res, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetList(int type)
        {
            queryResult res = new queryResult { IsSuccessful = true, Data = null, Err = "", Remarks = "" };

            ListType lt = (ListType)type;

            try
            {
                switch (lt)
                {
                    case ListType.Country:
                        res.Data = MvcApplication.ADDR.Countries.OrderBy(x => x.Country).ToArray();
                        break;
                    case ListType.Region:
                        res.Data = MvcApplication.ADDR.Regions.OrderBy(x => x.Region).ToArray();
                        break;
                    case ListType.Province:
                        res.Data = MvcApplication.ADDR.Provinces.OrderBy(x => x.Province).ToArray();
                        break;
                    case ListType.MunCity:
                        res.Data = MvcApplication.ADDR.MunCities.OrderBy(x => x.City).ToArray();
                        break;
                    case ListType.vwCity:
                        res.Data = MvcApplication.ADDR.vwCities.OrderBy(x => x.City).ToArray();
                        break;
                    case ListType.Brgy:
                        res.Data = MvcApplication.ADDR.Brgies.OrderBy(x => x.Brgy).ToArray();
                        break;
                }
            }
            catch (Exception ex)
            {
                res.IsSuccessful = false;
                res.Err = coreLib.Procs.ShowErrors(ex);
            }

            return Json(res, JsonRequestBehavior.AllowGet);
        }
    }
}