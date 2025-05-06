using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MediaPlus.Models.CustomAttributes
{
    public class CustomRequired: ValidationAttribute
    {

        public CustomRequired()
        {
        }
        
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var file = value as IFormFile;
            var _httpContext = (IHttpContextAccessor)validationContext.GetService(typeof(IHttpContextAccessor))!;
            var currentCulture = _httpContext.HttpContext.Request.Cookies[".AspNetCore.Culture"]?.Split('=')[1].Split('|')[0];
            var _accessor = _httpContext.HttpContext;
            var _route = _accessor.Request.Path.Value;

            if(_route.Contains("Create"))
            {
                if(file == null)
                {
                    return new ValidationResult(GetErrorMessage(currentCulture));
                }else
                {
                    return ValidationResult.Success;
                }
            }else if(_route.Contains("Edit")){
                if(file == null){
                    return ValidationResult.Success;
                }else
                {
                    return new ValidationResult(GetErrorMessage(currentCulture));
                }
            }
            return ValidationResult.Success;
        }

        public string GetErrorMessage(string currentCulture)
        {
            var error = currentCulture == "en-US" ? "This field is required" : "هذا الحقل مطلوب";
            return error;
        }
    }
}