using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
 using MediaPlus.DBModels;
using MediaPlus.DBModels.Repository;
using MediaPlus.Models.ViewModels;
using MediaPlus.Services;

namespace MediaPlus.Models.CustomAttributes
{
    public class LicensePeriodCheck: ValidationAttribute
    {
        private readonly UnitOfWork _unitOfWork = new();
        private readonly IRepository<License> _licenseTb;

        public LicensePeriodCheck( )
        {
            _licenseTb = _unitOfWork.GetRepositoryInstance<License>();
        }
         
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var licPeriod = value as LicensePeriodViewModel;
            var _httpContext = (IHttpContextAccessor)validationContext.GetService(typeof(IHttpContextAccessor))!;
            var _route = _httpContext.HttpContext.Request.Path.Value;
            var id = _route.Split("/").Last();
            var currentCulture = _httpContext.HttpContext.Request.Cookies[".AspNetCore.Culture"]?.Split('=')[1].Split('|')[0];
            // Get cust code for add license 

            var lics = _licenseTb.EntitiesIQueryable()
                                 .Where(l => l.LicCustCode == licPeriod.LicCustCode);


            if(licPeriod.LicEndAt.Date < licPeriod.LicStartAt.Date)
            {
                return new ValidationResult(GetErrorMessage(currentCulture));
            } 
            
            if(_route.Contains("Edit") 
            && lics
            .Any(l => licPeriod.LicStartAt <= l.LicEndAt && l.LicStartAt <= licPeriod.LicEndAt
                                && l.LicId != Convert.ToInt32(id)))
            {
                return new ValidationResult(GetErrorMessage2(currentCulture));
            }

            if(_route.Contains("Create") 
            && lics.Any(l => licPeriod.LicStartAt <= l.LicEndAt && l.LicStartAt <= licPeriod.LicEndAt))
            {
                return new ValidationResult(GetErrorMessage2(currentCulture));
            }

             return ValidationResult.Success;
            }
            

        public string GetErrorMessage(string currentCulture)
        {
            var error = currentCulture == "en-US" ? "License end time must be after license start time" : "يجب ان يكون وقت الانتهاء بعد وقت البدء";
            return error;
        }
        public string GetErrorMessage2(string currentCulture)
        {
            var error = currentCulture == "en-US" ? "There is overlap between 2 licenses" : "هناك تداخل بين انشاء اوقات التراخيص";
            return error;
        }

    }
}