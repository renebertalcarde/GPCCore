using System;
using System.Linq;
using System.Web.Mvc;
using coreApp.Filters;
using coreApp.Controllers;
using coreApp.DAL;
using coreApp.Interfaces;
using coreLib.Objects;
using Module.DB;
using Module.Core;
using System.Collections.Generic;

namespace coreApp.Areas.HRModule.Controllers
{
    [EmployeeInfoFilter]
    [EmployeeAuthorize]
    public class EmployeeTrainingsController : HLBase_AuthorizedController, IEmployeeController
    {
        public tblEmployee employee { get; set; }

        public EmployeeTrainingsController()
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

                List<tblEmployee_Training> model = Procs.GetEmployeeTrainings(employee.EmployeeId);
                return View(model);
            }
        }

        public ActionResult _Details(int? id)
        {
            ViewBag.Title = "Training";
            

            ViewBag.uiIsReadOnly = true;
            ViewBag.uiIncludeId = true;

            using (hr2017DataContext context = new hr2017DataContext())
            {
                tblEmployee_Training Training = context.tblEmployee_Trainings.Where(x => x.TrainingId == id).SingleOrDefault();
                if (Training == null)
                {
                    tblTrainingParticipant p = context.tblTrainingParticipants.Where(x => x.EmployeeId == employee.EmployeeId && x.TrainingId == id).SingleOrDefault();
                    if (p == null)
                    {
                        return new Raw_ActionResult(ModuleConstants.ID_NOT_FOUND);
                    }

                    Training = new tblEmployee_Training(p);
                }

                return PartialView("_Training", Training);
            }
        }

        public ActionResult _Create()
        {
            ViewBag.Title = "Training";
            

            ViewBag.uiIsReadOnly = false;
            ViewBag.uiIncludeId = false;

            tblEmployee_Training model = new tblEmployee_Training
            {
                EmployeeId = employee.EmployeeId
            };

            return PartialView("_Training",model);
        }

        public void _CreatePost(FormCollection _model, ref queryResult res)
        {
            tblEmployee_Training model = (tblEmployee_Training)UpdateModel(new tblEmployee_Training().GetType(), _model);

            using (hr2017DataContext context = new hr2017DataContext())
            {
                if (!Cache.Get().userAccess.HasPermission())
                {
                    AddError(ModuleConstants_Authorization.USERACCESS_UNAUTHORIZED_ACTION);
                }

                if (model.DateFrom > model.DateTo)
                {
                    AddError("From date cannot be later than To date");
                }

                if (ModelState.IsValid)
                {

                    tblEmployee_Training Training = new tblEmployee_Training
                    {
                        EmployeeId = model.EmployeeId,
                        Title = model.Title,
                        DateFrom = model.DateFrom,
                        DateTo = model.DateTo,
                        NoOfHours = model.NoOfHours,
                        Type = model.Type,
                        ConductedBy = model.ConductedBy
                    };

                    context.tblEmployee_Trainings.InsertOnSubmit(Training);
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
            ViewBag.Title = "Training";
            

            ViewBag.uiIsReadOnly = false;
            ViewBag.uiIncludeId = true;

            using (hr2017DataContext context = new hr2017DataContext())
            {
                tblEmployee_Training Training = context.tblEmployee_Trainings.Where(x => x.EmployeeId == employee.EmployeeId && x.TrainingId == id).Single();
                if (Training == null)
                {
                    return new Raw_ActionResult(ModuleConstants.ID_NOT_FOUND);
                }
                else
                {
                    return PartialView("_Training",Training);
                }
            }
        }


        public void _EditPost(FormCollection _model, ref queryResult res)
        {
            tblEmployee_Training model = (tblEmployee_Training)UpdateModel(new tblEmployee_Training().GetType(), _model);

            using (hr2017DataContext context = new hr2017DataContext())
            {
                tblEmployee_Training Training = context.tblEmployee_Trainings.Where(x => x.TrainingId == model.TrainingId).SingleOrDefault();
                if (Training == null)
                {
                    throw new Exception(ModuleConstants.ID_NOT_FOUND);
                }
                else
                {
                    if (!Cache.Get().userAccess.HasPermission())
                    {
                        AddError(ModuleConstants_Authorization.USERACCESS_UNAUTHORIZED_ACTION);
                    }

                    if (model.DateFrom > model.DateTo)
                    {
                        AddError("From date cannot be later than To date");
                    }

                    if (!model.PDSEntry)
                    {
                        AddError("Cannot modify staff-encoded training record");
                    }
                    
                    if (ModelState.IsValid)
                    {

                        Training.Title = model.Title;
                        Training.DateFrom = model.DateFrom;
                        Training.DateTo = model.DateTo;
                        Training.NoOfHours = model.NoOfHours;
                        Training.Type = model.Type;
                        Training.ConductedBy = model.ConductedBy;

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

                tblEmployee_Training Training = context.tblEmployee_Trainings.Where(x => x.TrainingId == id).SingleOrDefault();
                if (Training == null)
                {
                    throw new Exception(ModuleConstants.ID_NOT_FOUND);
                }

                if (!Training.PDSEntry)
                {
                    AddError("Cannot delete staff-encoded training record");
                }

                if (ModelState.IsValid)
                {
                    context.tblEmployee_Trainings.DeleteOnSubmit(Training);
                    context.SubmitChanges();

                    res.Remarks = "Record was successfully deleted";
                }
                else
                {
                    throw new Exception(coreProcs.ShowErrors(ModelState));
                }
            }
        }
    }
}
