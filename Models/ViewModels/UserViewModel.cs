
using System.ComponentModel.DataAnnotations;
using MediaPlus.Models.CustomAttributes;
using MediaPlus.Models.CustomAttributes.ValidateDuplications;


namespace MediaPlus.Models.ViewModels
{
    public class UserViewModel
    { 
        public int UserId { get; set; }

        [RegularExpression(@"^[ء-ي\s]+$", ErrorMessage = "Only Arabic characters are allowed.")]
        [DuplicationCheckUserNameAr]
        public required string UserNameAr { get; set; }

        [RegularExpression(@"^[A-Za-z\s]+$", ErrorMessage = "Only English characters are allowed.")]
        [DuplicationCheckUserNameEn]
        public required string UserNameEn { get; set; }

        [Required(ErrorMessage="UserLoginName is required")]
        [DuplicationCheckUserLogin]
        public required string UserLoginName { get; set; }

        [Required(ErrorMessage="Password is required")]
        [DataType(DataType.Password)]
        [StringLength(50, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 8)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,}$"
        , ErrorMessage = "Password must contain at least one uppercase letter, one lowercase letter, one digit, and one special character.")]
        public string? UserPassword { get; set; }

        [MaxFileSize(20 * 1024 * 1024)]
        [AllowedExtensions(new string[] { ".jpg", ".png",".jpeg"})]
        public IFormFile? UserPhoto { get; set; }
        
        public string? UserPhotoPath { get; set; }

        public DateTime? UserCdate { get; set; }

        public DateTime? UserUdate { get; set; }

        public int UserRoleId { get; set; }
        public string? RoleName { get; set; }
 
        public string? UserCustCode { get; set; }
        public string? CustomerName {get; set;}

		[Compare("UserPassword", ErrorMessage = "كلمة المرور غير متطابقة")]
		public string ConfirmPassword { get; set; }


	}
}