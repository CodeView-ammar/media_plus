
using MediaPlus.DBModels;
using MediaPlus.DBModels.Repository;
using MediaPlus.Models.ViewModels;
using MediaPlus.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MediaPlus.Models.SecurityHelper;
using MediaPlus.Models.CustomFilters;
using System.Text.Json;

namespace MediaPlus.Controllers
{
    public class SingupController : Controller
    {
       
        private readonly UnitOfWork _unitOfWork = new();
        private readonly IRepository<Customer> _customerTb;
        private readonly IRepository<UserRole> _roleTb;
        private readonly IRepository<User> _userTb;
        private readonly IRepository<MaterialType> _materialTb;
        private readonly IRepository<UserPermission> _userPermissionTb;
        private readonly IRepository<RoleWithPermission> _roleWithPermissionTb;
        private readonly IHttpContextAccessor _accessor;
        private readonly CustomerSessionModel? _currentCustomer;
        private readonly UserSessionModel? _currentUser;
        private readonly ILogger<SingupController> _logger;
        private readonly IWebHostEnvironment env;
        private readonly bool isEnglishCulture ;

#pragma warning disable CS8602 // Dereference of a possibly null reference.
        public SingupController(ILogger<SingupController> logger,
                                  IWebHostEnvironment _env,
                                  IHttpContextAccessor accessor)
        {
            _customerTb = _unitOfWork.GetRepositoryInstance<Customer>();
            _userTb = _unitOfWork.GetRepositoryInstance<User>();
            _roleTb = _unitOfWork.GetRepositoryInstance<UserRole>();
            _materialTb = _unitOfWork.GetRepositoryInstance<MaterialType>();
            _userPermissionTb = _unitOfWork.GetRepositoryInstance<UserPermission>();
            _roleWithPermissionTb = _unitOfWork.GetRepositoryInstance<RoleWithPermission>();
            _logger = logger;
            _accessor = accessor;
            _currentCustomer = _accessor.HttpContext.Session.GetObject<CustomerSessionModel>("CustomerObject");
            _currentUser = _accessor.HttpContext.Session.GetObject<UserSessionModel>("UserObject");
            isEnglishCulture = _accessor.HttpContext.Request.Cookies[".AspNetCore.Culture"] == "c=en-US|uic=en-US";
            env = _env;
        }

        [HttpGet]
        public IActionResult Index()
        {
          
            return View("~/Views/Account/Singup.cshtml"); 
        }


        [HttpPost]
        public async Task<IActionResult> CreateAsync(UserViewModel UserVM)
        {

            if (!ModelState.IsValid)
            {
                ArrangeDropdownlistData();
                return View("Create");

            }
			if (UserVM.UserPassword != UserVM.ConfirmPassword)
			{
				ModelState.AddModelError("ConfirmPassword", "كلمة المرور غير متطابقة");
				ArrangeDropdownlistData();
				return View("Create", UserVM);
			}

			var staticContentType = new string[]{

            };


            string filename = "";
            // Upload the valid photo to spacific folder
            if (UserVM.UserPhoto != null)
            {
                filename = Guid.NewGuid().ToString().Substring(0, 18) + Path.GetExtension(UserVM.UserPhoto.FileName);
                var path = Path.Combine(env.WebRootPath, "upload/user/photo", filename);
                using (var fileStream = System.IO.File.Create(path))
                {
                    await UserVM.UserPhoto.CopyToAsync(fileStream);
                }

            }

            UserRole? userRole = null;
            

            var newUser = new User()
            {
                UserNameAr = UserVM.UserNameAr,
                UserNameEn = UserVM.UserNameEn,
                UserLoginName = UserVM.UserLoginName,
                UserPassword = SecurityHelper.GetMD5(UserVM.UserPassword), // Will hash it to save in database 
                UserCustCode = _currentCustomer.CustCode == "SuperAdmin" ? UserVM.UserCustCode : _currentCustomer.CustCode, // Get from session
                UserRoleId = _currentCustomer.CustCode == "SuperAdmin" ? userRole.RoleId : UserVM.UserRoleId,
                UserPhoto = UserVM.UserPhoto == null ? "default_photo_user.jpg,image/jpeg" : filename + "," + UserVM.UserPhoto?.ContentType,
                UserCdate = DateTime.Now,
                UserUdate = DateTime.Now,
            };
            _userTb.Add(newUser);

            if(UserVM.UserCustCode != null && _customerTb.EntitiesIQueryable().Any(c => c.CustCode == UserVM.UserCustCode))
            {
                AddRelatedUserPermissions(UserVM, newUser, userRole.RoleId);
            }

            return RedirectToAction("Index", "User");
        }





        private void ArrangeDropdownlistData()
        {
            var roles = new List<SelectListItem>();
            var customers = new List<SelectListItem>();

            if (isEnglishCulture) {
                roles.AddRange(_roleTb.GetAllEntities()
                                 .Where(c=>c.RoleIsactive == 1
                                 && c.RoleCustCode == _currentCustomer.CustCode).Select(c=>new SelectListItem
                                                                {Text = c.RoleNameEn,
                                                                Value = c.RoleId.ToString()}).ToList());

                customers.AddRange(_customerTb.EntitiesIQueryable()
                                            .Where(c=>c.CustIsactive == 1)
                                            .Select(x=>new SelectListItem(){
                                                Text= x.CustNameEn.ToString(),
                                                Value = x.CustCode.ToString()
                                            }));
            }else{
                roles.AddRange(_roleTb.GetAllEntities()
                                        .Where(c=>c.RoleIsactive == 1
                                        && c.RoleCustCode == _currentCustomer.CustCode)
                                        .Select(c=>new SelectListItem
                                                                {Text = c.RoleNameAr,
                                                                Value = c.RoleId.ToString()}).ToList());
            
                customers.AddRange(_customerTb.EntitiesIQueryable()
                                            .Where(c=>c.CustIsactive == 1)
                                            .Select(x=>new SelectListItem(){
                                                Text= x.CustNameAr.ToString(),
                                                Value = x.CustCode.ToString()
                                            }));
            }

            ViewBag.CustomerSelectedList = customers;
            ViewBag.RoleSelectedList = roles;                                         
        }
        
        private void AddRelatedUserPermissions(UserViewModel UserVM, User newUser, int roleId)
        {
            var userPermissions = new string[] {
                "device.read","device.create","device.update","device.delete"
                ,"group.read","group.create","group.update","group.delete"
                ,"show.read","show.create","show.update","show.delete"
                ,"showtemplate.read","showtemplate.create","showtemplate.update","showtemplate.delete"
                ,"material.read","material.create","material.update","material.delete"
                };

            if (!_userTb.EntitiesIQueryable().Any(u => u.UserCustCode == _currentCustomer.CustCode))
            {
                foreach (var permission in userPermissions)
                {
                    var newPerm = new UserPermission(){
                        PermName = permission,
                        PermCustCode = UserVM.UserCustCode,
                        PermByUserid = newUser.UserId,
                        PermCdate = DateTime.Now,
                        PermUdate = DateTime.Now,
                        PermIsactive = 1
                    };
                    
                    _userPermissionTb.Add(newPerm);

                    _roleWithPermissionTb.Add(new RoleWithPermission()
                    {
                        RwpRoleId = roleId,
                        RwpPermissionId = newPerm.PermId,
                        RwpCdate = DateTime.Now,
                        RwpUdate = DateTime.Now,
                        RwpByUserid = newUser.UserId,
                        RwpCustCode = UserVM.UserCustCode,
                    });
                }
            }
        }


      }
}
#pragma warning restore CS8602 // Dereference of a possibly null reference.