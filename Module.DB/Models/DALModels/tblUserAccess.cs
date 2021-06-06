using System.ComponentModel.DataAnnotations;
using System;
using System.Linq;
using System.Collections.Generic;

namespace Module.DB
{

    public partial class tblUserAccessMetaData
    {
        [Display(Name = "Id")]
        public int AccessId { get; set; }

        [Display(Name = "System Administrator")]
        public bool system_admin { get; set; }
        
    }

    [MetadataType(typeof(tblUserAccessMetaData))]
    public partial class tblUserAccess
    {
    }

    
}