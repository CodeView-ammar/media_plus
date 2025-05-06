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
    public class ShowSettingController : Controller
    {
       
        private readonly UnitOfWork _unitOfWork = new();
        private readonly IRepository<ShowSetting> _ShowSettingTb;
        private readonly IRepository<AdGroup> _GroupTb;
        private readonly IRepository<AdDevice> _DeviceTb;
        private readonly ILogger<ShowSettingController> _logger;
        private readonly IHttpContextAccessor _accessor;
        private readonly CustomerSessionModel? _currentCustomer;

        public ShowSettingController(ILogger<ShowSettingController> logger,
                                  IHttpContextAccessor accessor)
        {
            _GroupTb = _unitOfWork.GetRepositoryInstance<AdGroup>();
            _DeviceTb = _unitOfWork.GetRepositoryInstance<AdDevice>();
            _ShowSettingTb = _unitOfWork.GetRepositoryInstance<ShowSetting>();
            _accessor = accessor;
            _currentCustomer = _accessor.HttpContext.Session.GetObject<CustomerSessionModel>("CustomerObject");
            _logger = logger;

        }


       //=====================================================================
        // ShowSetting Creation Part 
        [HttpGet]
        public IActionResult Create()
        {
            ArrangeDropdownlistData();
            return View();
        }


        [HttpPost]
        public IActionResult CreateConfirmation(ShowCreationViewModel ShowVm)
        {
            if (!ModelState.IsValid) {
                ArrangeDropdownlistData();
                return View("Create"); 
            }
            TempData.SetObject("ShowSetting", ShowVm.showSetting);
            return RedirectToAction("CreatePropagation");
        }

        [HttpGet]
        public IActionResult CreatePropagation()
        {
            var showSettings = TempData.GetObject<ShowSetting>("ShowSetting");
            var devices = _DeviceTb.EntitiesIQueryable()
                                    .Where(d=>d.DevicesGroupid == showSettings.ShowSettingGroupId
                                         && d.DevicesCustCode == _currentCustomer!.CustCode)
                                   .ToList();
            

            if(devices == null){

                showSettings = new ShowSetting()
                        {
                            ShowSettingGroupId = showSettings?.ShowSettingGroupId,
                            ShowSettingTotalView = showSettings?.ShowSettingTotalView,
                            ShowSettingDeviceId = -1,
                            ShowSettingPresent = 1,
                            ShowSettingNext = 2,
                            ShowSettingCdate = DateTime.Now,
                            ShowSettingUdate = DateTime.Now,
                            ShowSettingShowcode = Guid.NewGuid().ToString("n").Substring(0, 8),
                            ShowSettingCustCode = _currentCustomer!.CustCode
                        };
            }else{
                
                var Showcode = Guid.NewGuid().ToString("n").Substring(0, 8);
                
                foreach(var device in devices){
                    showSettings = new ShowSetting()
                            {
                                ShowSettingGroupId = showSettings?.ShowSettingGroupId,
                                ShowSettingTotalView = showSettings?.ShowSettingTotalView,
                                ShowSettingDeviceId = device.DevicesId,
                                ShowSettingPresent = 1,
                                ShowSettingNext = 2,
                                ShowSettingCdate = DateTime.Now,
                                ShowSettingUdate = DateTime.Now,
                                ShowSettingShowcode = Showcode,
                                ShowSettingCustCode = _currentCustomer!.CustCode
                            };
                }
            }

            _ShowSettingTb.Add(showSettings);
            TempData.UpdateObject("ShowSetting", showSettings);

            return RedirectToAction("Create", "Show", new { NumberOfShows = showSettings?.ShowSettingTotalView });
        }

       //======================================================================
       // ShowSetting Delete Part
       [HttpGet]
       public IActionResult Delete(string id) // Showcode
       {
            try
            {

            var ShowSetting = _ShowSettingTb.EntitiesIQueryable().FirstOrDefault(c => c.ShowSettingShowcode == id);
            
            _ShowSettingTb.Remove(ShowSetting.ShowSettingId);

            return RedirectToAction("Delete", "Show", new { showCode = id });
            }catch(Exception e)
            {
                return BadRequest();
            }
       }

       //======================================================================================
        private void ArrangeDropdownlistData()
        {
            var groups = new List<SelectListItem>();

            groups.AddRange(_GroupTb.EntitiesIQueryable()
                                        .Where(c=>c.GroupIsactive == 1
                                            && c.GroupCustCode == _currentCustomer!.CustCode)
                                            .Select(c=>
                                                    new SelectListItem
                                                    {
                                                    Text = c.GroupName.ToString(),
                                                    Value = c.GroupId.ToString(),
                                                    }
                                            ).ToList());
                                          
            ViewBag.GroupSelectedList = groups;
        }
       

    }
}
