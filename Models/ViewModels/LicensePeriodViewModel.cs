using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MediaPlus.Models.ViewModels
{
    public class LicensePeriodViewModel
    {
        [Required(ErrorMessage="Please Select Customer!")]
        public string LicCustCode { get; set; }

        [Required(ErrorMessage="Please input the start license date!")]
        public DateTime LicStartAt { get; set; } 

        [Required(ErrorMessage="Please input the end license date!")]
        public DateTime LicEndAt { get; set; }
    }
}