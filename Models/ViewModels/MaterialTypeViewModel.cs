using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using MediaPlus.Models.CustomAttributes.ValidateDuplications;

namespace MediaPlus.Models.ViewModels
{
    public class MaterialTypeViewModel
    {
        public int? MtypeId { get; set; }

        [RegularExpression(@"^[ء-ي\s]+$", ErrorMessage = "Only Arabic characters are allowed.")]
        [DuplicationCheckMtypeNameAr]
        public string? MtypeNameAr { get; set; }

        [RegularExpression(@"^[A-Za-z\s]+$", ErrorMessage = "Only English characters are allowed.")]
        [DuplicationCheckMtypeNameEn]
        public string? MtypeNameEn { get; set; }
  
        public DateTime? MtypeCdate { get; set; }

        public DateTime? MtypeUdate { get; set; }

        public int? MtypeUserId { get; set; }
        public string? UserName {get; set;}

        public string? MtypeCustCode { get; set; }
        public string? CustomerName {get; set;}

        public bool MtypeIsActive{get; set;}
    }
}