
using Microsoft.AspNetCore.Mvc;
 using MediaPlus.DBModels;
using MediaPlus.DBModels.Repository;
using MediaPlus.Models.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using MediaPlus.Models.CustomFilters;
using System.Text.Json;
using MediaPlus.Services;
using Microsoft.EntityFrameworkCore;
 using MediaPlus.DBModels;
using System.Text.Json.Serialization;
using Newtonsoft.Json;


namespace MediaPlus.Controllers
{
    [AuthorizeCustFilter]
    public class CustomerController : Controller
    {
        private readonly UnitOfWork _unitOfWork = new();
        private readonly IRepository<ShowHtmlcode> _showHtmlTb;
        private readonly IRepository<Customer> _customerTb;
        private readonly IRepository<License> _licenseTb;
        private readonly IRepository<ShowMaterial> _materialTb;
        private readonly IRepository<MaterialType> _materialTypeTb;
        private readonly IRepository<User> _userTb;
        private readonly IRepository<UserRole> _roleTb;
        private readonly IRepository<AdDevice> _deviceTb;
        private readonly IRepository<AdGroup> _groupTb;
        private readonly IRepository<Show> _showTb;
        private readonly IRepository<UserPermission> _permissionTb;
        private readonly IRepository<RoleWithPermission> _roleWithPermissionTb;
        private readonly IWebHostEnvironment env;
        private readonly IHttpContextAccessor _accessor;
        private readonly CustomerSessionModel? _currentCustomer;  
        private readonly UserSessionModel _currentUser;  
        private readonly bool isEnglishCulture;
        private readonly IRepository<ShowContent> _showContentTb;
        private readonly IRepository<ShowSetting> _showSettingTb;
        private readonly IRepository<ShowDetail> _showDetailTb;
        private readonly IRepository<ShowTemplate> _showTemplateTb;
        private readonly MediaPlusDbContext _c;
        public CustomerController(IWebHostEnvironment _env,
                                  IHttpContextAccessor accessor,
                                  MediaPlusDbContext c)
        {
            _showHtmlTb = _unitOfWork.GetRepositoryInstance<ShowHtmlcode>();
            _customerTb = _unitOfWork.GetRepositoryInstance<Customer>();
            _licenseTb = _unitOfWork.GetRepositoryInstance<License>();
            _materialTb = _unitOfWork.GetRepositoryInstance<ShowMaterial>();
            _materialTypeTb = _unitOfWork.GetRepositoryInstance<MaterialType>();
            _userTb = _unitOfWork.GetRepositoryInstance<User>();
            _roleTb = _unitOfWork.GetRepositoryInstance<UserRole>();
            _permissionTb = _unitOfWork.GetRepositoryInstance<UserPermission>();
            _roleWithPermissionTb = _unitOfWork.GetRepositoryInstance<RoleWithPermission>();
            _deviceTb = _unitOfWork.GetRepositoryInstance<AdDevice>();
            _groupTb = _unitOfWork.GetRepositoryInstance<AdGroup>();
            _showTb = _unitOfWork.GetRepositoryInstance<Show>();
            _showContentTb = _unitOfWork.GetRepositoryInstance<ShowContent>();
            _showSettingTb = _unitOfWork.GetRepositoryInstance<ShowSetting>();
            _showDetailTb = _unitOfWork.GetRepositoryInstance<ShowDetail>();
            _showTemplateTb = _unitOfWork.GetRepositoryInstance<ShowTemplate>();

            _accessor = accessor;
            _currentUser = _accessor.HttpContext.Session.GetObject<UserSessionModel>("UserObject");
            _currentCustomer = _accessor.HttpContext.Session.GetObject<CustomerSessionModel>("CustomerObject");
            isEnglishCulture = accessor.HttpContext!.Request.Cookies[".AspNetCore.Culture"] == "c=en-US|uic=en-US";
            env = _env;
            _c = c;
        }

        public async Task<IActionResult> Index()
        {
            var asd = _customerTb.GetAllEntities();
            var customerData =_customerTb.EntitiesIQueryable()
                                        .Select(x =>
                    new CustomerViewModel
                    {
                        CustId = x.CustId,
                        CustCode = x.CustCode,
                        CustNameAr = isEnglishCulture? "null" : x.CustNameAr,
                        CustNameEn = isEnglishCulture ? x.CustNameEn : "null",
                        CustEmail = x.CustEmail.Substring(0, 10) + "...",
                        CustCdate = x.CustCdate,
                        CustUdate = x.CustUdate,
                        CustState = x.CustIsactive == 1,
                        UserName = isEnglishCulture? _userTb.EntitiesIQueryable().FirstOrDefault(u => u.UserCustCode == x.CustCode).UserNameEn
                                                   : _userTb.EntitiesIQueryable().FirstOrDefault(u => u.UserCustCode == x.CustCode).UserNameAr,
                        CustLogoPath = x.CustLogo,
                    });
            var a = JsonConvert.SerializeObject(customerData);
            return View(customerData);
        }


        //=====================================================================
        // Customer Creation Part 
        [HttpGet]
        public IActionResult Create()
        {
            ArrangeDropdownlistData();
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Create(CustomerViewModel customerVM)
        {     
            if (!ModelState.IsValid) 
            {
                ArrangeDropdownlistData();
                return View("Create");
            } 

            string filename = null;

            if(customerVM.CustLogo != null)
            {
                // Upload the valid photo to spacific folder
                //              Guid to handle Dublication    +      Extention of the file
                filename = Guid.NewGuid().ToString().Substring(0,18) + Path.GetExtension(customerVM.CustLogo.FileName);
                var path = Path.Combine(env.WebRootPath, "upload/customer/photo", filename);
                using (var fileStream = System.IO.File.Create(path))
                {
                    await customerVM.CustLogo.CopyToAsync(fileStream);
                }
            }

            _customerTb.Add(
                new Customer(){
                    CustCode = Guid.NewGuid().ToString("n").Substring(0, 8),
                    //CustToken = Guid.NewGuid().ToString("n").Substring(0, 15),
                    CustNameAr = customerVM.CustNameAr,
                    CustNameEn = customerVM.CustNameEn,
                    CustMobileNo=customerVM.CustMobileNo,
                    CustTel = customerVM.CustTel,
                    CustVat = customerVM.CustVatNo,
                    CustTrNo = customerVM.CustTrNo,
                    CustEmail = customerVM.CustEmail,
                    CustIsactive = 1,
                    CustLicenseCode = 12312, // work to do
                    CustCdate = DateTime.Now,
                    CustUdate = DateTime.Now,
                    CustLogo = filename == null ? "default_logo.png,image/png" : filename+","+customerVM.CustLogo.ContentType,
                }
            );

            return RedirectToAction("Index", "Customer");
        
        }


        //===================================================================== 
        // Customer Edit Part
        [HttpGet]
        public IActionResult Edit(int id)
        {
            ArrangeDropdownlistData();
            var customer = _customerTb.EntitiesIQueryable()
                                      .Where(c => c.CustId == id)
                                      .Select(x=>
                                        new CustomerViewModel{
                                            CustId = x.CustId,
                                            CustCode = x.CustCode,
                                            CustNameAr = x.CustNameAr,
                                            CustNameEn = x.CustNameEn ,
                                            CustEmail = x.CustEmail,
                                            CustTrNo = x.CustTrNo,
                                            CustVatNo = x.CustVat,
                                            CustTel = x.CustTel,
                                            CustMobileNo = x.CustMobileNo,
                                            CustCdate = x.CustCdate,
                                            CustUdate = x.CustUdate,
                                            CustLogoPath = x.CustLogo,
                                            CustState = x.CustIsactive == 1
                                        }
                                      ).FirstOrDefault();

            if (customer == null)
            {
                return NotFound();
            }

            return View(customer);
        }

        [HttpPost]
        public IActionResult Edit(CustomerViewModel customerVM){

            if (!ModelState.IsValid){
                ArrangeDropdownlistData();
                return View("Edit");
            }

            var targetCustomer = _customerTb.GetEntity(customerVM.CustId);

            if(customerVM.CustLogo != null){
                //delete previos photo
                if (targetCustomer.CustLogo != "default_logo.png,image/png"){
                    System.IO.File.Delete(Path.Combine(env.WebRootPath, "upload/customer/photo", targetCustomer.CustLogo.Split(",")[0]));
                }
                // Upload the valid photo to spacific folder
                string filename = Guid.NewGuid().ToString().Substring(0,18) + Path.GetExtension(customerVM.CustLogo.FileName);
                var path = Path.Combine(env.WebRootPath, "upload/customer/photo", filename);
                using (var fileStream = System.IO.File.Create(path))
                {
                    customerVM.CustLogo.CopyTo(fileStream);
                }
                customerVM.CustLogoPath = filename+","+customerVM.CustLogo.ContentType;
            }

            targetCustomer.CustNameAr = customerVM.CustNameAr;
            targetCustomer.CustNameEn = customerVM.CustNameEn;
            targetCustomer.CustEmail = customerVM.CustEmail;
            targetCustomer.CustTrNo = customerVM.CustTrNo;
            targetCustomer.CustVat = customerVM.CustVatNo;
            targetCustomer.CustTel = customerVM.CustTel;
            targetCustomer.CustMobileNo = customerVM.CustMobileNo;
            targetCustomer.CustIsactive = customerVM.CustState ? 1 : 0;
            targetCustomer.CustLogo = customerVM.CustLogoPath;
            targetCustomer.CustUdate =  DateTime.Now;
 
            _customerTb.Update(targetCustomer);

            return RedirectToAction("Index", "Customer");
        }

    
       //======================================================================
       // Customer Soft Delete Part
       [HttpPost]
       public IActionResult Delete(int id)
       {    
            //delete attached photo
            var selectedItem = _customerTb.GetEntity(id);

            if (selectedItem.CustLogo != "default_logo.png,image/png")
            {
                var path = Path.Combine(env.WebRootPath, "upload/customer/photo", selectedItem.CustLogo.Split(",")[0]);
                System.IO.File.Delete(path);
            }

            DeleteAllRelatedCustomerData(selectedItem.CustCode);

            _customerTb.Remove(id);
            
            return RedirectToAction("Index", "Customer");
       }


        // =====================================================================
       // Toggle active Part
       [HttpGet]
       public IActionResult ToggleState(int Id)
       {    
            var targetElement = _customerTb.GetEntity(Id);

            targetElement.CustIsactive = targetElement.CustIsactive == 1 ? 0 : 1;

            _customerTb.Update(targetElement);

            return Ok("success");
       }

        //======================================================================
        // Customer Download Photo Part
        [HttpGet]
        public IActionResult Download(int Id)
        {
            var selectedFile = _customerTb.GetEntity(Id);
            if (selectedFile == null)
            {
                return NotFound();
            }

            var path = "~/upload/customer/photo/" + selectedFile.CustLogo.Split(",")[0];
            var contentType = selectedFile.CustLogo.Split(",")[1];
            var fileName = selectedFile.CustLogo.Split(",")[0];

            Response.Headers.Add("Content-Disposition", "attachment; filename=\"" + fileName + "\"");
            // Use a standard format for the Expires header
            Response.Headers.Add("Expires", DateTime.UtcNow.AddDays(-3).ToString("R")); // RFC1123 format
            Response.Headers.Add("Cache-Control", "no-cache");

            return File(path, contentType, fileName);
        }

        private void ArrangeDropdownlistData()
        {
            var roles = new List<SelectListItem>();
            if (isEnglishCulture) {
                roles.AddRange(_roleTb.EntitiesIQueryable()
                                      .Where(r=>r.RoleIsactive == 1
                                        && r.RoleCustCode == _currentCustomer.CustCode)
                                      .Select(c=>
                                                            new SelectListItem
                                                            {
                                                                Text = c.RoleNameEn,
                                                                Value = c.RoleId.ToString(),
                                                            }
                                                        ).ToList());
            }else{
                roles.AddRange(_roleTb.EntitiesIQueryable()
                                      .Where(r=>r.RoleIsactive == 1
                                        && r.RoleCustCode == _currentCustomer.CustCode)  
                                      .Select(c=>
                                                            new SelectListItem
                                                            {
                                                                Text = c.RoleNameAr,
                                                                Value = c.RoleId.ToString(),
                                                            }
                                                        ).ToList());
            }
                                                    
            ViewBag.RoleSelectedList = roles;
        }

        private void DeleteAllRelatedCustomerData(string CustCode){

            _deviceTb.EntitiesIQueryable()
                    .Where(d=>d.DevicesCustCode == CustCode)
                    .ToList()
                    .ForEach(d=> _deviceTb.Remove(d.DevicesId));

            _groupTb.EntitiesIQueryable()
                    .Where(d=>d.GroupCustCode == CustCode)
                    .ToList()
                    .ForEach(g=> _groupTb.Remove(g.GroupId));
            
            _licenseTb.EntitiesIQueryable()
                        .Where(l=>l.LicCustCode == CustCode)
                        .ToList()
                        .ForEach(l=> _licenseTb.Remove(l.LicId));
            
            _materialTb.EntitiesIQueryable()
                        .Where(m=>m.MatCustCode == CustCode)
                        .ToList()
                        .ToList()
                        .ForEach(m=> _materialTb.Remove(m.MatId));
            
            _userTb.EntitiesIQueryable()
                    .Where(u=>u.UserCustCode == CustCode)
                    .ToList()
                        .ForEach(user=> _userTb.Remove(user.UserId));
            
            _roleTb.EntitiesIQueryable()
                    .Where(u=>u.RoleCustCode == CustCode)
                    .ToList()
                    .ForEach(r=> _roleTb.Remove(r.RoleId));

            _permissionTb.EntitiesIQueryable()
                            .Where(p=>p.PermCustCode == CustCode)
                            .ToList()
                    .ForEach(pe => _permissionTb.Remove(pe.PermId));
            
            _roleWithPermissionTb.EntitiesIQueryable()
                                .Where(rwp=> rwp.RwpCustCode == CustCode)
                                .ToList()
                    .ForEach(rw => _roleWithPermissionTb.Remove(rw.RwpId));

            _showTb.EntitiesIQueryable()
                    .Where(s=>s.ShowCustCode == CustCode)
                    .ToList()
                    .ForEach(sh=>_showTb.Remove(sh.ShowId));

            _showContentTb.EntitiesIQueryable() 
                            .Where(s=>s.ContentsCustCode == CustCode)
                            .ToList()
                    .ForEach(sc=>_showContentTb.Remove(sc.ContentsId));


            _showSettingTb.EntitiesIQueryable() 
                            .Where(s=>s.ShowSettingCustCode == CustCode)
                            .ToList()
                    .ForEach(sc=>_showSettingTb.Remove(sc.ShowSettingId));

            _showDetailTb.EntitiesIQueryable() 
                            .Where(s=>s.ShowDetailsCustCode == CustCode)
                            .ToList()
                    .ForEach(sc=>_showDetailTb.Remove(sc.ShowDetailsId));

            _materialTypeTb.EntitiesIQueryable()
                             .Where(s=>s.MtypeCustCode == CustCode)
                            .ToList()
                    .ForEach(sc=>_materialTypeTb.Remove(sc.MtypeId));

            _showTemplateTb.EntitiesIQueryable() 
                            .Where(s=>s.TempCustCode == CustCode)
                            .ToList()
                    .ForEach(sc=>_showTemplateTb.Remove(sc.TempId));

            _showHtmlTb.EntitiesIQueryable() 
                            .Where(s=>s.ShowCustCode == CustCode)
                            .ToList()
                    .ForEach(sc=>_showHtmlTb.Remove(sc.ShowId));

        }
    }
}