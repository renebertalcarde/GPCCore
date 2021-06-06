using System.ComponentModel.DataAnnotations;
using System;
using System.Linq;
using System.Collections.Generic;

namespace HRLogixMobileLib.Models
{
    public class tblEmployee_Career
    {
        public int CareerId { get; set; }
        public string Position { get; set; }

        [Display(Name = "Department")]
        public string DepartmentPath { get; set; }

        [Display(Name = "Branch Address")]
        public string BranchAddress { get; set; }
    }
}