using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
 using MediaPlus.DBModels;
using MediaPlus.DBModels.Repository;
using MediaPlus.Services;

namespace MediaPlus.Models.CustomAttributes.ValidateDuplications
{
    public class DuplicationCheckTempNameEng: ValidationAttribute
    {
        private readonly UnitOfWork _unitOfWork = new();
        private readonly IRepository<ShowTemplate> _showTemplateTb;

        public DuplicationCheckTempNameEng()
        {
           _showTemplateTb = _unitOfWork.GetRepositoryInstance<ShowTemplate>();
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {

            var _accessor = ((IHttpContextAccessor)validationContext.GetService(typeof(IHttpContextAccessor))).HttpContext;
            var currentCustCode = _accessor.Session.GetObject<Customer>("CustomerObject").CustCode;
            var _route = _accessor.Request.Path.Value;
            var id = _route.Split("/").Last();
            var currentCulture = _accessor.Request.Cookies[".AspNetCore.Culture"]?.Split('=')[1].Split('|')[0];


            var input = value as string;

            if(_route.Contains("Edit") 
            && _showTemplateTb.EntitiesIQueryable()
                .Any(x=> x.TempNameEng == input && x.TempId != Convert.ToInt32(id) && x.TempCustCode == currentCustCode))
            {
                return new ValidationResult(GetErrorMessage(currentCulture));
            }
            if(_route.Contains("Create")
            &&_showTemplateTb.EntitiesIQueryable().Any(x=> x.TempNameEng == input && x.TempCustCode == currentCustCode)){
                return new ValidationResult(GetErrorMessage(currentCulture));
            }

            return ValidationResult.Success;
        }

        public string GetErrorMessage(string currentCulture)
        {
            var error = currentCulture == "en-US" ? "English Template Name already exists." : "هذا الأسم بالأنجليزية موجود بالفعل الرجاء ادخال اسم اخر.";
            return error;
        }
    }
}