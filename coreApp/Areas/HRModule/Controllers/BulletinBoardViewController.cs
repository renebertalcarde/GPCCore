using System.Linq;
using System.Web.Mvc;
using System;
using coreApp.Controllers;
using coreLib.Objects;
using System.Collections.Generic;
using Module.Core;
using coreLib.Enums;
using Module.DB;

namespace coreApp.Areas.HRModule.Controllers
{
    public class BulletinBoardViewController : HLBase_NoCoreAuthorizedController
    {
        public List<tblBulletinBoard> WidgetList(int count = 6)
        {
            using (dalDataContext context = new dalDataContext())
            {
                PeriodModel pm = new PeriodModel(PeriodModelInitType.Last30Days);

                var tmp = context.tblBulletinBoards
                    .Where(x => x.Enabled && x.ShowInDashboard)
                    .OrderByDescending(x => x.DateOfPosting)
                    .ThenBy(x => x.Title)
                    .ToList();

                List<tblBulletinBoard> model = new List<tblBulletinBoard>();

                if (tmp.Count > count)
                {
                    model = tmp.Where(x => pm.Within(x.DateOfPosting)).Take(count).ToList();
                }
                else
                {
                    model = tmp;
                }

                return model;
            }
        }

        public ActionResult Index()
        {
            using (dalDataContext context = new dalDataContext())
            {
                var model = context.tblBulletinBoards
                    .Where(x => x.Enabled)
                    .OrderByDescending(x => x.DateOfPosting).ThenBy(x => x.Title)
                    .ToList();
                return View(model);
            }
        }

        public ActionResult Details(int id)
        {
            using (dalDataContext context = new dalDataContext())
            {
                var model = context.tblBulletinBoards.Where(x => x.Id == id).SingleOrDefault();
                if (model == null)
                {
                    throw new Exception(ModuleConstants.ID_NOT_FOUND);
                }

                if (!model.Enabled)
                {
                    throw new Exception("This post has been disabled");
                }

                return View(model);
            }
        }

    }
}
