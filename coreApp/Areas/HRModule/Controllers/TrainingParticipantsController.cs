using System.Linq;
using System.Web.Mvc;
using System;
using coreApp.DAL;
using coreApp.Controllers;
using System.Collections.Generic;
using Module.DB;
using coreApp.Filters;
using coreApp.Interfaces;
using Module.Core;
using Module.DB.Procs;

namespace coreApp.Areas.HRModule.Controllers
{
    [TrainingInfoFilter]
    public class TrainingParticipantsController : Base_NoCoreHRStaffController, ITrainingController
    {
        public tblTraining training { get; set; }

        public ActionResult Index()
        {
            using (hr2017DataContext context = new hr2017DataContext())
            {
                List<tblEmployee> model = training.GetParticipants();
                return View(model);
            }
        }

        public ActionResult AddEmployees()
        {
            return PartialView();
        }

        [HttpPost]
        public ActionResult AddEmployees(int[] employeeIds)
        {
            queryResult res = new queryResult { IsSuccessful = true, Data = null, Remarks = "", Err = "" };

            try
            {
                if (!employeeIds.Any())
                {
                    throw new Exception("No employee selected");
                }

                using (dalDataContext _context = new dalDataContext())
                {
                    using (hr2017DataContext context = new hr2017DataContext())
                    {
                        List<int> ids = new List<int>();
                        foreach(int id in employeeIds)
                        {
                            if (!context.tblTrainingParticipants.Any(x => x.EmployeeId == id && x.TrainingId == training.Id))
                            {
                                ids.Add(id);
                            }
                        }

                        List<tblTrainingParticipant> participants = ids.Select(x => new tblTrainingParticipant
                        {
                            EmployeeId = x,
                            TrainingId = training.Id
                        })
                        .ToList();

                        context.tblTrainingParticipants.InsertAllOnSubmit(participants);
                        context.SubmitChanges();
                            
                       
                        res.Remarks = "Employees were successfully added";                       
                    }
                }
            }
            catch (Exception e)
            {
                res.IsSuccessful = false;
                res.Err = coreProcs.ShowErrors(e);
            }

            return Json(res);
        }

        [HttpPost]
        public ActionResult DeleteEmployees(int[] employeeIds)
        {
            queryResult model = new queryResult { IsSuccessful = true, Data = null, Remarks = "", Err = "" };

            try
            {
                if (!employeeIds.Any())
                {
                    throw new Exception("No employee selected");
                }

                using (hr2017DataContext context = new hr2017DataContext())
                {
                    var tmp = context.tblTrainingParticipants.Where(x => x.TrainingId == training.Id && employeeIds.Contains(x.EmployeeId));

                    context.tblTrainingParticipants.DeleteAllOnSubmit(tmp);
                    context.SubmitChanges();

                    model.Remarks = "Selected employees were successfully removed from this list";
                }
            }
            catch (Exception e)
            {
                model.IsSuccessful = false;
                model.Err = coreProcs.ShowErrors(e);
            }

            return Json(model);
        }
    }
}