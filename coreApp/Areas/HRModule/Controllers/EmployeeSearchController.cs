using System.Linq;
using System.Web.Mvc;
using System;
using System.Collections.Generic;
using coreApp.Controllers;
using coreApp.DAL;
using Module.DB;
using Module.DB.Procs;
using coreApp.Models;

namespace coreApp.Areas.HRModule.Controllers
{
    public class EmployeeSearchController : HLBase_NoCoreAuthorizedController
    {
        public ActionResult GetList(string lastName = "", string firstName = "", int mfoId = -1, string departmentIds = "", int positionId = -1, int groupId = -1, int employmentType = -1, bool excludeNoCareer = false, bool excludeNoOffice = false, string altSource = "", string active = "", bool noPaging = false, bool forScheduling = false, string nodepartment = "", bool includeLocations = false, int country = -1, int province = -1, int city = -1, int brgy = -1)
        {
            EmployeeSearch search = new EmployeeSearch(altSource, lastName, firstName, mfoId, departmentIds, positionId, groupId, employmentType, excludeNoCareer, excludeNoOffice, active, forScheduling, nodepartment, includeLocations, country, province, city, brgy);

            List<tblEmployee> model = search.GetEmployees();
            
            ViewBag.NoPaging = noPaging;
            ViewBag.IncludeLocations = includeLocations;

            Session["EmployeeSearch"] = search;
            Session["EmployeeSearchResult"] = model;

            return PartialView("List", model);
        }

        [ChildActionOnly]
        public ActionResult Search(coreApp.Models.EmployeeSearchUIModel model)
        {
            return PartialView(model);
        }
    }
}
