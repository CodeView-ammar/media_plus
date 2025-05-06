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
    public class DuplicationCheckMtypeNameAr: ValidationAttribute
    {
        private readonly UnitOfWork _unitOfWork = new();
        private readonly IRepository<MaterialType> _materialTypeTb;

        public DuplicationCheckMtypeNameAr()
        {
           _materialTypeTb = _unitOfWork.GetRepositoryInstance<MaterialType>();
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {

            var _accessor = ((IHttpContextAccessor)validationContext.GetService(typeof(IHttpContextAccessor))).HttpContext;
            var currentCustCode = _accessor.Session.GetObject<Customer>("CustomerObject").CustCode;
            var _route = _accessor.Request.Path.Value;
            var id = _route.Split("/").Last();

            var input = value as string;

            if(_route.Contains("Edit") 
            && _materialTypeTb.EntitiesIQueryable()
                .Any(x=> x.MtypeNameAr == input && x.MtypeId != Convert.ToInt32(id) && x.MtypeCustCode == currentCustCode))
            {
                return new ValidationResult(GetErrorMessage());
            }
            if(_route.Contains("Create")
            &&_materialTypeTb.EntitiesIQueryable().Any(x=> x.MtypeNameAr == input && x.MtypeCustCode == currentCustCode)){
                return new ValidationResult(GetErrorMessage());
            }

            return ValidationResult.Success;
        }

        public string GetErrorMessage()
        {
            return "Arabic Material Type Name already exists.";
        }
    }
}