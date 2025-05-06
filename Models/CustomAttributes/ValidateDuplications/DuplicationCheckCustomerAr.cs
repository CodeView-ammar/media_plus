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
    public class DuplicationCheckCustomerAr : ValidationAttribute
    {
        private readonly UnitOfWork _unitOfWork = new();
        private readonly IRepository<Customer> _customerTb;

        public DuplicationCheckCustomerAr()
        {
           _customerTb = _unitOfWork.GetRepositoryInstance<Customer>();
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {

            var _accessor = ((IHttpContextAccessor)validationContext.GetService(typeof(IHttpContextAccessor))).HttpContext;
            var _route = _accessor.Request.Path.Value;
            var id = _route.Split("/").Last();
            var currentCulture = _accessor.Request.Cookies[".AspNetCore.Culture"]?.Split('=')[1].Split('|')[0];


            var input = value as string;

            if(_route.Contains("Edit") 
            && _customerTb.EntitiesIQueryable()
                .Any(x=> x.CustNameAr == input && x.CustId != Convert.ToInt32(id)))
            {
                return new ValidationResult(GetErrorMessage(currentCulture));
            }
            
             if(_route.Contains("Create")
            &&_customerTb.EntitiesIQueryable().Any(x=> x.CustNameAr == input)){
                return new ValidationResult(GetErrorMessage(currentCulture));
            }

            return ValidationResult.Success;
        }

        public string GetErrorMessage(string currentCulture)
        {
            var error = currentCulture == "en-US" ? "Arabic Customer Name already exists." : "هذا الأسم بالعربية موجود بالفعل الرجاء ادخال اسم اخر.";
            return error;
        }
    }
}