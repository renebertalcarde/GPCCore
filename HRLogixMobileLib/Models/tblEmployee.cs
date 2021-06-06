using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HRLogixMobileLib.Models
{
    public class tblEmployee
    {
        [Display(Name = "Id")]
        public int EmployeeId { get; set; }

        [Display(Name = "Employee No.")]
        public string IdNo { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Display(Name = "Middle Name")]
        public string MiddleName { get; set; }

        [Display(Name = "Name Ext.")]
        public string NameExt { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Display(Name = "Geo-Authentication Areas")]
        public string GeoAuth_Areas { get; set; }

        [Display(Name = "Geo-Authentication Device Id")]
        public string GeoAuth_DeviceRef { get; set; }


        public bool IsActive { get; set; }
        [Display(Name = "Full Name")]
        public string FullName { get; set; }

        [Display(Name = "Full Name")]
        public string FullName_FN { get; set; }
    }
}
