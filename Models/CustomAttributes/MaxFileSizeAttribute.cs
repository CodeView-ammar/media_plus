using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MediaPlus.Models.CustomAttributes
{
    public class MaxFileSizeAttribute : ValidationAttribute
    {
        private readonly int _maxFileSize;
        public MaxFileSizeAttribute(int maxFileSize)
        {
            _maxFileSize = maxFileSize;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            
            var file = value as IFormFile;
            var _httpContext = (IHttpContextAccessor)validationContext.GetService(typeof(IHttpContextAccessor));
            var _route =_httpContext.HttpContext.Request.Path.Value;
            var currentCulture = _httpContext.HttpContext.Request.Cookies[".AspNetCore.Culture"]?.Split('=')[1].Split('|')[0];
            
            if(_route.Contains("Edit")){
                return ValidationResult.Success; 
            }

            if (file != null)
            {
            if (file.Length > _maxFileSize)
                {
                    return new ValidationResult(GetErrorMessage(currentCulture));
                }
            }

            return ValidationResult.Success;
        }

        public string GetErrorMessage(string currentCulture)
        {
            var error = currentCulture == "en-US" ? $"Maximum allowed file size is { _maxFileSize} MB" : $"الحد الاقصى لحجم الملف هو { _maxFileSize} ميجا بايت";
            return error;
        }
            
    }
}