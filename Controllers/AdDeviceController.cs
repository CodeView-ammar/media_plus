using MediaPlus.DBModels;
using MediaPlus.DBModels.Repository;
using MediaPlus.Models.CustomFilters;
using MediaPlus.Models.ViewModels;
using MediaPlus.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

#pragma warning disable CS8602 // Dereference of a possibly null reference.

namespace MediaPlus.Controllers
{
    [AuthorizeCustFilter]
    public class AdDeviceController: Controller
    {
       
        private readonly UnitOfWork _unitOfWork = new();
        private readonly IRepository<User> _userTb;
        private readonly IRepository<Customer> _customerTb;
        private readonly IRepository<AdDevice> _AdDeviceTb;
        private readonly IRepository<AdGroup> _AdGroupTb;
        private readonly ILogger<AdDeviceController> _logger;
        private readonly IWebHostEnvironment env;
        private readonly IHttpContextAccessor _accessor;
        private readonly Boolean isEnglishCulture;
        private readonly UserSessionModel _currentUser;
        private readonly CustomerSessionModel? _currentCustomer;
        public AdDeviceController
            (
            ILogger<AdDeviceController> logger,
            IWebHostEnvironment _env,
            IHttpContextAccessor accessor
            )
        {
            _customerTb = _unitOfWork.GetRepositoryInstance<Customer>();
            _userTb = _unitOfWork.GetRepositoryInstance<User>();
            _AdDeviceTb = _unitOfWork.GetRepositoryInstance<AdDevice>();
            _AdGroupTb = _unitOfWork.GetRepositoryInstance<AdGroup>();
            _accessor = accessor;
            _logger = logger;
            isEnglishCulture = _accessor.HttpContext.Request.Cookies[".AspNetCore.Culture"] == "c=en-US|uic=en-US";
            _currentUser = _accessor.HttpContext.Session.GetObject<UserSessionModel>("UserObject");
            _currentCustomer = _accessor.HttpContext.Session.GetObject<CustomerSessionModel>("CustomerObject");
            env = _env;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var AdDevices =_AdDeviceTb
                        .EntitiesIQueryable()
                        .Where(d=>d.DevicesCustCode == _currentCustomer.CustCode)
                        .Select(x =>
                        new AdDeviceViewModel
                        { 
                            DevicesId = x.DevicesId,
                            DevicesName = x.DevicesName,
                            DevicesUdate = x.DevicesUdate,
                            DevicesCdate = x.DevicesCdate,
                            DevicesOnoff = x.DevicesOnoff == 1,
                            DevicesOfflinePhoto = x.DevicesOfflinePhoto,
                            DevicesGroupname = _AdGroupTb.EntitiesIQueryable().FirstOrDefault(g => g.GroupId == x.DevicesGroupid).GroupName,

                            DevicesByUserName = isEnglishCulture? _userTb.EntitiesIQueryable().FirstOrDefault(u=> u.UserId == x.DevicesByUserid).UserNameEn
                                                                : _userTb.EntitiesIQueryable().FirstOrDefault(u=> u.UserId == x.DevicesByUserid).UserNameAr,

                            DevicesCustomerName = isEnglishCulture? _customerTb.EntitiesIQueryable().FirstOrDefault(c => c.CustCode == x.DevicesCustCode).CustNameEn
                                                                :_customerTb.EntitiesIQueryable().FirstOrDefault(c => c.CustCode == x.DevicesCustCode).CustNameAr
                        }).ToList();

            
            return View(AdDevices);
        }


       //=====================================================================
        // AdDevice Creation Part 
        [HttpGet]
        public IActionResult Create()
        {
            ArrangeDropdownlistData();
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> CreateAsync(AdDeviceViewModel adDeviceViewModel)
        {
            if (!ModelState.IsValid) {
                ArrangeDropdownlistData();
                return View("Create"); 
            }
            string filename;

            if(adDeviceViewModel.DevicesOfflinePhotoFile != null)
            {
                // Upload the valid photo to specific folder
                //              Guid to handle Duplication    +      Extension of the file
                filename = Guid.NewGuid().ToString().Substring(0,18) + Path.GetExtension(adDeviceViewModel.DevicesOfflinePhotoFile.FileName);
                var path = Path.Combine(env.WebRootPath, "upload/device/photo", filename);
                using (var fileStream = System.IO.File.Create(path))
                {
                    await adDeviceViewModel.DevicesOfflinePhotoFile.CopyToAsync(fileStream);
                }
                filename = filename+","+adDeviceViewModel.DevicesOfflinePhotoFile.ContentType;
            }else{
                filename = "default_device.jpg,image/jpg";
            }

            _AdDeviceTb.Add(
                    new AdDevice()
                    {
                        DevicesByUserid = _currentUser.UserId, // Get from session
                        DevicesCustCode = _currentCustomer.CustCode, // Get from session
                        DevicesCdate = DateTime.Now,
                        DevicesUdate = DateTime.Now,
                        DevicesOnoff = 1,
                        DevicesName = adDeviceViewModel.DevicesName,
                        DevicesGroupid = adDeviceViewModel.DevicesGroupid,
                        DevicesOfflinePhoto = filename,
                    }
            );

            // Should here add show related to this device

            return RedirectToAction("Index", "AdDevice");

        }
 

        //===================================================================== 
        // AdDevice Edit Part
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var AdDevice = _AdDeviceTb.GetEntity(id);
            
            if (AdDevice == null)
            {
                return NotFound();
            }

            ArrangeDropdownlistData();

            var AdDeviceVM = new AdDeviceViewModel{
                DevicesId = AdDevice.DevicesId,
                DevicesName = AdDevice.DevicesName,
                DevicesUdate = AdDevice.DevicesUdate,
                DevicesCdate = AdDevice.DevicesCdate,
                DevicesOnoff = AdDevice.DevicesOnoff == 1,
                DevicesOfflinePhoto = AdDevice.DevicesOfflinePhoto,
                DevicesGroupid = AdDevice.DevicesGroupid!.Value
            };

            return View(AdDeviceVM);
        }

        [HttpPost]
        public IActionResult Edit(AdDeviceViewModel AdDeviceVM){

            if (!ModelState.IsValid) {
                ArrangeDropdownlistData();
                return View("Edit");
            }
            
            var targetDevice = _AdDeviceTb.GetEntity(AdDeviceVM.DevicesId);

            if(AdDeviceVM.DevicesOfflinePhotoFile != null){
                if(targetDevice.DevicesOfflinePhoto.Split(",")[0] != "default_device.jpg"){
                    //delete previos photo
                    System.IO.File.Delete(Path.Combine(env.WebRootPath, "upload/device/photo", targetDevice.DevicesOfflinePhoto.Split(",")[0]));
                }

                // Upload the valid photo to spacific folder
                string filename = Guid.NewGuid().ToString().Substring(0,18) + Path.GetExtension(AdDeviceVM.DevicesOfflinePhotoFile.FileName);
                var path = Path.Combine(env.WebRootPath, "upload/device/photo", filename);
                using (var fileStream = System.IO.File.Create(path))
                {
                    AdDeviceVM.DevicesOfflinePhotoFile.CopyTo(fileStream);
                }
                AdDeviceVM.DevicesOfflinePhoto = filename+","+AdDeviceVM.DevicesOfflinePhotoFile.ContentType;
            }

            targetDevice.DevicesName = AdDeviceVM.DevicesName;
            targetDevice.DevicesUdate = DateTime.Now;
            targetDevice.DevicesOnoff = AdDeviceVM.DevicesOnoff ?1:0;
            targetDevice.DevicesGroupid = AdDeviceVM.DevicesGroupid;
               
            _AdDeviceTb.Update(targetDevice);

            return RedirectToAction("Index", "AdDevice");
        }

        //======================================================================
        // Device Download Photo Part
        [HttpGet]
        public IActionResult Download(int Id)
        {
            var selectedFile = _AdDeviceTb.GetEntity(Id);
            
            if (selectedFile == null)
            {
                return NotFound();
            }

            var path = "~/upload/device/photo/" + selectedFile.DevicesOfflinePhoto.Split(",")[0];
            var ContentType = selectedFile.DevicesOfflinePhoto.Split(",")[1];
            var fileName = selectedFile.DevicesOfflinePhoto.Split(",")[0];

            Response.Headers.Add("Content-Disposition", "attachment; filename=" + fileName);
            Response.Headers.Add("Expires", DateTime.UtcNow.AddDays(-3).ToString("R"));
            Response.Headers.Add("Cache-Control", "no-cache");
            return File(path, ContentType, fileName);
        }

       //======================================================================
       // AdDevice Delete Part
       [HttpGet]
       public IActionResult Delete(int id)
       {
            var selectedFile = _AdDeviceTb.GetEntity(id);
            if(selectedFile.DevicesOfflinePhoto.Split(",")[0] != "default_device.jpg"){
                var path = Path.Combine(env.WebRootPath, "upload/device/photo"      
                                            , selectedFile.DevicesOfflinePhoto.Split(",")[0]);
                System.IO.File.Delete(path);
            }
            
            _AdDeviceTb.Remove(id);
            
            return RedirectToAction("Index", "AdDevice");
       }

        //======================================================================
       // AdDevice Toggle active Part
       [HttpGet]
       public IActionResult ToggleState(int Id)
       {    
            var targetElement = _AdDeviceTb.GetEntity(Id);

            targetElement.DevicesOnoff = targetElement.DevicesOnoff == 1 ? 0 : 1;

            _AdDeviceTb.Update(targetElement);

            return Ok("success");
       }

        private void ArrangeDropdownlistData()
        {
            var groups = new List<SelectListItem>();


            groups.AddRange(_AdGroupTb.EntitiesIQueryable()
                                      .Where(r=>r.GroupIsactive == 1
                                            && r.GroupCustCode == _currentCustomer.CustCode)  
                                      .Select(c=>
                                                            new SelectListItem
                                                            {
                                                                Text = c.GroupName,
                                                                Value = c.GroupId.ToString(),
                                                            }
                                        ).ToList());
          
                                                    
            ViewBag.GroupSelectedList = groups;
        }
    }
}
