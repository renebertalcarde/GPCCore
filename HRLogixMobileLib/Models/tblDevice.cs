using System.ComponentModel.DataAnnotations;
using System;

namespace HRLogixMobileLib.Models
{

    public class tblDevice
    {
        [Display(Name = "Id")]
        public int DeviceId { get; set; }

        [Required]
        [Range(1, 99999, ErrorMessage = "The Office field is required")]
        [Display(Name = "Office")]
        public int OfficeId { get; set; }

        [Required]
        [Display(Name = "Device Name")]
        public string DeviceName { get; set; }

        [Required]
        [Display(Name = "Type")]
        [Range(0, 999999, ErrorMessage = "The field Type is required")]
        public int Type { get; set; }

        [Display(Name = "Brand")]
        public string DeviceBrand { get; set; }

        [Required]
        [Display(Name = "Model")]
        public string DeviceModel { get; set; }

        [Required]
        [Display(Name = "IP Address")]
        public string IP { get; set; }

        public string Username { get; set; }
        public string Password { get; set; }


        [Required]
        public int? Port { get; set; }

        [Required]
        [Display(Name = "Terminal Id")]
        public int TerminalId { get; set; }

        [Display(Name = "Managed by Kiosk")]
        public bool ManagedByKiosk { get; set; }

        public bool Match(tblDevice device)
        {
            bool ret = false;

            ret = 
                TerminalId == device.TerminalId && 
                IP == device.IP &&
                Port == device.Port && 
                DeviceModel == device.DeviceModel;

            return ret;
        }

        public bool _set;
    }
}