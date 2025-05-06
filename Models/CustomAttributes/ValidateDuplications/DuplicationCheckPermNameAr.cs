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
    public class DuplicationCheckPermNameAr : ValidationAttribute
    {
        private readonly UnitOfWork _unitOfWork = new();
        private readonly IRepository<UserPermission> _userPermissionTb;

        public DuplicationCheckPermNameAr()
        {
           _userPermissionTb = _unitOfWork.GetRepositoryInstance<UserPermission>();
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var _accessor = ((IHttpContextAccessor)validationContext.GetService(typeof(IHttpContextAccessor))).HttpContext;
            var _route = _accessor.Request.Path.Value;
            var currentCustCode = _accessor.Session.GetObject<Customer>("CustomerObject").CustCode;
            var id = _route.Split("/").Last();

            var input = value as string;

            if(_route.Contains("Edit") 
            && _userPermissionTb.EntitiesIQueryable()
                .Any(x=> x.PermName == input && x.PermId != Convert.ToInt32(id) && x.PermCustCode == currentCustCode))
            {
                return new ValidationResult(GetErrorMessage());
            }
            if(_route.Contains("Create")
            &&_userPermissionTb.EntitiesIQueryable().Any(x=> x.PermName == input && x.PermCustCode == currentCustCode)){
                return new ValidationResult(GetErrorMessage());
            }

            return ValidationResult.Success;
        }

        public string GetErrorMessage()
        {
            return "Arabic Permission Name already exists.";
        }
    }
}