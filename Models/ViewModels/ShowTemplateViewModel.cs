using System.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediaPlus.Models.CustomAttributes.ValidateDuplications;
 using MediaPlus.DBModels;

namespace MediaPlus.Models.ViewModels
{
    public class ShowTemplateViewModel
    {
        public int TempId { get; set; }
        
        [RegularExpression(@"^[ء-ي\s]+$", ErrorMessage = "Only Arabic characters are allowed.")]
        [Required(ErrorMessage = "Template Arabic Name is required")]
        [DuplicationCheckTempNameAr]
        public string? TempNameAr { get; set; }

        [RegularExpression(@"^[A-Za-z\s]+$", ErrorMessage = "Only English characters are allowed.")]
        [Required(ErrorMessage = "Template English Name is required")]
        [DuplicationCheckTempNameEng]
        public string? TempNameEng { get; set; }

        [RangeAttribute( 1, 10 , ErrorMessage = "The number of rows must be between 1 and 10")]
        public int? TempRowNo { get; set; }
        
        [RangeAttribute( 1, 10 , ErrorMessage = "The number of rows must be between 1 and 10")]
        public int? TempColNo { get; set; }


        public DateTime? TempCdate { get; set; }

        public DateTime? TempUdate { get; set; }

        public int? TempByUserid { get; set; }
        public string? TempByUserName { get; set; }

        public string? TempCustCode { get; set; }
        public string? TempCustomerName {get; set;}
        
        public bool TempIsactive { get; set; }

        public List<TemplateDetail>? TempDetails { get; set; }
    }
}