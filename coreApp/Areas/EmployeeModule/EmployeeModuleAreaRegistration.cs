using System.Web.Mvc;

namespace coreApp.Areas.EmployeeModule
{
    public class EmployeeModuleAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "EmployeeModule";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "MyProfile",
                "My/Profile",
                new { controller = "MyProfile", action = "Details" }
            );

            context.MapRoute(
               "MyInfo",
               "My/BasicInformation",
               new { controller = "MyInfo", action = "Details" }
            );

            context.MapRoute(
               "MyInfo_PDS",
               "My/PDS",
               new { controller = "MyInfo", action = "Print" }
            );

            context.MapRoute(
                "MyCareers",
                "My/Career/{action}",
                new { controller = "MyCareers", action = "Index" }
            );

            context.MapRoute(
              "MyChildren",
              "My/Children",
              new { controller = "MyChildren", action = "Index" }
           );

            context.MapRoute(
             "MyEduc",
             "My/Educ",
             new { controller = "MyEduc", action = "Index" }
          );

            context.MapRoute(
             "MyWorkExps",
             "My/WorkExperiences",
             new { controller = "MyWorkExps", action = "Index" }
          );

            context.MapRoute(
             "MyTrainings",
             "My/Trainings",
             new { controller = "MyTrainings", action = "Index" }
          );

            context.MapRoute(
            "MyCivilServices",
            "My/CivilServices",
            new { controller = "MyCivilServices", action = "Index" }
         );

            context.MapRoute(
            "MyVoluntary",
            "My/VoluntaryWorks",
            new { controller = "MyVoluntary", action = "Index" }
         );

            context.MapRoute(
            "MyOtherInfo",
            "My/OtherInformation",
            new { controller = "MyOtherInfo", action = "Details" }
         );

            context.MapRoute(
            "MyInfo_PDSQuestions",
            "My/PDSFormQuestions",
            new { controller = "MyInfo", action = "PDSQuestions" }
         );

            context.MapRoute(
           "MyReferences",
           "My/References",
           new { controller = "MyReferences", action = "Index" }
        );

            context.MapRoute(
              "MyPhotos",
              "My/Photos",
              new { controller = "MyPhotos", action = "Details" }
           );

            context.MapRoute(
              "MySchedules",
              "My/Schedules/{startDate}/{endDate}",
              new { controller = "MySchedules", action = "Index", startDate = UrlParameter.Optional, endDate = UrlParameter.Optional }
           );

            context.MapRoute(
                "MyDeviceIdNos",
                "My/DeviceIdNos",
                new { controller = "MyDeviceIdNos", action = "Index" }
            );

            context.MapRoute(
                "MyAllowancesDeductions",
                "My/AllowancesAndDeductions",
                new { controller = "MyAllowancesDeductions", action = "Index" }
            );

            context.MapRoute(
                "MyGroups",
                "My/Groups",
                new { controller = "MyGroups", action = "Index" }
            );

            context.MapRoute(
               "MyIssuances",
               "My/Issuances",
               new { controller = "MyIssuances", action = "Index" }
            );

            context.MapRoute(
                "MyGeoAuth_Kiosk",
                "My/GeoLocation/AttendanceKiosk",
                new { controller = "MyGeoAuth", action = "Index" }
            );

            context.MapRoute(
              "MyGeoAuth_Device",
              "My/GeoLocation/DeviceRegistration",
              new { controller = "MyGeoAuth", action = "RegisterIndex" }
           );

            context.MapRoute(
              "MyHolidays",
              "Holidays/{action}/{startDate}/{endDate}/{id}",
              new { controller = "Holidays", action = "Index", id = UrlParameter.Optional, startDate = UrlParameter.Optional, endDate = UrlParameter.Optional }
           );

            context.MapRoute(
               "MyPayslips",
               "My/Payslips/{action}/{id}",
               new { controller = "MyPayslips", action = "Index", id = UrlParameter.Optional }
            );

            context.MapRoute(
               "MyTimeLogs",
               "My/TimeLogs/{action}/{startDate}/{endDate}",
               new { controller = "MyTimeLogs", action = "Index", startDate = UrlParameter.Optional, endDate = UrlParameter.Optional }
            );

            context.MapRoute(
               "MyLeaveApplications",
               "My/LeaveApplications/{action}/{id}",
               new { controller = "MyLeaveApplications", action = "Index", id = UrlParameter.Optional }
            );

            context.MapRoute(
               "MyTravelApplications",
               "My/TravelApplications/{action}/{id}",
               new { controller = "MyTravelApplications", action = "Index", id = UrlParameter.Optional }
            );

            context.MapRoute(
              "MyOTApplications",
              "My/OTApplications/{action}/{id}",
              new { controller = "MyOTApplications", action = "Index", id = UrlParameter.Optional }
           );

            context.MapRoute(
               "MyLeaveCredits",
               "My/LeaveCredits/{action}/{leaveTypeId}/{startDate}/{endDate}",
               new { controller = "MyLeaveCredits", action = "Index", leaveTypeId = UrlParameter.Optional, startDate = UrlParameter.Optional, endDate = UrlParameter.Optional }
           );

            context.MapRoute(
                "MyAttendance",
                "My/Attendance/{action}/{startDate}/{endDate}/{filters}",
                new { controller = "MyAttendance", action = "Index", startDate = UrlParameter.Optional, endDate = UrlParameter.Optional, filters = UrlParameter.Optional }
            );

            context.MapRoute(
                "MyRestDays",
                "My/RestDays",
                new { controller = "MyRD", action = "Index" }
            );

            context.MapRoute(
                "MyTravels",
                "My/Travels",
                new { controller = "MyTravels", action = "Index" }
            );

            context.MapRoute(
                "MyOT",
                "My/Overtimes",
                new { controller = "MyOT", action = "Index" }
            );

            context.MapRoute(
                "EmployeeModule_default",
                "EmployeeModule/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}