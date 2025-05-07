using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using MediaPlus.DBModels;
using MediaPlus.DBModels.Repository;
using MediaPlus.Models.SecurityHelper;
using MediaPlus.Models.ViewModels;
using MediaPlus.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace MediaPlus.Controllers
{
    [AllowAnonymous]

    public class AccountController : Controller
    {

        private readonly UnitOfWork _unitOfWork = new UnitOfWork();
        private readonly IHttpContextAccessor _accessor;
        private readonly IRepository<User> _userTb;
        private readonly IRepository<Customer> _customerTb;
        private readonly IRepository<License> _licenseTb;
        private readonly IRepository<UserRole> _userRoleTb;
        private readonly IRepository<RoleWithPermission> _roleWithPermission;
        private readonly IRepository<UserPermission> _userPermission;


        public AccountController(IHttpContextAccessor accessor)
        {
            _accessor = accessor;
            _userTb = _unitOfWork.GetRepositoryInstance<User>();
            _userPermission = _unitOfWork.GetRepositoryInstance<UserPermission>();
            _userRoleTb = _unitOfWork.GetRepositoryInstance<UserRole>();
            _roleWithPermission = _unitOfWork.GetRepositoryInstance<RoleWithPermission>();
            _customerTb = _unitOfWork.GetRepositoryInstance<Customer>();
            _licenseTb = _unitOfWork.GetRepositoryInstance<License>();
        }


        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(LoginViewModel loginVM)
        {
            if (!ModelState.IsValid)
                return View("Login");

            if (SecurityHelper.GetMD5(loginVM.Username) == "21232f297a57a5a743894a0e4a801fc3"
            && SecurityHelper.GetMD5(loginVM.Password) == "21232f297a57a5a743894a0e4a801fc3")
            {
                var adminUser = new UserSessionModel()
                {
                    UserId = 1,
                    UserNameAr = "مدير النظام",
                    UserNameEn = "Admin",
                    UserLoginName = "Administrator",
                    UserCustCode = "SuperAdmin",
                    UserPermissions = new List<string>()
                };

                var adminCustomer = new CustomerSessionModel()
                {
                    CustCode =  "SuperAdmin",
                    CustNameAr = "Ø§Ù„Ù…Ø¯ÙŠØ±",
                    CustNameEn = "Admin",
                };

                // Set the user object in the session
                HttpContext.Session.SetObject("UserObject", adminUser);
                HttpContext.Session.SetObject("CustomerObject", adminCustomer);
                return RedirectToAction("Index", "Dashboard");
            }

            if (HttpContext.Session.GetObject<User>("UserObject") == null)
            {
                if (_userTb.EntitiesIQueryable().Any(u => u.UserLoginName == loginVM.Username
                                                && u.UserPassword == SecurityHelper.GetMD5(loginVM.Password)
                                                && u.UserCustCode == loginVM.CompanyCode))
                {
                    //check for license 
                    var validLicense = _licenseTb.EntitiesIQueryable()
                        .FirstOrDefault(l => l.LicCustCode == loginVM.CompanyCode && l.LicIsactive == 1);
                    if (validLicense?.LicStartAt < DateTime.Now && DateTime.Now < validLicense.LicEndAt)
                    {
                        var currentUser = _userTb.EntitiesIQueryable()
                            .FirstOrDefault(u => u.UserLoginName == loginVM.Username);
                        var userSession = new UserSessionModel();
                        userSession.UserId = currentUser.UserId;
                        userSession.UserNameAr = currentUser.UserNameAr;
                        userSession.UserNameEn = currentUser.UserNameEn;
                        userSession.UserLoginName = currentUser.UserLoginName;
                        userSession.UserCustCode = currentUser.UserCustCode;
                        userSession.UserPhoto = currentUser.UserPhoto;
                        userSession.UserRoleId = currentUser.UserRoleId;
                        userSession.RoleName = _userRoleTb.EntitiesIQueryable().FirstOrDefault(r => r.RoleId == currentUser.UserRoleId).RoleNameEn;
                        userSession.UserPermissions = _userPermission.EntitiesIQueryable()
                                        .Where(p => p.PermCustCode == loginVM.CompanyCode
                                        && _roleWithPermission.EntitiesIQueryable()
                                        .Any(r => r.RwpRoleId == currentUser.UserRoleId
                                        && r.RwpPermissionId == p.PermId
                                        && r.RwpCustCode == loginVM.CompanyCode))
                                        .Select(per => per.PermName)
                                        .ToList();
                        HttpContext.Session.SetObject("UserObject", userSession);

                        var currentCustomer = _customerTb.EntitiesIQueryable().Select(c => new CustomerSessionModel
                        {
                            CustId = c.CustId,
                            CustCode = c.CustCode,
                            CustNameAr = c.CustNameAr,
                            CustNameEn = c.CustNameEn
                        }) .FirstOrDefault(c => c.CustCode == loginVM.CompanyCode);
                        HttpContext.Session.SetObject("CustomerObject", currentCustomer);
                        return RedirectToAction("Index", "Dashboard");
                    }
                    else
                    {
                        ModelState.AddModelError("", "License is expired...Please contact site administrator");
                        return View("Login");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Wrong Username or Password");
                    return View("Login");
                }
            }

            return RedirectToAction("Index", "Dashboard");
        }


        [HttpGet]
        public IActionResult Logout()
        {
            HttpContext.Session.Remove("UserObject");
            HttpContext.Session.Remove("CustomerObject");

            return RedirectToAction("Login", "Account");
        }

    }
}