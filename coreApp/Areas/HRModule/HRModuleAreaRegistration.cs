using System.Web.Mvc;

namespace coreApp.Areas.HRModule
{
    public class HRModuleAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "HRModule";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
             "AttendanceMonitoring",
             "HR/AttendanceMonitoring/{action}/{id}",
             new { controller = "AttendanceMonitoring", action = "Index", id = UrlParameter.Optional }
           );

            context.MapRoute(
             "PeriodAttendance",
             "HR/PeriodAttendance/{id}",
             new { controller = "AttendanceMonitoring", action = "PeriodAttendance", id = UrlParameter.Optional }
           );

            context.MapRoute(
            "Employees",
            "HR/Manage/Employees",
            new { controller = "Employees", action = "Index" }
            );

            context.MapRoute(
             "Employee",
             "HR/Manage/Employee/{action}/{id}",
             new { controller = "Employees", action = "Details", id = UrlParameter.Optional }
           );

            context.MapRoute(
             "BulletinBoardDocTypes",
             "HR/Manage/AnnouncementDocTypes/{action}/{id}",
             new { controller = "BulletinBoardDocTypes", action = "Index", id = UrlParameter.Optional }
           );

            context.MapRoute(
              "BulletinBoard",
              "HR/Manage/Announcements/{action}/{id}",
              new { controller = "BulletinBoard", action = "Index", id = UrlParameter.Optional }
            );

            context.MapRoute(
              "BulletinBoardView",
              "HR/Announcements/{action}/{id}",
              new { controller = "BulletinBoardView", action = "Index", id = UrlParameter.Optional }
            );

            context.MapRoute(
              "TravelApplications",
              "HR/OnlineApplications/Travels/{action}/{id}",
              new { controller = "TravelApplications", action = "Index", id = UrlParameter.Optional }
            );

            context.MapRoute(
             "OTApplications",
             "HR/OnlineApplications/Overtimes/{action}/{id}",
             new { controller = "OTApplications", action = "Index", id = UrlParameter.Optional }
           );

            context.MapRoute(
             "LeaveApplications",
             "HR/OnlineApplications/Leaves/{action}/{id}",
             new { controller = "LeaveApplications", action = "Index", id = UrlParameter.Optional }
           );

            context.MapRoute(
           "Trainings",
           "HR/Batch/Trainings/{action}/{id}",
           new { controller = "Trainings", action = "Index", id = UrlParameter.Optional }
         );

            context.MapRoute(
           "TrainingParticipants",
           "HR/Batch/Trainings/Participants/{action}/{trainingId}/{id}",
           new { controller = "TrainingParticipants", action = "Index", trainingId = UrlParameter.Optional, id = UrlParameter.Optional }
         );

            context.MapRoute(
              "SetUserAccounts",
              "HR/Batch/CreateEmployeeAccounts/{action}",
              new { controller = "SetUserAccounts", action = "Index" }
            );

            context.MapRoute(
              "SetHRPermissions",
              "HR/Batch/UpdatePermissions/{action}",
              new { controller = "SetHRPermissions", action = "Index" }
            );

            context.MapRoute(
             "SetAllowancesDeductions",
             "HR/Batch/AllowancesAndDeductions/{action}",
             new { controller = "SetAllowancesDeductions", action = "Index" }
           );

            context.MapRoute(
              "SetGeoAuthAreas",
              "HR/Batch/GeoAuthenticationAreas/{action}",
              new { controller = "SetGeoAuthAreas", action = "Index" }
            );

            context.MapRoute(
                "SalaryGrades",
                "HR/Config/SalaryGrades/{action}/{dt}",
                new { controller = "SalaryGrades", action = "Details", dt = UrlParameter.Optional }
            );

            context.MapRoute(
                "EmployeeAttendance",
                "HR/Query/Employee/Attendance/{action}/{employeeId}/{startDate}/{endDate}/{filters}",
                new { controller = "EmployeeAttendance", action = "Index", employeeId = UrlParameter.Optional, startDate = UrlParameter.Optional, endDate = UrlParameter.Optional, filters = UrlParameter.Optional }
            );

            context.MapRoute(
                "HRPermissions",
                "HR/Manage/Employee/Permissions/{action}/{employeeId}",
                new { controller = "HRPermissions", action = "Index", employeeId = UrlParameter.Optional }
            );

            context.MapRoute(
               "EmployeeAccess",
               "HR/Manage/Employee/Access/{action}/{employeeId}/{id}",
               new { controller = "EmployeeAccess", action = "Index", employeeId = UrlParameter.Optional, id = UrlParameter.Optional }
           );

            context.MapRoute(
                "EmployeeAccount",
                "HR/Manage/Employee/Account/{action}/{employeeId}/{id}",
                new { controller = "EmployeeAccount", action = "Index", employeeId = UrlParameter.Optional, id = UrlParameter.Optional }
            );

            context.MapRoute(
               "EmployeeAllowancesDeductions",
               "HR/Manage/Employee/AllowancesDeductions/{action}/{employeeId}/{id}",
               new { controller = "EmployeeAllowancesDeductions", action = "Index", employeeId = UrlParameter.Optional, id = UrlParameter.Optional }
           );

            context.MapRoute(
                "EmployeeCivilServices",
                "HR/Manage/Employee/CivilServices/{action}/{employeeId}/{id}",
                new { controller = "EmployeeCivilServices", action = "Index", employeeId = UrlParameter.Optional, id = UrlParameter.Optional }
            );

            context.MapRoute(
               "EmployeeEducation",
               "HR/Manage/Employee/Educ/{action}/{employeeId}/{id}",
               new { controller = "EmployeeEduc", action = "Index", employeeId = UrlParameter.Optional, id = UrlParameter.Optional }
           );

            context.MapRoute(
               "EmployeeIssuances",
               "HR/Manage/Employee/Issuances/{action}/{employeeId}/{id}",
               new { controller = "EmployeeIssuances", action = "Index", employeeId = UrlParameter.Optional, id = UrlParameter.Optional }
           );

            context.MapRoute(
                "EmployeeChildren",
                "HR/Manage/Employee/Children/{action}/{employeeId}/{id}",
                new { controller = "EmployeeChildren", action = "Index", employeeId = UrlParameter.Optional, id = UrlParameter.Optional }
            );

            context.MapRoute(
                "EmployeeGroups",
                "HR/Manage/Employee/Groups/{action}/{employeeId}/{id}",
                new { controller = "EmployeeGroups", action = "Index", employeeId = UrlParameter.Optional, id = UrlParameter.Optional }
            );

            context.MapRoute(
               "EmployeeReferences",
               "HR/Manage/Employee/References/{action}/{employeeId}/{id}",
               new { controller = "EmployeeReferences", action = "Index", employeeId = UrlParameter.Optional, id = UrlParameter.Optional }
           );

            context.MapRoute(
                "EmployeeFiles",
                "HR/Manage/Employee/Files/{action}/{employeeId}/{id}",
                new { controller = "EmployeeFiles", action = "Index", employeeId = UrlParameter.Optional, id = UrlParameter.Optional }
            );

            context.MapRoute(
              "EmployeeVoluntary",
              "HR/Manage/Employee/Voluntary/{action}/{employeeId}/{id}",
              new { controller = "EmployeeVoluntary", action = "Index", employeeId = UrlParameter.Optional, id = UrlParameter.Optional }
          );

            context.MapRoute(
                "EmployeeWorkExps",
                "HR/Manage/Employee/WorkExps/{action}/{employeeId}/{id}",
                new { controller = "EmployeeWorkExps", action = "Index", employeeId = UrlParameter.Optional, id = UrlParameter.Optional }
            );

            context.MapRoute(
                "EmployeeTrainings",
                "HR/Manage/Employee/Trainings/{action}/{employeeId}/{id}",
                new { controller = "EmployeeTrainings", action = "Index", employeeId = UrlParameter.Optional, id = UrlParameter.Optional }
            );

            context.MapRoute(
               "EmployeeTravels",
               "HR/Manage/Employee/Travels/{action}/{employeeId}/{id}",
               new { controller = "EmployeeTravels", action = "Index", employeeId = UrlParameter.Optional, id = UrlParameter.Optional }
           );

            context.MapRoute(
              "EmployeeOT",
              "HR/Manage/Employee/OT/{action}/{employeeId}/{id}",
              new { controller = "EmployeeOT", action = "Index", employeeId = UrlParameter.Optional, id = UrlParameter.Optional }
          );

            context.MapRoute(
             "EmployeeRD",
             "HR/Manage/Employee/RD/{action}/{employeeId}/{id}",
             new { controller = "EmployeeRD", action = "Index", employeeId = UrlParameter.Optional, id = UrlParameter.Optional }
         );

            context.MapRoute(
                "EmployeePhotos",
                "HR/Manage/Employee/Photos/{action}/{employeeId}",
                new { controller = "EmployeePhotos", action = "Index", employeeId = UrlParameter.Optional }
            );

            context.MapRoute(
                "EmployeeInfo",
                "HR/Manage/Employee/BasicInformation/{action}/{employeeId}/{id}",
                new { controller = "EmployeeInfo", action = "Index", employeeId = UrlParameter.Optional, id = UrlParameter.Optional }
            );

            context.MapRoute(
               "EmployeeSkills",
               "HR/Manage/Employee/Skills/{action}/{employeeId}/{id}",
               new { controller = "EmployeeSkills", action = "Index", employeeId = UrlParameter.Optional, id = UrlParameter.Optional }
           );

            context.MapRoute(
              "EmployeeNonAcademics",
              "HR/Manage/Employee/NonAcademics/{action}/{employeeId}/{id}",
              new { controller = "EmployeeNonAcademics", action = "Index", employeeId = UrlParameter.Optional, id = UrlParameter.Optional }
          );

            context.MapRoute(
              "EmployeeMemberships",
              "HR/Manage/Employee/Memberships/{action}/{employeeId}/{id}",
              new { controller = "EmployeeMemberships", action = "Index", employeeId = UrlParameter.Optional, id = UrlParameter.Optional }
          );

            context.MapRoute(
                "EmployeeCareers",
                "HR/Manage/Employee/Career/{action}/{employeeId}/{id}",
                new { controller = "EmployeeCareers", action = "Index", employeeId = UrlParameter.Optional, id = UrlParameter.Optional }
            );
            
            context.MapRoute(
              "Groups",
              "HR/Config/Groups/{action}/{id}",
              new { controller = "Groups", action = "Index", id = UrlParameter.Optional }
          );

            context.MapRoute(
              "GroupMembers",
              "HR/Config/Group/Members/{action}/{groupId}/{id}",
              new { controller = "GroupMembers", action = "Index", groupId = UrlParameter.Optional, id = UrlParameter.Optional }
          );

            context.MapRoute(
             "OfficeStructure",
             "HR/Config/OfficeStructure/{action}/{id}",
             new { controller = "OfficeStructure", action = "Index", id = UrlParameter.Optional }
         );

            context.MapRoute(
              "MFOs",
              "HR/Config/OfficeGroups/{action}/{id}",
              new { controller = "MFOs", action = "Index", id = UrlParameter.Optional }
          );

            context.MapRoute(
              "Offices",
              "HR/Config/Offices/{action}/{id}",
              new { controller = "Offices", action = "Index", id = UrlParameter.Optional }
          );

            context.MapRoute(
               "Departments",
               "HR/Config/Office/Departments/{action}/{officeId}/{id}",
               new { controller = "Departments", action = "Index", officeId = UrlParameter.Optional, id = UrlParameter.Optional }
           );

            context.MapRoute(
             "Positions",
             "HR/Config/Positions/{action}/{id}",
             new { controller = "Positions", action = "Index", id = UrlParameter.Optional }
         );

            context.MapRoute(
              "Rates",
              "HR/Config/Position/Rates/{action}/{positionId}/{id}",
              new { controller = "Rates", action = "Index", positionId = UrlParameter.Optional, id = UrlParameter.Optional }
          );

            context.MapRoute(
            "GeoAuthAreas",
            "HR/Config/GeoLocationAreas/{action}/{id}",
            new { controller = "GeoAuthAreas", action = "Index", id = UrlParameter.Optional }
        );

            context.MapRoute(
            "Signatories",
            "HR/Config/Signatories/{action}/{id}",
            new { controller = "Signatories", action = "Index", id = UrlParameter.Optional }
        );

            context.MapRoute(
                "HRModule_default",
                "HRModule/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}