using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
 using MediaPlus.DBModels;
using MediaPlus.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.VisualBasic;

namespace MediaPlus.Models.CustomFilters
{
    public class AuthorizeCustFilter : Attribute, IAuthorizationFilter
    {

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            if(context.HttpContext.Session.GetObject<Customer>("CustomerObject") == null 
                && context.HttpContext.Session.GetObject<User>("UserObject") == null)
            {
                context.Result = new RedirectResult("app/Account/Login");
            }
        }
        
    }
}