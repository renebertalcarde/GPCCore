using System.ComponentModel.DataAnnotations;
using System;
using System.Linq;
using System.Data;

namespace Module.DB
{
    public partial class tblAccountabilitiesMetaData
    {
        [Display(Name = "Date")]
        [DataType(dataType: DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0: MM/dd/yyyy hh:mm:ss tt}")]
        public DateTime LogDate { get; set; }
    }

    [MetadataType(typeof(tblAccountabilitiesMetaData))]
    public partial class tblAccountability
    {
        public string Stakeholder
        {
            get
            {
                return Procs.Common.getUser(UserId);
            }
        }
    }
}