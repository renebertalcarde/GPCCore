using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace coreApp.Models
{
    public class PasswordSetting
    {
        public PasswordSetting()
        {
            //
            // TODO: Add constructor logic here
            //
        }
        
        //password minimum length
        public int MinLength { get; set; }

        //password maximum length
        public int MaxLength { get; set; }

        //password Numbers length
        public int NumsLength { get; set; }

        //password Upper letter length
        public int UpperLength { get; set; }

        //password Special character length
        public int SpecialLength { get; set; }

        //password valid special characters
        public string SpecialChars { get; set; }
    }
}