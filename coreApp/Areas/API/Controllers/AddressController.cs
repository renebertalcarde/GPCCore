using HRLogixMobileLib;
using Module.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace coreApp.Areas.API.Controllers
{
    public class AddressController : Controller
    {
        public ActionResult GetList(int type, int parentId)
        {
            queryResult res = new queryResult { IsSuccessful = true, Data = null, Err = "", Remarks = "" };

            AddressListType lt = (AddressListType)type;

            try
            {
                switch (lt)
                {
                    case AddressListType.Region:
                        res.Data = Cache.Get_Tables().ADDR.Regions.Where(x => x.CountryId == parentId).ToArray();
                        break;
                    case AddressListType.Province:
                        res.Data =
                            Cache.Get_Tables().ADDR.Regions.Where(x => x.CountryId == parentId)
                            .Join(Cache.Get_Tables().ADDR.Provinces, a => a.Id, b => b.RegionId, (a, b) => b)
                            .OrderBy(x => x.Province)
                            .ToArray();
                        break;
                    case AddressListType.MunCity:
                        res.Data = Cache.Get_Tables().ADDR.MunCities.Where(x => x.ProvinceId == parentId).ToArray();
                        break;
                    case AddressListType.vwCity:
                        res.Data = Cache.Get_Tables().ADDR.vwCities.Where(x => x.ProvinceId == parentId).ToArray();
                        break;
                    case AddressListType.Brgy:
                        res.Data = Cache.Get_Tables().ADDR.Brgies.Where(x => x.CityId == parentId).ToArray();
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