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
    public class DuplicationCheckShowTypeNameAr : ValidationAttribute
    {
        private readonly UnitOfWork _unitOfWork = new();
        private readonly IRepository<ShowType> _showTypeTb;

        public DuplicationCheckShowTypeNameAr()
        {
           _showTypeTb = _unitOfWork.GetRepositoryInstance<ShowType>();
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {

            var _accessor = ((IHttpContextAccessor)validationContext.GetService(typeof(IHttpContextAccessor))).HttpContext;
            var _route = _accessor.Request.Path.Value;
            var currentCustCode = _accessor.Session.GetObject<Customer>("CustomerObject").CustCode;
            var id = _route.Split("/").Last();
            var currentCulture = _accessor.Request.Cookies[".AspNetCore.Culture"]?.Split('=')[1].Split('|')[0];


            var input = value as string;

             if(_route.Contains("Edit") 
            && _showTypeTb.EntitiesIQueryable()
                .Any(x=> x.ShowTypeNameAr == input && x.ShowTypeId != Convert.ToInt32(id) && x.ShowTypeCustCode == currentCustCode))
            {
                return new ValidationResult(GetErrorMessage());
            }
            if(_route.Contains("Create")
            &&_showTypeTb.EntitiesIQueryable()
                .Any(x=> x.ShowTypeNameAr == input && x.ShowTypeCustCode == currentCustCode))
            {
                return new ValidationResult(GetErrorMessage());
            }

            return ValidationResult.Success;
        }


        public string GetErrorMessage()
        {
            return "Arabic Show Type Name already exists.";
        }
    }
}