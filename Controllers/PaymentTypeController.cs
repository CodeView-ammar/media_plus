using System;
using System.Linq;
using MediaPlus.DBModels;
using MediaPlus.DBModels.Repository;
using MediaPlus.Models.Constants;
using MediaPlus.Models.CustomFilters;
using MediaPlus.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using MediaPlus.Services;

namespace MediaPlus.Controllers
{
    [AuthorizeCustFilter]
    public class PaymentTypeController : Controller
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly IRepository<User> _userTb;
        private readonly IRepository<Customer> _customerTb;
        private readonly IRepository<PaymentType> _paymentTypeTb;
        private readonly IHttpContextAccessor _accessor;
        private readonly CustomerSessionModel _currentCustomer;
        private readonly ILogger<PaymentTypeController> _logger;
        private readonly IWebHostEnvironment _env;
        private readonly bool _isEnglishCulture;

        public PaymentTypeController(
            ILogger<PaymentTypeController> logger,
            IWebHostEnvironment env,
            IHttpContextAccessor accessor,
            UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _customerTb = _unitOfWork.GetRepositoryInstance<Customer>();
            _userTb = _unitOfWork.GetRepositoryInstance<User>();
            _paymentTypeTb = _unitOfWork.GetRepositoryInstance<PaymentType>();
            _logger = logger;
            _accessor = accessor;
            _currentCustomer = _accessor.HttpContext.Session.GetObject<CustomerSessionModel>("CustomerObject");
            _isEnglishCulture = _accessor.HttpContext.Request.Cookies[".AspNetCore.Culture"] == "c=en-US|uic=en-US";
            _env = env;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var mTypes = _paymentTypeTb.EntitiesIQueryable()
                .Where(g => g.TypeCustCode == _currentCustomer.CustCode)
                .Select(x => new PaymentTypeViewModel
                {
                    TypeId = x.TypeId,
                    TypeName = _isEnglishCulture ? "null" : x.TypeName,
                    TypeShortName = _isEnglishCulture ? x.TypeShortName : "null",
                    TypeCreateAt =  (DateTime?)null,
                    TypeUpdateAt = (DateTime?)null,
                    TypeIsActive = !x.TypeIsActive,
                    TypeAddByUserId = 1,
                    TypeCustCode = ""
                });

            return View(mTypes);
        }

        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.isEnglishCulture = _isEnglishCulture;
            return View();
        }

        [HttpPost]
        public IActionResult Create(PaymentTypeViewModel mTypeVM)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.isEnglishCulture = _isEnglishCulture;
                return View(mTypeVM);
            }

            _paymentTypeTb.Add(new PaymentType
            {
                TypeIsActive = true,
                TypeCreateAt = DateTime.Now,
                TypeUpdateAt = DateTime.Now,
                TypeShortName = mTypeVM.TypeName.ToLower(), //  ⁄œÌ·
                TypeName = mTypeVM.TypeName,
                TypeCustCode = _currentCustomer.CustCode,
                TypeAddByUserId = _accessor.HttpContext.Session.GetObject<User>("UserObject").UserId
            });

            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var paymentType = _paymentTypeTb.GetEntity(id);
            if (paymentType == null)
            {
                return NotFound();
            }

            var paymentTypeVM = new PaymentTypeViewModel
            {
                TypeId = paymentType.TypeId,
                TypeName = paymentType.TypeName,
                TypeShortName = paymentType.TypeShortName.ToLower(),
                TypeCreateAt = paymentType.TypeCreateAt,
                TypeUpdateAt = paymentType.TypeUpdateAt,
                TypeIsActive = !paymentType.TypeIsActive,
                TypeCustCode = paymentType.TypeCustCode
            };

            return View(paymentTypeVM);
        }

        [HttpPost]
        public IActionResult Edit(PaymentTypeViewModel mTypeVM)
        {
            if (!ModelState.IsValid)
            {
                return View(mTypeVM);
            }

            var mType = new PaymentType
            {
                TypeId = mTypeVM.TypeId,
                TypeName = mTypeVM.TypeName,
                TypeShortName = mTypeVM.TypeShortName,
                TypeCreateAt = (DateTime)mTypeVM.TypeCreateAt,
                TypeUpdateAt = DateTime.Now,
                TypeCustCode = mTypeVM.TypeCustCode,
                TypeIsActive = mTypeVM.TypeIsActive
            };

            _paymentTypeTb.Update(mType);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            _paymentTypeTb.Remove(id);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult ToggleState(int id)
        {
            var targetElement = _paymentTypeTb.GetEntity(id);
            if (targetElement != null)
            {
                targetElement.TypeIsActive = !targetElement.TypeIsActive;
                _paymentTypeTb.Update(targetElement);
                return Ok("success");
            }
            return NotFound();
        }
    }
}