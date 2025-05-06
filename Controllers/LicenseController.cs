
using System.Text.Json;
 using MediaPlus.DBModels;
using MediaPlus.DBModels.Repository;
using MediaPlus.Models.CustomFilters;
using MediaPlus.Models.ViewModels;
using MediaPlus.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MediaPlus.Controllers
{
    [AuthorizeCustFilter]
    public class LicenseController : Controller
    {
       
         private readonly UnitOfWork _unitOfWork = new();
        private readonly IRepository<Customer> _customerTb;
        private readonly IRepository<User> _userTb;
        private readonly IRepository<License> _licenseTb;
        private readonly IHttpContextAccessor _accessor;
        private readonly CustomerSessionModel? _currentCustomer;
        private readonly ILogger<LicenseController> _logger;
        private readonly Boolean isEnglishCulture ;

        public LicenseController(ILogger<LicenseController> logger,
                                  IHttpContextAccessor accessor)
        {
            _customerTb = _unitOfWork.GetRepositoryInstance<Customer>();
            _userTb = _unitOfWork.GetRepositoryInstance<User>();
            _licenseTb = _unitOfWork.GetRepositoryInstance<License>();
            _accessor = accessor;
            _currentCustomer = _accessor.HttpContext.Session.GetObject<CustomerSessionModel>("CustomerObject");
            isEnglishCulture = _accessor.HttpContext.Request.Cookies[".AspNetCore.Culture"] == "c=en-US|uic=en-US";
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Index()
        {
        var licenses =_licenseTb.EntitiesIQueryable()
                                .Select(x =>
                    new LicenseViewModel
                    {
                        LicId = x.LicId,
                        LicPeriod = new LicensePeriodViewModel
                                {
                                    LicStartAt = x.LicStartAt,
                                    LicEndAt = x.LicEndAt,
                                    LicCustCode = x.LicCustCode
                                },
                        LicDeviceNo = x.LicDeviceNo,
                        LicUdate = x.LicUdate,
                        LicCdate = x.LicCdate,
                        LicIsactive = x.LicIsactive == 1,
                        
                        LicUserName = (isEnglishCulture ? _userTb.EntitiesIQueryable().FirstOrDefault(u => u.UserId == x.LicUserNo)!.UserNameEn 
                                                        : _userTb.EntitiesIQueryable().FirstOrDefault(u => u.UserId == x.LicUserNo)!.UserNameAr) ?? "",
                        LicCustomerName = (isEnglishCulture ? _customerTb.EntitiesIQueryable().FirstOrDefault(c => c.CustIsactive == 1 && c.CustCode == x.LicCustCode)!.CustNameEn 
                                                            :_customerTb.EntitiesIQueryable().FirstOrDefault(c => c.CustIsactive == 1 && c.CustCode == x.LicCustCode)!.CustNameAr) ?? "", 
                    });

            return View(licenses);
        }

       //=====================================================================
        // License Creation Part 
        [HttpGet]
        public IActionResult Create()
        {
            ArrangeDropDownList();
            return View();
        }



        [HttpPost]
        public IActionResult Create(LicenseViewModel LicenseVM)
        {
            if (!ModelState.IsValid) {
                ArrangeDropDownList();
                return View("Create");
            } 

            Random rnd = new Random();

            var LicenseExistCheck = LicenseCheck(LicenseVM);

            _licenseTb.Add(
                new License()
                {
                    LicStartAt = LicenseVM.LicPeriod.LicStartAt,
                    LicEndAt = LicenseVM.LicPeriod.LicEndAt,
                    LicCustCode = LicenseVM.LicPeriod.LicCustCode,
                    LicUserNo = _accessor.HttpContext.Session.GetObject<User>("UserObject").UserId, // Get from session
                    LicDeviceNo = rnd.Next(1,100),
                    LicIsactive = LicenseExistCheck,
                    LicCdate = DateTime.Now,
                    LicUdate = DateTime.Now,
                }
            );

            return RedirectToAction("Index", "License");
        }


        //===================================================================== 
        // License Edit Part
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var License = _licenseTb.EntitiesIQueryable()
                                      .Where(l => l.LicId == id)
                                      .Select(x=>
                                        new LicenseViewModel{
                                            LicPeriod = new LicensePeriodViewModel
                                            {
                                                LicStartAt = x.LicStartAt,
                                                LicEndAt = x.LicEndAt,
                                                LicCustCode = x.LicCustCode,
                                            },
                                            LicDeviceNo = x.LicDeviceNo,
                                            LicUserNo = x.LicUserNo,
                                            LicCdate = x.LicCdate,
                                            LicUdate = x.LicUdate,
                                            LicId = x.LicId,
                                            LicIsactive = x.LicIsactive == 1
                                        }
                                      ).FirstOrDefault();

            if (License == null)
            {
                return NotFound();
            }

            return View(License);
        }

        [HttpPost]
        public IActionResult Edit(LicenseViewModel licenseVM){

            if (!ModelState.IsValid) 
                return View("Edit");

            var LicenseExistCheck = LicenseCheck(licenseVM);

            var license = new License(){
                        LicStartAt = licenseVM.LicPeriod.LicStartAt,
                        LicEndAt = licenseVM.LicPeriod.LicEndAt,
                        LicUdate = DateTime.Now,
                        LicCdate = licenseVM.LicCdate,
                        LicIsactive = LicenseExistCheck,
                        LicId = licenseVM.LicId,
                        LicUserNo = licenseVM.LicUserNo,
                        LicCustCode = licenseVM.LicPeriod.LicCustCode!,
                        LicDeviceNo = licenseVM.LicDeviceNo,
                        
                    };
               
            _licenseTb.Update(license);

            return RedirectToAction("Index", "License");
        }

       //======================================================================
       // License Soft Delete Part
       [HttpPost]
       public IActionResult Delete(int id)
       {
            _licenseTb.Remove(id);
            
            return RedirectToAction("Index", "License");
       }

        // =====================================================================
       // Toggle active Part
       [HttpGet]
       public IActionResult ToggleState(int Id)
       {    
            var targetElement = _licenseTb.GetEntity(Id);

            targetElement.LicIsactive = targetElement.LicIsactive == 1 ? 0 : 1;

            _licenseTb.Update(targetElement);

            return Ok("success");
       }

        //===========================================================================
        // Arrange Dropdownlist
        private void ArrangeDropDownList()
        {
            var Customers = new List<SelectListItem>();

            if(isEnglishCulture){
                Customers.AddRange(_customerTb.EntitiesIQueryable()
                                        .Select(c=>
                                                new SelectListItem
                                                {
                                                Text = c.CustNameEn!.ToString(),
                                                Value = c.CustCode.ToString(),
                                                }
                                        ).ToList());
            }else{
                Customers.AddRange(_customerTb.EntitiesIQueryable()
                                        .Select(c=>
                                                new SelectListItem
                                                {
                                                Text = c.CustNameAr!.ToString(),
                                                Value = c.CustCode.ToString(),
                                                }
                                        ).ToList());

            }
                                          
            ViewBag.CustomerSelectedList = Customers;
        }

        private int LicenseCheck(LicenseViewModel LicenseVM){

            if(!(LicenseVM.LicPeriod.LicStartAt <=  DateTime.Now && DateTime.Now <= LicenseVM.LicPeriod.LicEndAt))
                return 0;

            if(_licenseTb.EntitiesIQueryable().Where(l=>l.LicCustCode == LicenseVM.LicPeriod.LicCustCode).Any(l=>l.LicIsactive == 1))
                return 0;

            return 1; 
        }
    }
}