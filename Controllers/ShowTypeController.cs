using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
 using MediaPlus.DBModels;
using MediaPlus.DBModels.Repository;
using MediaPlus.Models.CustomFilters;
using MediaPlus.Models.ViewModels;
using MediaPlus.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace MediaPlus.Controllers
{
[AuthorizeCustFilter]
    public class ShowTypeController : Controller
    {
       
        private readonly UnitOfWork _unitOfWork = new();
        private readonly IRepository<User> _userTb;
        private readonly IRepository<Customer> _customerTb;
        private readonly IRepository<ShowType> _ShowTypeTb;
        private readonly IHttpContextAccessor _accessor;
        private readonly CustomerSessionModel? _currentCustomer;
        private readonly ILogger<ShowTypeController> _logger;
        private readonly IWebHostEnvironment env;
        private readonly Boolean isEnglishCulture ;

        public ShowTypeController(ILogger<ShowTypeController> logger,
                                  IWebHostEnvironment _env,
                                  IHttpContextAccessor accessor)
        {
            _customerTb = _unitOfWork.GetRepositoryInstance<Customer>();
            _userTb = _unitOfWork.GetRepositoryInstance<User>();
            _ShowTypeTb = _unitOfWork.GetRepositoryInstance<ShowType>();
            _logger = logger;
            _accessor = accessor;
            _currentCustomer = _accessor.HttpContext.Session.GetObject<CustomerSessionModel>("CustomerObject");
            isEnglishCulture = _accessor.HttpContext.Request.Cookies[".AspNetCore.Culture"] == "c=en-US|uic=en-US";
            env = _env;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var ShowTypes =_ShowTypeTb.EntitiesIQueryable()
                                      .Where(g=>g.ShowTypeCustCode == _currentCustomer.CustCode)
                                      .Select(x =>new ShowTypeViewModel
                                      {
                                        ShowTypeId = x.ShowTypeId,
                                        ShowTypeNameAr =isEnglishCulture ? "null" : x.ShowTypeNameAr,
                                        ShowTypeNameEn =isEnglishCulture ?  x.ShowTypeNameEng : "null",
                                        ShowTypeIsactive = x.ShowTypeIsactive == 1 ? true : false,
                                        ShowTypeCdate = x.ShowTypeCdate,
                                        ShowTypeUdate = x.ShowTypeUdate,
                                        
                                        ShowTypeCustomerName =isEnglishCulture ? _customerTb.EntitiesIQueryable().FirstOrDefault(c => c.CustCode == x.ShowTypeCustCode).CustNameEn
                                        : _customerTb.EntitiesIQueryable().FirstOrDefault(c => c.CustCode == x.ShowTypeCustCode).CustNameAr,

                                        ShowTypeUserName =isEnglishCulture ? _userTb.EntitiesIQueryable().FirstOrDefault(u => u.UserId == x.ShowTypeByUserid).UserNameEn
                                        : _userTb.EntitiesIQueryable().FirstOrDefault(u => u.UserId == x.ShowTypeByUserid).UserNameEn     
                                    });

            
            return View(ShowTypes);
        }


       //=====================================================================
        // ShowType Creation Part 
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }


        [HttpPost]
        public IActionResult Create(ShowTypeViewModel showTypeVM)
        {

            if (!ModelState.IsValid) 
                return View("Create"); 
            
            _ShowTypeTb.Add(
                    new ShowType()
                    {
                        ShowTypeNameAr = showTypeVM.ShowTypeNameAr,
                        ShowTypeNameEng = showTypeVM.ShowTypeNameEn,
                        ShowTypeCdate = DateTime.Now,
                        ShowTypeIsactive = 1,
                        ShowTypeUdate = DateTime.Now,
                        ShowTypeByUserid = _accessor.HttpContext.Session.GetObject<User>("UserObject").UserId, // Get from session
                        ShowTypeCustCode = _accessor.HttpContext.Session.GetObject<Customer>("CustomerObject").CustCode, // Get from session
                    }
            );

            return RedirectToAction("Index", "ShowType");
        }


        //===================================================================== 
        // ShowType Edit Part
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var ShowType = _ShowTypeTb.GetEntity(id);
            
            if (ShowType == null)
            {
                return NotFound();
            }
            
            var ShowTypeVM = new ShowTypeViewModel{
                    ShowTypeId = ShowType.ShowTypeId,
                    ShowTypeNameAr = ShowType.ShowTypeNameAr,
                    ShowTypeNameEn = ShowType.ShowTypeNameEng,
                    ShowTypeCdate = ShowType.ShowTypeCdate,
                    ShowTypeIsactive = ShowType.ShowTypeIsactive == 1,
                    ShowTypeUdate = ShowType.ShowTypeUdate,
                    ShowTypeCustCode = ShowType.ShowTypeCustCode,
                    ShowTypeByUserid = ShowType.ShowTypeByUserid,
            };

            return View(ShowTypeVM);
        }

        [HttpPost]
        public IActionResult Edit(ShowTypeViewModel ShowTypeVM){

            if (!ModelState.IsValid) 
                return View("Edit");
            

            var ShowType = new ShowType()
                            {
                                ShowTypeId = ShowTypeVM.ShowTypeId,
                                ShowTypeNameAr = ShowTypeVM.ShowTypeNameAr,
                                ShowTypeNameEng = ShowTypeVM.ShowTypeNameEn,
                                ShowTypeCdate = ShowTypeVM.ShowTypeCdate,
                                ShowTypeIsactive = ShowTypeVM.ShowTypeIsactive == true ? 1 : 0,
                                ShowTypeUdate = DateTime.Now,
                                ShowTypeCustCode = ShowTypeVM.ShowTypeCustCode,
                                ShowTypeByUserid = ShowTypeVM.ShowTypeByUserid
                            };
               
            _ShowTypeTb.Update(ShowType);

            return RedirectToAction("Index", "ShowType");
        }

       //======================================================================
       // ShowType Delete Part
       [HttpGet]
       public IActionResult Delete(int id)
       {
            _ShowTypeTb.Remove(id);
            
            return RedirectToAction("Index", "ShowType");
       }

        // =====================================================================
       // Toggle active Part
       [HttpGet]
       public IActionResult ToggleState(int Id)
       {    
            var targetElement = _ShowTypeTb.GetEntity(Id);

            targetElement.ShowTypeIsactive = targetElement.ShowTypeIsactive == 1 ? 0 : 1;

            _ShowTypeTb.Update(targetElement);

            return Ok("success");
       }
    }
}
