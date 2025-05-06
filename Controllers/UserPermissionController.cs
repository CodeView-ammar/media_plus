using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography.Xml;
using System.Text.Json;
using System.Threading.Tasks;
 using MediaPlus.DBModels;
using MediaPlus.DBModels.Repository;
using MediaPlus.Models.CustomFilters;
using MediaPlus.Models.ViewModels;
using MediaPlus.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;

#pragma warning disable CS8602 // Dereference of a possibly null reference.

namespace MediaPlus.Controllers
{
    [AuthorizeCustFilter]
    public class UserPermissionController : Controller
    {
       
        private readonly UnitOfWork _unitOfWork = new();
        private readonly IRepository<User> _userTb;
        private readonly IRepository<Customer> _customerTb;
        private readonly IRepository<UserPermission> _userPermissionTb;
        private readonly IRepository<UserRole> _roles;
        private readonly IRepository<RoleWithPermission> _rolesWithPermission;
        private readonly IHttpContextAccessor _accessor;
        private readonly CustomerSessionModel? _currentCustomer;
        private readonly IWebHostEnvironment env;
        private readonly ILogger<UserPermissionController> _logger;
        private readonly Boolean isEnglishCulture ;
 
        public UserPermissionController(ILogger<UserPermissionController> logger,
                                  IWebHostEnvironment _env,
                                  IHttpContextAccessor accessor)
        {
            _customerTb = _unitOfWork.GetRepositoryInstance<Customer>();
            _userTb = _unitOfWork.GetRepositoryInstance<User>();
            _userPermissionTb = _unitOfWork.GetRepositoryInstance<UserPermission>();
            _roles = _unitOfWork.GetRepositoryInstance<UserRole>();
            _rolesWithPermission = _unitOfWork.GetRepositoryInstance<RoleWithPermission>();
            _logger = logger;
            _accessor = accessor;
            _currentCustomer = _accessor.HttpContext.Session.GetObject<CustomerSessionModel>("CustomerObject");
            isEnglishCulture = accessor.HttpContext.Request.Cookies[".AspNetCore.Culture"] == "c=en-US|uic=en-US";
            env = _env;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var UserPermissions =_userPermissionTb
                        .EntitiesIQueryable()
                        .Where(u=>u.PermCustCode == _currentCustomer.CustCode)
                        .Select(x =>
                        new UserPermissionViewModel
                        {
                            PermId = x.PermId,
                            PermCdate = x.PermCdate,
                            PermUdate = x.PermUdate,
                            PermIsactive = x.PermIsactive == 1,
                            PermNameEn = isEnglishCulture ? x.PermName : "null",
                            PermNameAr = isEnglishCulture ? "null" : x.PermName,
                            PermByUserName =isEnglishCulture? _userTb.EntitiesIQueryable().FirstOrDefault(u => u.UserId == x.PermByUserid).UserNameEn
                                                            : _userTb.EntitiesIQueryable().FirstOrDefault(u => u.UserId == x.PermByUserid).UserNameAr, 
                            PremCustomerName =isEnglishCulture? _customerTb.EntitiesIQueryable().FirstOrDefault(c => c.CustCode == x.PermCustCode).CustNameEn
                                                            : _customerTb.EntitiesIQueryable().FirstOrDefault(c => c.CustCode == x.PermCustCode).CustNameAr,

                        });

            
            return View(UserPermissions);
        }


       //=====================================================================
        // UserPermission Creation Part 
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }


        [HttpPost]
        public IActionResult Create(UserPermissionViewModel UserPermVM)
        {
            if (!ModelState.IsValid) 
                return View("Create"); 
            var InsertedUserPermission =  new UserPermission()
            {
                PermByUserid = _accessor.HttpContext.Session.GetObject<User>("UserObject").UserId, // Get from session
                PermCustCode = _accessor.HttpContext.Session.GetObject<Customer>("CustomerObject").CustCode, // Get from session
                PermCdate = DateTime.Now,
                PermUdate = DateTime.Now,
                PermIsactive = 1,
                PermName = UserPermVM.PermNameEn,
            };

            _userPermissionTb.Add(InsertedUserPermission);

            return RedirectToAction("Index", "UserPermission");

        }


        //===================================================================== 
        // UserPermission Edit Part
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var UserPermission = _userPermissionTb.GetEntity(id);
            
            if (UserPermission == null)
            {
                return NotFound();
            }
            
            var UserPermissionVM = new UserPermissionViewModel{
                    PermId = UserPermission.PermId,  
                    PermCustCode = UserPermission.PermCustCode,
                    PermByUserid = UserPermission.PermByUserid.Value,
                    PermCdate = UserPermission.PermCdate,
                    PermUdate = UserPermission.PermUdate,
                    PermIsactive = UserPermission.PermIsactive == 1,
                    PermNameEn = UserPermission.PermName.ToLower(),
                    PermNameAr = UserPermission.PermName,
            };

            return View(UserPermissionVM);
        }

        [HttpPost]
        public IActionResult Edit(UserPermissionViewModel UserPermissionVM){

            if (!ModelState.IsValid) 
                return View("Edit");
            

            var UserPermission = new UserPermission()
                            {
                                PermName = UserPermissionVM.PermNameEn,
                                PermId = UserPermissionVM.PermId,  
                                PermCustCode = UserPermissionVM.PermCustCode,
                                PermByUserid = UserPermissionVM.PermByUserid,
                                PermCdate = UserPermissionVM.PermCdate,
                                PermUdate = DateTime.Now,
                                PermIsactive = (bool)UserPermissionVM.PermIsactive ? 1 : 0,                 
                            };
               
            _userPermissionTb.Update(UserPermission);

            return RedirectToAction("Index", "UserPermission");
        }

       //======================================================================
       // UserPermission Delete Part
       [HttpGet]
       public IActionResult Delete(int id)
       {
            _userPermissionTb.Remove(id);
            
            return RedirectToAction("Index", "UserPermission");
       }
       

        // Toggle active Part
       [HttpGet]
       public IActionResult ToggleState(int Id)
       {    
            var targetElement = _userPermissionTb.GetEntity(Id);

            targetElement.PermIsactive = targetElement.PermIsactive == 1 ? 0 : 1;

            _userPermissionTb.Update(targetElement);

            return Ok("success");
       }
    }
}
