using System.ComponentModel.DataAnnotations;
using HRLogixMobileLib.Enums;

namespace HRLogixMobileLib.Models
{

    public class tblEmployee_Info
    {
        public int? Gender { get; set; }


        [Display(Name = "Gender")]
        public string Gender_Desc
        {
            get
            {
                return System.Enum.GetName(typeof(Gender), Gender ?? 0);
            }
        }

        public bool IsMale
        {
            get => Gender == (int)HRLogixMobileLib.Enums.Gender.Male;
        }
    }
}