using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Dynamic.Core;
using MediaPlus.Models.ViewModels;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Localization;
using MediaPlus.DBModels.Repository;
 using MediaPlus.DBModels;
using MediaPlus.Services;
using Microsoft.AspNetCore.Authorization;
using MediaPlus.Models.CustomFilters;

namespace MediaPlus.Controllers;

[AuthorizeCustFilter]
public class DashboardController : Controller
{
    private readonly ILogger<DashboardController> _logger;
    private readonly IStringLocalizer<DashboardController> _localizer;
    private readonly UnitOfWork _unitOfWork = new();
    private readonly IRepository<User> _userTb;
    private readonly IRepository<Customer> _customerTb;
    private readonly bool _isEnglishCulture;
    private readonly IHttpContextAccessor _httpContext;

    public DashboardController(ILogger<DashboardController> logger
                              ,IStringLocalizer<DashboardController> localizer
                              ,IHttpContextAccessor httpContext)
    {
        _logger = logger;
        _localizer = localizer;
        _userTb = _unitOfWork.GetRepositoryInstance<User>();
        _customerTb = _unitOfWork.GetRepositoryInstance<Customer>();
        _httpContext = httpContext;
        _isEnglishCulture = httpContext.HttpContext.Request.Cookies[".AspNetCore.Culture"] == "c=en-US|uic=en-US";
    }


    public IActionResult Index()
    {
        ViewBag.IsEnglish = _isEnglishCulture;
       
        return View();
    }

    [HttpGet]
    public IActionResult SetLanguage(string returnUrl)
    {
        //Switch culture to another one
        var currentCulture = _httpContext.HttpContext?.Request.Cookies[".AspNetCore.Culture"]?.Split('=')[1].Split('|')[0];

        var culture = currentCulture == "en-US" ? "ar-SA" : "en-US";

        Response.Cookies.Append(
            CookieRequestCultureProvider.DefaultCookieName,
            CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
            new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
        );
        
        return LocalRedirect(returnUrl);
    }

    [HttpGet]
    public IActionResult Test(){
        return View();
    }

 

}
