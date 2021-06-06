using System.Linq;
using System.Net;
using System.Web.Mvc;
using System;
using coreApp.Controllers;
using coreApp;
using coreLib.Objects;
using Module.DB;
using Module.Core;
using coreApp.DAL;

namespace coreApp.Areas.HRModule.Controllers
{
    [UserAccessAuthorize(allowedAccess: "hr_config")]
    public class GeoAuthAreasController : HLBase_AuthorizedController
    {
        public GeoAuthAreasController()
        {
            IndexProc = new IndexDelegate(_Index);
            DetailsProc = new DetailsDelegate(_Details);
            CreateProc = new CreateDelegate(_Create);
            CreatePostProc = new CreatePostDelegate(_CreatePost);
            EditProc = new EditDelegate(_Edit);
            EditPostProc = new EditPostDelegate(_EditPost);
            DeletePostProc = new DeletePostDelegate(_Delete);
        }

        public ActionResult _Index()
        {
            using (dalDataContext context = new dalDataContext())
            {
                var model = context.tblGeoAuth_Areas.OrderBy(x => x.AreaName).ToList();
                return View(model);
            }
        }

        public ActionResult _Details(int? id)
        {
            ViewBag.Title = "Area";
            

            ViewBag.uiIsReadOnly = true;
            ViewBag.uiIncludeId = true;


            using (dalDataContext context = new dalDataContext())
            {
                tblGeoAuth_Area area = context.tblGeoAuth_Areas.Where(x => x.Id == id).SingleOrDefault();
                if (area == null)
                {
                    return new Raw_ActionResult(ModuleConstants.ID_NOT_FOUND);
                }
                else
                {
                    return PartialView("_Area", area);
                }
            }
           
        }

        public ActionResult _Create()
        {
            ViewBag.Title = "Area";
            

            ViewBag.uiIsReadOnly = false;
            ViewBag.uiIncludeId = false;

            return PartialView("_Area", new tblGeoAuth_Area { GMaps_LatLng = FixedSettings.GMaps_DefaultLatLng });
        }

        public void _CreatePost(FormCollection _model, ref queryResult res)
        {
            tblGeoAuth_Area model = (tblGeoAuth_Area)UpdateModel(new tblGeoAuth_Area().GetType(), _model);

            using (dalDataContext context = new dalDataContext())
            {
                if (ModelState.IsValid)
                {
                    tblGeoAuth_Area area = new tblGeoAuth_Area
                    {
                        AreaName = model.AreaName,
                        Description = model.Description,
                        Latitude = model.Latitude,
                        Longitude = model.Longitude,
                        Radius = model.Radius
                    };

                    context.tblGeoAuth_Areas.InsertOnSubmit(area);
                    context.SubmitChanges();
                    
                    res.Remarks = "Record was successfully created";
                    
                }
                else
                {
                    throw new Exception(coreProcs.ShowErrors(ModelState));
                }
            }
        }

        public ActionResult _Edit(int? id)
        {
            ViewBag.Title = "Area";
            

            ViewBag.uiIsReadOnly = false;
            ViewBag.uiIncludeId = true;

            using (dalDataContext context = new dalDataContext())
            {
                tblGeoAuth_Area area = context.tblGeoAuth_Areas.Where(x => x.Id == id).Single();
                if (area == null)
                {
                    return new Raw_ActionResult(ModuleConstants.ID_NOT_FOUND);
                }
                else
                {
                    return PartialView("_Area", area);
                }
            }
        }


        public void _EditPost(FormCollection _model, ref queryResult res)
        {
            tblGeoAuth_Area model = (tblGeoAuth_Area)UpdateModel(new tblGeoAuth_Area().GetType(), _model);

            using (dalDataContext context = new dalDataContext())
            {
                if (ModelState.IsValid)
                {

                    tblGeoAuth_Area area = context.tblGeoAuth_Areas.Where(x => x.Id == model.Id).SingleOrDefault();
                    if (area == null)
                    {
                        throw new Exception(ModuleConstants.ID_NOT_FOUND);
                    }
                    else
                    {
                        area.AreaName = model.AreaName;
                        area.Description = model.Description;
                        area.Latitude = model.Latitude;
                        area.Longitude = model.Longitude;
                        area.Radius = model.Radius;

                        context.SubmitChanges();
                        
                        res.Remarks = "Record was successfully updated";
                        
                    }

                }
                else
                {
                    throw new Exception(coreProcs.ShowErrors(ModelState));
                }
            }
        }

        public void _Delete(int id, ref queryResult res)
        {
            using (dalDataContext context = new dalDataContext())
            {
                if (ModelState.IsValid)
                {

                    tblGeoAuth_Area area = context.tblGeoAuth_Areas.Where(x => x.Id == id).SingleOrDefault();
                    if (area == null)
                    {
                        throw new Exception(ModuleConstants.ID_NOT_FOUND);
                    }
                    else
                    {
                        context.tblGeoAuth_Areas.DeleteOnSubmit(area);
                        context.SubmitChanges();

                        res.Remarks = "Record was successfully deleted";
                        
                    }

                }
                else
                {
                    throw new Exception(coreProcs.ShowErrors(ModelState));
                }
            }
        }
    }
}
