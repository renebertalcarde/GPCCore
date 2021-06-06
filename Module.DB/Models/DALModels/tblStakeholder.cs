using System.ComponentModel.DataAnnotations;
using System;
using System.Linq;
using System.Data;
using Module.DB.Enums;
using Module.DB.Procs;

namespace Module.DB
{
    public partial class tblStakeholdersMetaData
    {
        [Display(Name = "Id")]
        public int StakeholderId { get; set; }

        [Display(Name = "Stakeholder No.")]
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

        [Display(Name = "Last Password Change")]
        public DateTime? LastPasswordChange { get; set; }

        [Display(Name = "Last Password Reset")]
        public DateTime? LastPasswordReset { get; set; }

        [Display(Name = "Last Password Reset By")]
        public DateTime? LastPasswordResetBy_UserId { get; set; }

        [Display(Name = "Civil Status")]
        public int CivilStatus { get; set; }

        [Required]
        [Display(Name = "Barangay")]
        [Range(1, 999999, ErrorMessage = "The Barangay field (Home) is required")]
        public int Home_BrgyId { get; set; }

        [Display(Name = "Country")]
        public int Home_CountryId { get; set; }

        [Display(Name = "Postal Code")]
        public int Home_PostalCode { get; set; }

        [Display(Name = "Telephone No.")]
        public string Home_TelephoneNo { get; set; }

        [Display(Name = "Mobile No.")]
        public string MobileNo { get; set; }
    }

    [MetadataType(typeof(tblStakeholdersMetaData))]
    public partial class tblStakeholder
    {
        public bool IsActive()
        {
            return Account.GetUser(email: Email).Disabled != true;
        }


        [Display(Name = "Full Name")]
        public string FullName
        {
            get
            {
                return coreLib.Procs.getFullName(LastName, FirstName, MiddleName, NameExt).ToUpper();
            }
        }

        [Display(Name = "Full Name")]
        public string FullName_FN
        {
            get
            {
                return coreLib.Procs.getFullName_FN(LastName, FirstName, MiddleName, NameExt).ToUpper();
            }
        }

        [Display(Name = "Last Password Reset By")]
        public string LastPasswordResetBy
        {
            get
            {
                return Module.DB.Procs.Common.getUser(LastPasswordResetBy_UserId);
            }
        }

        public Nullable<int> Age
        {
            get
            {
                if (DOB == null)
                {
                    return null;
                }
                else
                {
                    DateTime now = DateTime.Now;
                    int age = now.Year - DOB.Value.Year;
                    if (now < DOB.Value.AddYears(age))
                    {
                        age--;
                    }

                    return age;
                }
            }
        }

        [Display(Name = "Gender")]
        public string Gender_Desc
        {
            get
            {
                return Gender == null ? "" : System.Enum.GetName(typeof(Gender), Gender);
            }
        }

        [Display(Name = "Civil Status")]
        public string CivilStatus_Desc
        {
            get
            {
                return CivilStatus == null ? "" : System.Enum.GetName(typeof(CivilStatus), CivilStatus);
            }
        }

        [Display(Name = "Home")]
        public string Home
        {
            get
            {
                return Module.DB.Procs.Common.getStatAddress(Home_Address, Home_BrgyId, Home_PostalCode);
            }
        }

        public vwCity vwCity
        {
            get
            {
                vwCity ret = null;

                _brgy brgy = Module.DB.Procs.Common.Global_CachedTables.ADDR.Brgies.Where(x => x.Id == Home_BrgyId).SingleOrDefault();
                if (brgy != null)
                {
                    ret = Module.DB.Procs.Common.Global_CachedTables.ADDR.vwCities.Where(x => x.Id == brgy.CityId).SingleOrDefault();
                }

                return ret;
            }
        }

        public AddressModel HomeAddressModel
        {
            get
            {
                return new AddressModel
                {
                    Address = Home_Address,
                    BrgyId = Home_BrgyId ?? -1,
                    PostalCode = Home_PostalCode,
                    Prefix = "Home_"
                };
            }
        }
    }
}