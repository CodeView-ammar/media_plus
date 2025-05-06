#pragma warning disable CS8603 // Possible null reference return.
#pragma warning disable CS8765 // Nullability of type of parameter doesn't match overridden member (possibly because of nullability attributes).
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
    public class DuplicationCheckMaterialEn: ValidationAttribute
    {
        private readonly UnitOfWork _unitOfWork = new();
        private readonly IRepository<ShowMaterial> _MaterialTb;
        public DuplicationCheckMaterialEn()
        {
            _MaterialTb = _unitOfWork.GetRepositoryInstance<ShowMaterial>();
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var _accessor = ((IHttpContextAccessor)validationContext.GetService(typeof(IHttpContextAccessor))).HttpContext;
            var currentCustCode = "8d4d0a16";
            var _route = _accessor.Request.Path.Value;
            var id = _route.Split("/").Last();
            var currentCulture = _accessor.Request.Cookies[".AspNetCore.Culture"]?.Split('=')[1].Split('|')[0];


            var input = value as string;

            if(_route.Contains("Edit") 
            && _MaterialTb.EntitiesIQueryable()
                .Any(x=> x.MatShowNameEn == input && x.MatId != Convert.ToInt32(id) && x.MatCustCode == currentCustCode))
            {
                return new ValidationResult(GetErrorMessage(currentCulture));
            }
             if(_route.Contains("Create")
            &&_MaterialTb.EntitiesIQueryable().Any(x=> x.MatShowNameEn == input && x.MatCustCode == currentCustCode)){
                return new ValidationResult(GetErrorMessage(currentCulture));
            }

            return ValidationResult.Success;
        }

        
        public string GetErrorMessage(string currentCulture)
        {
            var error = currentCulture == "en-US" ? "English Customer Name already exists." : "هذا الأسم بالأنجليزية موجود بالفعل الرجاء ادخال اسم اخر.";
            return error;
        }
            
    }
#pragma warning restore CS8765 // Nullability of type of parameter doesn't match overridden member (possibly because of nullability attributes).
#pragma warning restore CS8603 // Possible null reference return.
}