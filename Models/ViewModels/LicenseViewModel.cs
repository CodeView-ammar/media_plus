using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using MediaPlus.Models.CustomAttributes;

namespace MediaPlus.Models.ViewModels
{
    public class LicenseViewModel
    {
        public int LicId { get; set; }

        [LicensePeriodCheck]
        public LicensePeriodViewModel LicPeriod {get; set;}

        public int LicDeviceNo { get; set; }
        public int LicUserNo { get; set; }
        public string? LicUserName{ get; set; }

        public string? LicCustomerName { get; set; }

        public bool LicIsactive { get; set; }
        public DateTime LicCdate { get; set; }

        public DateTime LicUdate { get; set; }
        public int LicMachineNo { get; set; }  
    }
}