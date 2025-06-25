using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
 using MediaPlus.DBModels;
using MediaPlus.Models.CustomAttributes.ValidateDuplications;

namespace MediaPlus.Models.ViewModels
{
    public class RoleViewModel
    { 
        public int? RoleId { get; set; }

        [RegularExpression(@"^[ء-ي\s]+$", ErrorMessage = "Only Arabic characters are allowed.")]
        [DuplicationCheckRoleNameAr]
        [Required(ErrorMessage = "الاسم العربي مطلوب")]
        public string? RoleNameAr { get; set; }

        [RegularExpression(@"^[A-Za-z\s]+$", ErrorMessage = "Only English characters are allowed.")]
        [DuplicationCheckRoleNameEn]
        [Required(ErrorMessage = "الاسم الانجليزي مطلوب")]
        public string? RoleNameEn { get; set; }

        public DateTime? RoleCdate { get; set; }

        public DateTime? RoleUdate { get; set; }

        public int RoleByuserId { get; set; } // default value 3 for super admin
        public string? UserName { get; set; } 
        public string? RoleCustCode { get; set; } // dropdown list for customer
        public string? CustomerName { get; set; } 

        public bool RoleIsactive { get; set; }
        

    }
}