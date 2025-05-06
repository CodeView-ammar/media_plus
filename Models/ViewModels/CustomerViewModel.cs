using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using MediaPlus.Models.CustomAttributes;
using MediaPlus.Models.CustomAttributes.ValidateDuplications;
using Microsoft.AspNetCore.Http;

namespace MediaPlus.Models.ViewModels
{
    public class CustomerViewModel
    {
        [RegularExpression(@"^[ء-ي\s]+$", ErrorMessage = "Only Arabic characters are allowed.")]
        [DuplicationCheckCustomerAr]
        public required string CustNameAr { get; set; }

        [RegularExpression(@"^[A-Za-z\s]+$", ErrorMessage = "Only English characters are allowed.")]
        [DuplicationCheckCustomerEn]
        public required string CustNameEn { get; set; }
        
        public string? UserName { get; set; }

        public string? CustVatNo { get; set; }
        public string? CustTrNo { get; set; }

        public string? CustTel { get; set; }
        public string? CustMobileNo { get; set; }    

        [EmailAddress]
        public string? CustEmail { get; set; }

        [MaxFileSize(20 * 1024 * 1024)]
        [AllowedExtensions(new string[] { ".jpg", ".png",".jpeg"})]
        public IFormFile? CustLogo { get; set; }
        public string? CustLogoPath { get; set; }

        public DateTime? CustCdate { get; set; }
        public DateTime? CustUdate { get; set; }

        public int CustId { get;  set; }
        public string? CustCode { get;  set; } 
        public bool CustState { get; set; }
    }
}