using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text.Json;
using System.Threading.Tasks;
 using MediaPlus.DBModels;
using MediaPlus.DBModels.Repository;
using MediaPlus.Models.Constants;
using MediaPlus.Models.CustomFilters;
using MediaPlus.Models.ViewModels;
using MediaPlus.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;

namespace MediaPlus.Controllers
{[AuthorizeCustFilter]
    public class MaterialTypeController : Controller
    {
       
        private readonly UnitOfWork _unitOfWork = new();
        private readonly IRepository<User> _userTb;
        private readonly IRepository<Customer> _customerTb;
        private readonly IRepository<MaterialType> _materialTypeTb;
        private readonly IHttpContextAccessor _accessor;
        private readonly CustomerSessionModel? _currentCustomer;

        private readonly ILogger<MaterialTypeController> _logger;
        private readonly IWebHostEnvironment env;
        private readonly Boolean isEnglishCulture ;

#pragma warning disable CS8602 // Dereference of a possibly null reference.
        public MaterialTypeController(ILogger<MaterialTypeController> logger,
                                  IWebHostEnvironment _env,
                                  IHttpContextAccessor accessor)
        {
            _customerTb = _unitOfWork.GetRepositoryInstance<Customer>();
            _userTb = _unitOfWork.GetRepositoryInstance<User>();
            _materialTypeTb = _unitOfWork.GetRepositoryInstance<MaterialType>();
            _logger = logger;
            _accessor = accessor;
            _currentCustomer = _accessor.HttpContext.Session.GetObject<CustomerSessionModel>("CustomerObject");
            isEnglishCulture = _accessor.HttpContext.Request.Cookies[".AspNetCore.Culture"] == "c=en-US|uic=en-US";
            
            env = _env;
        }

        [HttpGet]
        public IActionResult Index()
        {

            var mTypes =_materialTypeTb
                        .EntitiesIQueryable()
                        .Where(g=>g.MtypeCustCode == _currentCustomer.CustCode)
                        .Select(x =>
                        new MaterialTypeViewModel
                        {
                        MtypeId = x.MtypeId,
                        MtypeNameAr = isEnglishCulture? "null" : x.MtypeNameAr,
                        MtypeNameEn = isEnglishCulture? x.MtypeNameEn : "null",
                        MtypeCdate = x.MtypeCdate,
                        MtypeUdate = x.MtypeUdate,
                        MtypeIsActive = x.MtypeIsactive == 1,
                        UserName = isEnglishCulture? _userTb.EntitiesIQueryable()
                                                            .FirstOrDefault(u => u.UserId == x.MtypeUserId)
                                                            .UserNameEn 
                                                    : _userTb.EntitiesIQueryable()
                                                             .FirstOrDefault(u => u.UserId == x.MtypeUserId)
                                                             .UserNameAr,
                        CustomerName = isEnglishCulture? _customerTb.EntitiesIQueryable()
                                                                    .FirstOrDefault(c => c.CustCode == x.MtypeCustCode)
                                                                    .CustNameEn 
                                                        :_customerTb.EntitiesIQueryable()
                                                                    .FirstOrDefault(c => c.CustCode == x.MtypeCustCode)
                                                                    .CustNameAr,
                        });

            
            return View(mTypes);
        }


       //=====================================================================
        // MaterialType Creation Part 
        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.isEnglishCulture = isEnglishCulture;
            return View();
        }


        [HttpPost]
        public IActionResult Create(MaterialTypeViewModel mTypeVM)
        {
            if (!ModelState.IsValid) {
                ViewBag.isEnglishCulture = isEnglishCulture;
                return View("Create"); 
            }

            _materialTypeTb.Add(
                    new MaterialType()
                    {
                        MtypeIsactive = 1,
                        MtypeCdate = DateTime.Now,
                        MtypeUdate = DateTime.Now,
                        MtypeNameEn = mTypeVM.MtypeNameEn.ToLower(),
                        MtypeNameAr = mTypeVM.MtypeNameAr,
                        MtypeUserId = _accessor.HttpContext.Session.GetObject<User>("UserObject").UserId, // Get from session
                        MtypeCustCode = _accessor.HttpContext.Session.GetObject<Customer>("CustomerObject").CustCode, // Get from session
                    }
            );

            return RedirectToAction("Index", "MaterialType");

        }


        //===================================================================== 
        // MaterialType Edit Part
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var materialType = _materialTypeTb.GetEntity(id);
            
            if (materialType == null)
            {
                return NotFound();
            }
            
            var materialTypeVM = new MaterialTypeViewModel{
                        MtypeId = materialType.MtypeId,
                        MtypeNameAr = materialType.MtypeNameAr,
                        MtypeNameEn = materialType.MtypeNameEn.ToLower() ,
                        MtypeCdate = materialType.MtypeCdate,
                        MtypeUdate = materialType.MtypeUdate,
                        MtypeUserId = materialType.MtypeUserId,
                        MtypeCustCode = materialType.MtypeCustCode,
                        MtypeIsActive = materialType.MtypeIsactive == 1
            };

            return View(materialTypeVM);
        }

        [HttpPost]
        public IActionResult Edit(MaterialTypeViewModel MtypeVM){

            if (!ModelState.IsValid) {
                return View("Edit");
            }

            var mType = new MaterialType()
                            {
                                MtypeId = MtypeVM.MtypeId.Value,  
                                MtypeNameAr = MtypeVM.MtypeNameAr,
                                MtypeNameEn = MtypeVM.MtypeNameEn,
                                MtypeCdate = MtypeVM.MtypeCdate,
                                MtypeUdate = DateTime.Now,
                                MtypeUserId = MtypeVM.MtypeUserId,
                                MtypeCustCode = MtypeVM.MtypeCustCode,  
                                MtypeIsactive =    MtypeVM.MtypeIsActive ? 1 : 0                       
                            };
               
            _materialTypeTb.Update(mType);

            return RedirectToAction("Index", "MaterialType");
        }

       //======================================================================
       // MaterialType Delete Part
       [HttpGet]
       public IActionResult Delete(int id)
       {
            _materialTypeTb.Remove(id);
            
            return RedirectToAction("Index", "MaterialType");
       }
    
        // =====================================================================
       // Toggle active Part
       [HttpGet]
       public IActionResult ToggleState(int Id)
       {    
            var targetElement = _materialTypeTb.GetEntity(Id);

            targetElement.MtypeIsactive = targetElement.MtypeIsactive == 1 ? 0 : 1;

            _materialTypeTb.Update(targetElement);

            return Ok("success");
       }



    }
}

#pragma warning restore CS8602 // Dereference of a possibly null reference.
