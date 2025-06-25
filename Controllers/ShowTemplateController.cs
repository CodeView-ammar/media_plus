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
    public class ShowTemplateController : Controller
    {
        private readonly UnitOfWork _unitOfWork = new();
        private readonly IRepository<User> _userTb;
        private readonly IRepository<Customer> _customerTb;
        private readonly IRepository<ShowTemplate> _ShowTemplateTb;
        private readonly IRepository<TemplateDetail> _ShowTemplateDetailTb;
        private readonly IHttpContextAccessor _accessor;
        private readonly CustomerSessionModel? _currentCustomer;
        private readonly ILogger<ShowTemplateController> _logger;
        private readonly IWebHostEnvironment env;
        private readonly bool isEnglishCulture ;

        public ShowTemplateController(ILogger<ShowTemplateController> logger,
                                  IWebHostEnvironment _env,
                                  IHttpContextAccessor accessor)
        {
            _customerTb = _unitOfWork.GetRepositoryInstance<Customer>();
            _userTb = _unitOfWork.GetRepositoryInstance<User>();
            _ShowTemplateTb = _unitOfWork.GetRepositoryInstance<ShowTemplate>();
            _ShowTemplateDetailTb = _unitOfWork.GetRepositoryInstance<TemplateDetail>();
            _accessor = accessor;
            _currentCustomer = _accessor.HttpContext.Session.GetObject<CustomerSessionModel>("CustomerObject");
            isEnglishCulture = _accessor.HttpContext.Request.Cookies[".AspNetCore.Culture"] == "c=en-US|uic=en-US";
            _logger = logger;
            env = _env;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var ShowTemplates =_ShowTemplateTb.EntitiesIQueryable()
                                    .Where(g=>g.TempCustCode == _currentCustomer.CustCode)
                                    .Select(x =>
                                    new ShowTemplateViewModel
                                    {
                                        TempId = x.TempId,
                                        TempCdate = x.TempCdate,
                                        TempUdate = x.TempUdate,
                                        TempIsactive = x.TempIsactive == 1,
                                        TempNameAr =isEnglishCulture? "null" :  x.TempNameAr,
                                        TempNameEng =isEnglishCulture? x.TempNameEng : "null",
                                        TempColNo = x.TempColNo,
                                        TempRowNo = x.TempRowNo,
                                        TempByUserName = isEnglishCulture ? _userTb.EntitiesIQueryable().FirstOrDefault(u => u.UserId == x.TempByUserid).UserNameEn
                                                                        : _userTb.EntitiesIQueryable().FirstOrDefault(u => u.UserId == x.TempByUserid).UserNameAr, 
                                        TempCustomerName = isEnglishCulture ? _customerTb.EntitiesIQueryable().FirstOrDefault(c => c.CustCode == x.TempCustCode).CustNameEn
                                                                        : _customerTb.EntitiesIQueryable().FirstOrDefault(c => c.CustCode == x.TempCustCode).CustNameAr
                                    
                                    });

            return View(ShowTemplates);
        }


       //=====================================================================
        // ShowTemplate Creation Part 
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }


        [HttpPost]
        public IActionResult Create(ShowTemplateViewModel showTemplateVM)
        {
            if (!ModelState.IsValid) 
                return View("Create"); 
            
            var newShowTemplate = new ShowTemplate()
                {
                    TempNameAr = showTemplateVM.TempNameAr,
                    TempNameEng = showTemplateVM.TempNameEng,
                    TempColNo = showTemplateVM.TempColNo,
                    TempRowNo = showTemplateVM.TempRowNo ,
                    TempByUserid = _accessor.HttpContext.Session.GetObject<User>("UserObject").UserId, // Get from session
                    TempCustCode = _accessor.HttpContext.Session.GetObject<Customer>("CustomerObject").CustCode, // Get from session
                    TempCdate = DateTime.Now,
                    TempUdate = DateTime.Now,
                    TempIsactive = 1,
                };

            _ShowTemplateTb.Add(newShowTemplate);

            var defaultHeight = 100 / showTemplateVM.TempRowNo;
            var defaultWidth = 100 / showTemplateVM.TempColNo;
            var ZoneCode = 0.0;
            for(int i = 1; i <= newShowTemplate.TempRowNo; i++)
            {
                ZoneCode = i;

                for(int j = 1; j <= newShowTemplate.TempColNo; j++)
                {
                    ZoneCode = Math.Round(ZoneCode+.1,1);

                    _ShowTemplateDetailTb.Add(new TemplateDetail
                    {
                        TempTempId = newShowTemplate.TempId,
                        TempByUserid = _accessor.HttpContext.Session.GetObject<User>("UserObject").UserId, // Get from session
                        TempCustCode = _accessor.HttpContext.Session.GetObject<Customer>("CustomerObject").CustCode, // Get from session
                        TempUdate = DateTime.Now,
                        TempCdate = DateTime.Now,
                        TempZoneCode = ZoneCode,
                        TempZoneHeight = defaultHeight, 
                        TempZoneWidth = defaultWidth,
                        TempIsactive = 1,
                    });
                }
            }

            return RedirectToAction("Index", "ShowTemplate");
        }


        //===================================================================== 
        // ShowTemplate Edit Part
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var ShowTemplate = _ShowTemplateTb.GetEntity(id);
            
            if (ShowTemplate == null)
            {
                return NotFound();
            }
            
            var ShowTemplateVM = new ShowTemplateViewModel{
                TempId = ShowTemplate.TempId,
                TempCustCode = ShowTemplate.TempCustCode,
                TempByUserid = ShowTemplate.TempByUserid,
                TempCdate = ShowTemplate.TempCdate,
                TempUdate = ShowTemplate.TempUdate,
                TempIsactive = ShowTemplate.TempIsactive == 1,
                TempNameAr = ShowTemplate.TempNameAr,
                TempNameEng = ShowTemplate.TempNameEng,
                TempColNo = ShowTemplate.TempColNo,
                TempRowNo = ShowTemplate.TempRowNo
            };

            return View(ShowTemplateVM);
        }

        [HttpPost]
        public IActionResult Edit(ShowTemplateViewModel ShowTemplateVM){

            if (!ModelState.IsValid) 
                return View("Edit");
            

            var ShowTemplate = new ShowTemplate()
                            {
                                TempId = ShowTemplateVM.TempId,
                                TempCustCode = ShowTemplateVM.TempCustCode,
                                TempByUserid = ShowTemplateVM.TempByUserid,
                                TempCdate = ShowTemplateVM.TempCdate,
                                TempUdate = DateTime.Now,
                                TempIsactive = ShowTemplateVM.TempIsactive ? 1 : 0,
                                TempNameAr = ShowTemplateVM.TempNameAr,
                                TempNameEng = ShowTemplateVM.TempNameEng,
                                TempColNo = ShowTemplateVM.TempColNo,
                                TempRowNo = ShowTemplateVM.TempRowNo
                            };
               
            _ShowTemplateTb.Update(ShowTemplate);

            return RedirectToAction("Index", "ShowTemplate");
        }

        //======================================================================
        // ShowTemplate Delete Part
        [HttpPost]
        public IActionResult Delete(int id)
       {
            var showTemplate = _ShowTemplateTb.EntitiesIQueryable().FirstOrDefault(s=>s.TempId == id);

            _ShowTemplateTb.Remove(id);
            
            _ShowTemplateDetailTb.EntitiesIQueryable()
                                 .Where(s=>s.TempTempId == id)
                                 .ToList()
                                 .ForEach(s=>_ShowTemplateDetailTb.Remove(s.TempDetail));


            return RedirectToAction("Index", "ShowTemplate");
       }


        // =====================================================================
       // Toggle active Part
       [HttpGet]
       public IActionResult ToggleState(int Id)
       {    
            var targetElement = _ShowTemplateTb.GetEntity(Id);

            targetElement.TempIsactive = targetElement.TempIsactive == 1 ? 0 : 1;

            _ShowTemplateTb.Update(targetElement);

            return Ok("success");
       }

        // =====================================================================
        
        public IActionResult TemplateDetail()
        {
            return View();
        }

        // =====================================================================
       // Show Template Details Part
       [HttpGet]
       public IActionResult GetTemplatesDetails(int Id){
            return Json(_ShowTemplateDetailTb.EntitiesIQueryable().Where(x => x.TempTempId == Id).ToList());
       }


        // =====================================================================
        // Show Template Part
        [HttpPost]
        public IActionResult UpdateTemplatesDetails([Bind(Prefix = "TempDetails")] List<TemplateDetail> templateDetails)
        {
            var errors = new List<string>();

            foreach (var item in templateDetails)
            {
                if (item.TempZoneHeight == null || item.TempZoneWidth == null || item.TempZoneHeight == 0 || item.TempZoneWidth == 0)
                {
                    errors.Add($"Œÿ√ ›Ì «·„‰ÿﬁ… —ﬁ„ {item.TempDetail}: ÌÃ» ≈œŒ«· «·⁄—÷ Ê«·«— ›«⁄.");
                    continue;
                }

                var targetElement = _ShowTemplateDetailTb.GetEntity(item.TempDetail);

                if (targetElement == null)
                {
                    errors.Add($"«·⁄‰’— »—ﬁ„ {item.TempDetail} €Ì— „ÊÃÊœ ›Ì ﬁ«⁄œ… «·»Ì«‰« .");
                    continue;
                }

                targetElement.TempZoneHeight = item.TempZoneHeight;
                targetElement.TempZoneWidth = item.TempZoneWidth;
                targetElement.TempIsactive = item.TempIsactive ?? 1;

                _ShowTemplateDetailTb.Update(targetElement);
            }

            TempData["TemplateErrors"] = "";
            if (errors.Any())
            {
                TempData["TemplateErrors"] = JsonSerializer.Serialize(errors);
            }
            else
            {
                TempData["SuccessMessage"] = " „ «·Õ›Ÿ »‰Ã«Õ.";
            }

            return RedirectToAction("Index", "ShowTemplate");
        }

    }
}
