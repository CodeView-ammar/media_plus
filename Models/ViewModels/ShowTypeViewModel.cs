using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
 using MediaPlus.DBModels;
using MediaPlus.Models.CustomAttributes;
using MediaPlus.Models.CustomAttributes.ValidateDuplications;

namespace MediaPlus.Models.ViewModels
{
    public class ShowTypeViewModel
    {
        public int ShowTypeId { get; set; }
        
        [RegularExpression(@"^[ء-ي\s]+$", ErrorMessage = "Only Arabic characters are allowed.")]
        [Required(ErrorMessage = "Show Type Name is required")]
        [DuplicationCheckShowTypeNameAr]
        public string? ShowTypeNameAr { get; set; }

        [RegularExpression(@"^[A-Za-z\s]+$", ErrorMessage = "Only English characters are allowed.")]
        [Required(ErrorMessage = "Show Type Name is required")]
        [DuplicationCheckShowTypeNameEn]
        public string? ShowTypeNameEn { get; set; }

        public string? ShowTypeCustCode { get; set; }
        public string? ShowTypeCustomerName {get; set;}

        public int? ShowTypeByUserid { get; set; }
        public string? ShowTypeUserName {get; set;}

        public DateTime? ShowTypeCdate { get; set; }

        public DateTime? ShowTypeUdate { get; set; }

        public bool ShowTypeIsactive { get; set; }
    }
}