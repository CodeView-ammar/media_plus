using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MediaPlus.Models.CustomAttributes
{
    public class AllowedExtensionsAttribute : ValidationAttribute
    {
        private readonly string[] _extensions;
        public AllowedExtensionsAttribute(string[] extensions)
        {
            _extensions = extensions;
        }
        
        protected override ValidationResult IsValid(
        object value, ValidationContext validationContext)
        {
            var file = value as IFormFile;
            var _httpContext = (IHttpContextAccessor)validationContext.GetService(typeof(IHttpContextAccessor));
            var _route = _httpContext?.HttpContext?.Request.Path.Value;
            var currentCulture = _httpContext?.HttpContext?.Request.Cookies[".AspNetCore.Culture"]?.Split('=')[1].Split('|')[0];

            if(_route.Contains("Edit")){
                return ValidationResult.Success; 
            }


            if (file != null)
            {
                var extension = Path.GetExtension(file.FileName);
                if (!_extensions.Contains(extension.ToLower()))
                {
                    return new ValidationResult(GetErrorMessage(currentCulture));
                }
            }
            return ValidationResult.Success;
        }

        public string GetErrorMessage(string currentCulture)
        {
            var error = currentCulture == "en-US" ? "This file extension is not allowed!" : "لا يمكنك رفع هذا النوع من الملفات";
            return error;
        }
    }
}