using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using MediaPlus.Models.CustomAttributes.ValidateDuplications;

namespace MediaPlus.Models.ViewModels
{
    public class UserPermissionViewModel
    {
        public int PermId { get; set; }

        [Required(ErrorMessage = "Arabic Permission Name is required")]
        [DuplicationCheckPermNameAr]
        public string? PermNameAr { get; set; }

        [Required(ErrorMessage = "English Permission Name is required")]
        [DuplicationCheckPermNameEn]
        public string? PermNameEn { get; set; }
        public DateTime? PermUdate { get; set; }

        public DateTime? PermCdate { get; set; }

        public bool PermIsactive { get; set; }

        public int PermByUserid { get; set; }
        public string? PermByUserName { get; set; }

        public string? PermCustCode { get; set; }
        public string? PremCustomerName {get; set;}
    }
}