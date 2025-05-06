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
    public class DuplicationCheckUserLogin: ValidationAttribute
    {
        private readonly UnitOfWork _unitOfWork = new();
        private readonly IRepository<User> _userTb;

        public DuplicationCheckUserLogin()
        {
           _userTb = _unitOfWork.GetRepositoryInstance<User>();
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
            && _userTb.EntitiesIQueryable()
                .Any(x=> x.UserLoginName == input && x.UserId != Convert.ToInt32(id) && x.UserCustCode == currentCustCode))
            {
                return new ValidationResult(GetErrorMessage(currentCulture));
            }
            
            if(_route.Contains("Create")
            &&_userTb.EntitiesIQueryable().Any(x=> x.UserLoginName == input && x.UserCustCode == currentCustCode)){
                return new ValidationResult(GetErrorMessage(currentCulture));
            }

            return ValidationResult.Success;
        }

        public string GetErrorMessage(string currentCulture)
        {
            var error = currentCulture == "en-US" ? "Login Name already exists." : "اسم الدخول موجود بالفعل الرجاء ادخال اسم اخر.";
            return error;
        }
    }
} 