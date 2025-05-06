using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using MediaPlus.Models.ViewModels;

namespace MediaPlus.Models.CustomAttributes
{
    public class TakeOneInputFile: ValidationAttribute
    {
        public TakeOneInputFile()
        {
        }
         
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var input = value as MatFileViewModel;
            var _accessor = ((IHttpContextAccessor)validationContext.GetService(typeof(IHttpContextAccessor))).HttpContext;
            var _route = _accessor.Request.Path.Value;
            var currentCulture = _accessor.Request.Cookies[".AspNetCore.Culture"]?.Split('=')[1].Split('|')[0];
            
            if(input == null)
            {
                return new ValidationResult(GetErrorMessage(currentCulture));
            }else if(input.attachedMaterialFile == null && input.attachedText == null)
            {
                return new ValidationResult(GetErrorMessage(currentCulture));
            }
            else if(input.attachedMaterialFile != null && input.attachedText != null)
            {
                return new ValidationResult(GetErrorMessage(currentCulture));
            }
                return ValidationResult.Success;   
        }

        public string GetErrorMessage(string currentCulture)
        {
            var error = currentCulture == "en-US" ? "Please input media file or text file one option only." : "الرجاء تحديد ملف الوسائط او ملف النص واحد فقط.";
            return error;
        }

    }
}