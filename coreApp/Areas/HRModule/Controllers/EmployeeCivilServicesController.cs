using System;
using System.Linq;
using System.Web.Mvc;
using coreApp.Filters;
using coreApp.DAL;
using coreApp.Controllers;
using coreApp.Interfaces;
using coreLib.Objects;
using Module.DB;
using Module.Core;
using System.Collections.Generic;

namespace coreApp.Areas.HRModule.Controllers
{
    [EmployeeInfoFilter]
    [EmployeeAuthorize]
    public class EmployeeCivilServicesController : HLBase_AuthorizedController, IEmployeeController
    {
        public tblEmployee employee { get; set; }

        public EmployeeCivilServicesController()
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
            using (hr2017DataContext context = new hr2017DataContext())
            {

                var model = context.tblEmployee_CivilServices.Where(x => x.EmployeeId == employee.EmployeeId).OrderBy(x => x.DateOfExam).ToList();
                return View(model);
            }
        }


        public ActionResult _Details(int? id)
        {
            ViewBag.Title = "Civil Service";
            

            ViewBag.uiIsReadOnly = true;
            ViewBag.uiIncludeId = true;

            using (hr2017DataContext context = new hr2017DataContext())
            {
                tblEmployee_CivilService CS = context.tblEmployee_CivilServices.Where(x => x.EmployeeId == employee.EmployeeId && x.CSId == id).SingleOrDefault();
                if (CS == null)
                {
                    return new Raw_ActionResult(ModuleConstants.ID_NOT_FOUND);
                }
                else
                {
                    return PartialView("_CS", CS);
                }
            }
        }

        public ActionResult _Create()
        {
            ViewBag.Title = "Civil Service";
            

            ViewBag.uiIsReadOnly = false;
            ViewBag.uiIncludeId = false;

            tblEmployee_CivilService model = new tblEmployee_CivilService
            {
                EmployeeId = employee.EmployeeId
            };

            return PartialView("_CS", model);
        }

        public void _CreatePost(FormCollection _model, ref queryResult res)
        {
            tblEmployee_CivilService model = (tblEmployee_CivilService)UpdateModel(new tblEmployee_CivilService().GetType(), _model);

            using (hr2017DataContext context = new hr2017DataContext())
            {
                if (!Cache.Get().userAccess.HasPermission())
                {
                    AddError(ModuleConstants_Authorization.USERACCESS_UNAUTHORIZED_ACTION);
                }

                if (ModelState.IsValid)
                {

                    tblEmployee_CivilService CivilService = new tblEmployee_CivilService
                    {
                        EmployeeId = model.EmployeeId,
                        CareerService = model.CareerService,
                        Rating = model.Rating,
                        DateOfExam = model.DateOfExam,
                        POE_Address = model.POE_Address,
                        POE_CountryId = model.POE_CountryId,
                        POE_PostalCode = model.POE_PostalCode,
                        LicenseNo = model.LicenseNo,
                        LicenseReleaseDate = model.LicenseReleaseDate
                    };

                    context.tblEmployee_CivilServices.InsertOnSubmit(CivilService);
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
            ViewBag.Title = "Civil Service";
            

            ViewBag.uiIsReadOnly = false;
            ViewBag.uiIncludeId = true;

            using (hr2017DataContext context = new hr2017DataContext())
            {
                tblEmployee_CivilService CivilService = context.tblEmployee_CivilServices.Where(x => x.EmployeeId == employee.EmployeeId && x.CSId == id).Single();
                if (CivilService == null)
                {
                    return new Raw_ActionResult(ModuleConstants.ID_NOT_FOUND);
                }
                else
                {
                    return PartialView("_CS", CivilService);
                }
            }
        }


        public void _EditPost(FormCollection _model, ref queryResult res)
        {
            tblEmployee_CivilService model = (tblEmployee_CivilService)UpdateModel(new tblEmployee_CivilService().GetType(), _model);

            using (hr2017DataContext context = new hr2017DataContext())
            {
                tblEmployee_CivilService CivilService = context.tblEmployee_CivilServices.Where(x => x.CSId == model.CSId).SingleOrDefault();
                if (CivilService == null)
                {
                    throw new Exception(ModuleConstants.ID_NOT_FOUND);
                }
                else
                {
                    if (!Cache.Get().userAccess.HasPermission())
                    {
                        AddError(ModuleConstants_Authorization.USERACCESS_UNAUTHORIZED_ACTION);
                    }

                    if (ModelState.IsValid)
                    {

                        CivilService.CareerService = model.CareerService;
                        CivilService.Rating = model.Rating;
                        CivilService.DateOfExam = model.DateOfExam;
                        CivilService.POE_Address = model.POE_Address;
                        CivilService.POE_CountryId = model.POE_CountryId;
                        CivilService.POE_PostalCode = model.POE_PostalCode;
                        CivilService.LicenseNo = model.LicenseNo;
                        CivilService.LicenseReleaseDate = model.LicenseReleaseDate;

                        context.SubmitChanges();

                        res.Remarks = "Record was successfully updated";
                    }
                    else
                    {
                        throw new Exception(coreProcs.ShowErrors(ModelState));
                    }
                }
            }
        }

        public void _Delete(int id, ref queryResult res)
        {
            using (hr2017DataContext context = new hr2017DataContext())
            {
                if (!Cache.Get().userAccess.HasPermission())
                {
                    AddError(ModuleConstants_Authorization.USERACCESS_UNAUTHORIZED_ACTION);
                }

                if (ModelState.IsValid)
                {

                    tblEmployee_CivilService CivilService = context.tblEmployee_CivilServices.Where(x => x.CSId == id).SingleOrDefault();
                    if (CivilService == null)
                    {
                        throw new Exception(ModuleConstants.ID_NOT_FOUND);
                    }
                    else
                    {
                        context.tblEmployee_CivilServices.DeleteOnSubmit(CivilService);
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
