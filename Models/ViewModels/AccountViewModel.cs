using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MediaPlus.Models.ViewModels
{
    public class LoginViewModel
    {
        [Required]
        public required string Username { get; set; }

        [Required]
        public required string Password { get; set; }

        public string? CompanyCode {get; set;}
    }
    public class RegisterViewModel
    {
        [Required]
        [MinLength(3, ErrorMessage = "Name must be at least 3 characters long")]
        [MaxLength(12, ErrorMessage = "Name must be less than 20 characters long")]
        [Display(Name = "Username")]
        public required string ShortName { get; set; }

        [EmailAddress]
        [Required]
        public required string Email { get; set; }

        [Required]
        public required string Password { get; set; }

        [Required]
        [Compare("Password")]
        public required string ConfirmPassword { get; set; }

      
    }

    public class ChangePasswordViewModel{
        [Required]
        public required string CurrentPassword{get; set;}

        [Required]
        public required string NewPassword{get; set;}

        [Compare("NewPassword")]
        public required string ConfirmPassword{get; set;}
    }

    public class ForgetPasswordViewModel{
        [Required]
        [EmailAddress]
        public required string Email{get; set;}
    }

    public class ResetPasswordViewModel{
        [Required]
   public required string Token{get; set;}

        [Required]
        [EmailAddress]
        public required string Email{get; set;}

        [Required]
        [DataType(DataType.Password)]
        public required string NewPassword{get; set;}

        [Required]
        [DataType(DataType.Password)]
        [Compare("NewPassword")]
        public required string NewPasswordConfirmation{get; set;}   
    }
}
