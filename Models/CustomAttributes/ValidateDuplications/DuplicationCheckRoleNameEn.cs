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
    public class DuplicationCheckRoleNameEn : ValidationAttribute
    {
        private readonly UnitOfWork _unitOfWork = new();
        private readonly IRepository<UserRole> _userRoleTb;

        public DuplicationCheckRoleNameEn()
        {
           _userRoleTb = _unitOfWork.GetRepositoryInstance<UserRole>();
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
            && _userRoleTb.EntitiesIQueryable()
                .Any(x=> x.RoleNameEn == input && x.RoleId != Convert.ToInt32(id) && x.RoleCustCode == currentCustCode))
            {
                return new ValidationResult(GetErrorMessage(currentCulture));
            }
            
             if(_route.Contains("Create")
            &&_userRoleTb.EntitiesIQueryable().Any(x=> x.RoleNameEn == input && x.RoleCustCode == currentCustCode))
            {
                return new ValidationResult(GetErrorMessage(currentCulture));
            }

            return ValidationResult.Success;
        }

        public string GetErrorMessage(string currentCulture)
        {
            var error = currentCulture == "en-US" ? "English Role Name already exists." : "هذا الأسم بالأنجليزية موجود بالفعل الرجاء ادخال اسم اخر.";
            return error;
        }
    }
}