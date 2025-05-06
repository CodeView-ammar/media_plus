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

#pragma warning disable CS8602 // Dereference of a possibly null reference.

namespace MediaPlus.Controllers
{
    [AuthorizeCustFilter]
    public class AdGroupController : Controller
    {
       
        private readonly UnitOfWork _unitOfWork = new();
        private readonly IRepository<User> _userTb;
        private readonly IRepository<Customer> _customerTb;
        private readonly IRepository<AdGroup> _AdGroupTb;
        private readonly ILogger<AdGroupController> _logger;
        private readonly Boolean isEnglishCulture ;
        private readonly IHttpContextAccessor _accessor;
        private readonly CustomerSessionModel? _currentCustomer;
        public AdGroupController(ILogger<AdGroupController> logger,
                                  IHttpContextAccessor accessor)
        {
            _customerTb = _unitOfWork.GetRepositoryInstance<Customer>();
            _userTb = _unitOfWork.GetRepositoryInstance<User>();
            _AdGroupTb = _unitOfWork.GetRepositoryInstance<AdGroup>();
            _accessor = accessor; 
            _currentCustomer = _accessor.HttpContext.Session.GetObject<CustomerSessionModel>("CustomerObject");
            _logger = logger;
            isEnglishCulture = _accessor.HttpContext.Request.Cookies[".AspNetCore.Culture"] == "c=en-US|uic=en-US";

        }

        [HttpGet]
        public IActionResult Index()
        {
            var AdGroups =_AdGroupTb.EntitiesIQueryable()
                                    .Where(g=>g.GroupCustCode == _currentCustomer.CustCode)
                                    .Select(x =>
                                    new AdGroupViewModel
                                    {
                                        GroupId = x.GroupId,
                                        GroupName = x.GroupName,
                                        GroupCdate = x.GroupCdate,
                                        GroupIsactive = x.GroupIsactive == 1,
                                        GroupUdate = x.GroupUdate,
                                        GroupUserName = isEnglishCulture ? _userTb.EntitiesIQueryable().FirstOrDefault(u => u.UserId == x.GroupByUserid).UserNameEn
                                                                        : _userTb.EntitiesIQueryable().FirstOrDefault(u => u.UserId == x.GroupByUserid).UserNameAr, 
                                        GroupCustomerName = isEnglishCulture ? _customerTb.EntitiesIQueryable().FirstOrDefault(c => c.CustCode == x.GroupCustCode).CustNameEn
                                                                        : _customerTb.EntitiesIQueryable().FirstOrDefault(c => c.CustCode == x.GroupCustCode).CustNameAr
                                    
                                    });

            
            return View(AdGroups);
        }


       //=====================================================================
        // AdGroup Creation Part 
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }


        [HttpPost]
        public IActionResult Create(AdGroupViewModel GroupVM)
        {

            if (!ModelState.IsValid) 
                return View("Create"); 
            
            _AdGroupTb.Add(
                    new AdGroup()
                    {
                        GroupByUserid = _accessor.HttpContext.Session.GetObject<User>("UserObject").UserId, // Get from session
                        GroupCustCode = _accessor.HttpContext.Session.GetObject<Customer>("CustomerObject").CustCode, // Get from session
                        GroupCdate = DateTime.Now,
                        GroupUdate = DateTime.Now,
                        GroupIsactive = 1,
                        GroupName = GroupVM.GroupName,
                    }
            );

            return RedirectToAction("Index", "AdGroup");

        }


        //===================================================================== 
        // AdGroup Edit Part
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var AdGroup = _AdGroupTb.GetEntity(id);
            
            if (AdGroup == null)
            {
                return NotFound();
            }
            
            var AdGroupVM = new AdGroupViewModel{
                    GroupId = AdGroup.GroupId,
                    GroupName = AdGroup.GroupName,
                    GroupCdate = AdGroup.GroupCdate,
                    GroupIsactive = AdGroup.GroupIsactive == 1,
                    GroupUdate = AdGroup!.GroupUdate,
                    GroupByUserid = AdGroup.GroupByUserid,
                    GroupCustCode = AdGroup.GroupCustCode,
            };

            return View(AdGroupVM);
        }

        [HttpPost]
        public IActionResult Edit(AdGroupViewModel AdGroupVM){

            if (!ModelState.IsValid) 
                return View("Edit");
            

            var AdGroup = new AdGroup()
                            {
                                GroupId = AdGroupVM.GroupId,
                                GroupCustCode = AdGroupVM.GroupCustCode, 
                                GroupByUserid = AdGroupVM.GroupByUserid, 
                                GroupCdate = AdGroupVM.GroupCdate.Value,
                                GroupUdate = DateTime.Now,
                                GroupIsactive = AdGroupVM.GroupIsactive ? 1 : 0,
                                GroupName = AdGroupVM.GroupName,       
                            };
               
            _AdGroupTb.Update(AdGroup);

            return RedirectToAction("Index", "AdGroup");
        }

       //======================================================================
       // AdGroup Delete Part
       [HttpGet]
       public IActionResult Delete(int id)
       {
            _AdGroupTb.Remove(id);
            
            return RedirectToAction("Index", "AdGroup");
       }
        // =====================================================================
       // AdDevice Toggle active Part
       [HttpGet]
       public IActionResult ToggleState(int Id)
       {    
            var targetElement = _AdGroupTb.GetEntity(Id);

            targetElement.GroupIsactive = targetElement.GroupIsactive == 1 ? 0 : 1;

            _AdGroupTb.Update(targetElement);

            return Ok("success");
       }

    }
}
