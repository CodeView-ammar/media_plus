
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
    [AuthorizeCustFilter]
    public class UserController : Controller
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
        private readonly ILogger<UserController> _logger;
        private readonly IWebHostEnvironment env;
        private readonly bool isEnglishCulture ;

#pragma warning disable CS8602 // Dereference of a possibly null reference.
        public UserController(ILogger<UserController> logger,
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
            ViewBag.isEnglishCulture = isEnglishCulture;

            IQueryable<UserViewModel> users = null;
            
            if(_currentCustomer.CustCode == "SuperAdmin"){
                users = _userTb.EntitiesIQueryable()
                                .Select(x =>
                                    new UserViewModel
                                    {  
                                    UserId = x.UserId,
                                    UserNameAr = isEnglishCulture? "null" :x.UserNameAr,
                                    UserNameEn = isEnglishCulture?  x.UserNameEn : "null",
                                    UserCdate = x.UserCdate,
                                    UserUdate = x.UserUdate,
                                    UserLoginName = x.UserLoginName,
                                    UserPhotoPath = x.UserPhoto,                                 
                                    CustomerName = isEnglishCulture? _customerTb.EntitiesIQueryable()
                                                                    .FirstOrDefault(c => c.CustCode == x.UserCustCode)
                                                                    .CustNameEn 
                                                        :_customerTb.EntitiesIQueryable()
                                                                    .FirstOrDefault(c => c.CustCode == x.UserCustCode)
                                                                    .CustNameAr,
                                                                    
                                    RoleName = isEnglishCulture? _roleTb.EntitiesIQueryable()
                                                                    .FirstOrDefault(r => r.RoleId == x.UserRoleId)
                                                                    .RoleNameEn 
                                                                :_roleTb.EntitiesIQueryable()
                                                                    .FirstOrDefault(r => r.RoleId == x.UserRoleId)
                                                                    .RoleNameAr,
                                    });

                    return View(users); 
            }

            users =_userTb.EntitiesIQueryable()
                            .Where(u=>u.UserCustCode == _currentCustomer.CustCode
                            && u.UserRoleId != _currentUser.UserRoleId)
                            .Select(x =>
                                    new UserViewModel
                                    {  
                                    UserId = x.UserId,
                                    UserNameAr = isEnglishCulture? "null" :x.UserNameAr,
                                    UserNameEn = isEnglishCulture?  x.UserNameEn : "null",
                                    UserCdate = x.UserCdate,
                                    UserUdate = x.UserUdate,
                                    UserLoginName = x.UserLoginName,
                                    UserPhotoPath = x.UserPhoto,
                                    UserPassword = SecurityHelper.GetMD5(x.UserPassword),
                                    CustomerName = isEnglishCulture? _customerTb.EntitiesIQueryable()
                                                                    .FirstOrDefault(c => c.CustCode == x.UserCustCode)
                                                                    .CustNameEn 
                                                        :_customerTb.EntitiesIQueryable()
                                                                    .FirstOrDefault(c => c.CustCode == x.UserCustCode)
                                                                    .CustNameAr,
                                                                    
                                    RoleName = isEnglishCulture? _roleTb.EntitiesIQueryable()
                                                                    .FirstOrDefault(r => r.RoleId == x.UserRoleId)
                                                                    .RoleNameEn 
                                                                :_roleTb.EntitiesIQueryable()
                                                                    .FirstOrDefault(r => r.RoleId == x.UserRoleId)
                                                                    .RoleNameAr,
                                    });

            
            return View(users);
        }


       //=====================================================================
        // User Creation Part 
        [HttpGet]
        public IActionResult Create()
        {
            ArrangeDropdownlistData();
            return View();
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
            
            if (_currentCustomer.CustCode == "SuperAdmin")
            {
                userRole = _roleTb.EntitiesIQueryable().FirstOrDefault(r => r.RoleNameEn == "administrator");

                if (userRole == null)
                {

                    userRole = new UserRole()
                    {
                        RoleNameAr = "مدير",
                        RoleNameEn = "administrator",
                        RoleCdate = DateTime.Now,
                        RoleUdate = DateTime.Now,
                        RoleIsactive = 1,
                        RoleCustCode = "SuperAdmin",
                        RoleByuserId = -1
                    };
                    _roleTb.Add(userRole);
                }
              

            }

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
                AddRelatedMaterialType(UserVM, newUser);
                AddRelatedUserPermissions(UserVM, newUser, userRole.RoleId);
            }

            return RedirectToAction("Index", "User");
        }




        //===================================================================== 
        // User Edit Part


        [HttpGet]
        public IActionResult Edit(int id)
        {

            ArrangeDropdownlistData();

            var user = _userTb.GetEntity(id);
            
            if (user == null)
            {
                return NotFound();
            }
            
            var userVm = new UserViewModel{
                UserId = user.UserId,
                UserNameAr = user.UserNameAr,
                UserNameEn = user.UserNameEn,
                UserCustCode = user.UserCustCode,
                UserRoleId = user.UserRoleId,
                UserCdate = user.UserCdate,
                UserUdate = user.UserUdate,
                UserPhotoPath = user.UserPhoto,
                UserLoginName = user.UserLoginName,
                UserPassword = user.UserPassword
            };

            return View(userVm);
        }

        [HttpPost]
        public IActionResult Edit(UserViewModel userVm){

            if (!ModelState.IsValid){
                ArrangeDropdownlistData();
                return View("Edit"); 
            }
			if (userVm.UserPassword != userVm.ConfirmPassword)
			{
				ModelState.AddModelError("ConfirmPassword", "كلمة المرور غير متطابقة");
				ArrangeDropdownlistData();
				return View("Edit", userVm);
			}
			var targetUser = _userTb.GetEntity(userVm.UserId);

            if(userVm.UserPhoto != null){

                //delete previos photo
                if (targetUser.UserPhoto != "default_photo_user.jpg,image/jpg"){
                    System.IO.File.Delete(Path.Combine(env.WebRootPath, "upload/user/photo", targetUser.UserPhoto.Split(",")[0]));
                }

                // Upload the valid photo to spacific folder
                string filename = Guid.NewGuid().ToString().Substring(0,18) + Path.GetExtension(userVm.UserPhoto.FileName);
                var path = Path.Combine(env.WebRootPath, "upload/user/photo", filename);
                using (var fileStream = System.IO.File.Create(path))
                {
                    userVm.UserPhoto.CopyTo(fileStream);
                }
                userVm.UserPhotoPath = filename+","+userVm.UserPhoto.ContentType;
            }

            targetUser.UserNameAr = userVm.UserNameAr;
            targetUser.UserNameEn = userVm.UserNameEn;
            targetUser.UserLoginName = userVm.UserLoginName;
            targetUser.UserPassword = userVm.UserPassword;
            targetUser.UserUdate = DateTime.Now;
            targetUser.UserCdate = userVm.UserCdate.Value;
            targetUser.UserCustCode = userVm.UserCustCode;
            targetUser.UserRoleId = userVm.UserRoleId;
            targetUser.UserPhoto = userVm.UserPhotoPath;
               
            _userTb.Update(targetUser);

            return RedirectToAction("Index", "User");
        }

       //======================================================================
       // User Delete Part
       [HttpGet]
       public IActionResult Delete(int id)
       {
            //delete attached photo
            var selectedFile = _userTb.GetEntity(id);

            if (selectedFile.UserPhoto != "default_photo_user.jpg,image/jpeg")
            {
                var path = Path.Combine(env.WebRootPath, "upload/user/photo", selectedFile.UserPhoto.Split(",")[0]);
                System.IO.File.Delete(path);
            }

            // delete material type
            var matType = _materialTb.EntitiesIQueryable().Where(m=>m.MtypeUserId == selectedFile.UserId).ToList();
            
            foreach(var item in matType){
                _materialTb.Remove(item.MtypeId);
            }

            var userPermission = _userPermissionTb.EntitiesIQueryable().Where(m=>m.PermByUserid == selectedFile.UserId).ToList();
            
            foreach(var item in userPermission){
                _userPermissionTb.Remove(item.PermId);
            }

            var userRoles = _roleTb.EntitiesIQueryable().Where(r=>r.RoleByuserId == selectedFile.UserId).ToList();
            
            foreach(var item in userRoles){
                _roleTb.Remove(item.RoleId);
            }

            var RoleWithPermissions = _roleWithPermissionTb.EntitiesIQueryable().Where(r=>r.RwpByUserid == selectedFile.UserId).ToList();

            foreach(var item in RoleWithPermissions){
                _roleWithPermissionTb.Remove(item.RwpId);
            }

            var roles = _roleTb.EntitiesIQueryable().Where(r=>r.RoleByuserId == selectedFile.UserId).ToList();

            foreach(var item in roles){
                _roleWithPermissionTb.Remove(item.RoleId);
            }
            
            _userTb.Remove(id);
            
            return RedirectToAction("Index", "User");
       }


        // User Download Photo Part
        [HttpGet]
        public IActionResult Download(int Id)
        {
            var selectedFile = _userTb.GetEntity(Id);

            if (selectedFile == null)
            {
                return NotFound();
            }

            var path = "~/upload/user/photo/" + selectedFile.UserPhoto.Split(",")[0];
            var ContentType = selectedFile.UserPhoto.Split(",")[1];
            var fileName = selectedFile.UserPhoto.Split(",")[0];

            Response.Headers.Add("Content-Disposition", "attachment; filename=" + fileName);
            Response.Headers.Add("Expires", DateTime.UtcNow.AddDays(-3).ToString("R"));
            Response.Headers.Add("Cache-Control", "no-cache");
            return File(path, ContentType, fileName);
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


        private void AddRelatedMaterialType(UserViewModel UserVM, User newUser)
        {
            var matTypeEn = new string[] { "photo", "video", "link", "description" };
            var matTypeAr = new string[] { "صورة", "فيديو", "رابط", "وصف" };
            var staticHTML = new string[] {
                """<td style="width: pwidth%; height: pheight%;"><img src="placeholder"></td>""",
                """<td style="width: pwidth%; height: pheight%;"><iframe src="placeholder" title="YouTube video player" frameborder="0" allow="accelerometer; autoplay; clipboard-write; encrypted-media; gyroscope; picture-in-picture; web-share" allowfullscreen></iframe></td>""",
                """<td style="width: pwidth%; height: pheight%;"><a href="placeholder">Link</a></td>""",
                """<td style="width: pwidth%; height: pheight%;"><h2>placeholder</h2></td>"""
            };

            for (var i = 0; i < matTypeEn.Length; i++)
            {
                _materialTb.Add(new MaterialType()
                {
                    MtypeNameAr = matTypeAr[i],
                    MtypeNameEn = matTypeEn[i],
                    MtypeStaticHtml = staticHTML[i],
                    MtypeCustCode = UserVM.UserCustCode,
                    MtypeCdate = DateTime.Now,
                    MtypeUdate = DateTime.Now,
                    MtypeUserId = newUser.UserId,
                    MtypeIsactive = 1,
                });
            }
        }
    }
}
#pragma warning restore CS8602 // Dereference of a possibly null reference.