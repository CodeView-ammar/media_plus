
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
    public class UserRoleController : Controller
    {
       
        private readonly UnitOfWork _unitOfWork = new();
        private readonly IRepository<Customer> _customerTb;
        private readonly IRepository<User> _userTb;
        private readonly IRepository<UserRole> _roleTb;
        private readonly IRepository<UserPermission> _userPermissionTb;
        private readonly IRepository<RoleWithPermission> _rolesWithPermission;
        private readonly IHttpContextAccessor _accessor;
        private readonly CustomerSessionModel? _currentCustomer;
        private readonly ILogger<UserRoleController> _logger;
        private readonly IWebHostEnvironment env;
        private readonly Boolean isEnglishCulture ;

        public UserRoleController(ILogger<UserRoleController> logger,
                                  IWebHostEnvironment _env,
                                  IHttpContextAccessor accessor)
        {
            _customerTb = _unitOfWork.GetRepositoryInstance<Customer>();
            _userTb = _unitOfWork.GetRepositoryInstance<User>();
            _roleTb = _unitOfWork.GetRepositoryInstance<UserRole>();
            _userPermissionTb = _unitOfWork.GetRepositoryInstance<UserPermission>();
            _rolesWithPermission = _unitOfWork.GetRepositoryInstance<RoleWithPermission>();
            _logger = logger;
            _accessor = accessor;
            _currentCustomer = _accessor.HttpContext.Session.GetObject<CustomerSessionModel>("CustomerObject");
            isEnglishCulture = _accessor.HttpContext.Request.Cookies[".AspNetCore.Culture"] == "c=en-US|uic=en-US";
            
            env = _env;
        }

        [HttpGet]
        public IActionResult Index()
        {
            ViewBag.isEnglishCulture = isEnglishCulture;

            
            var roles =_roleTb.EntitiesIQueryable().Where(u=>u.RoleCustCode == _currentCustomer.CustCode)
                                .Select(x =>
                        new RoleViewModel
                        {
                            RoleId = x.RoleId,
                            RoleNameAr = isEnglishCulture? "null" : x.RoleNameAr,
                            RoleNameEn = isEnglishCulture? x.RoleNameEn : "null",
                            RoleCdate = x.RoleCdate,
                            RoleUdate = x.RoleUdate,
                            RoleIsactive = x.RoleIsactive == 1 ,
                            UserName = isEnglishCulture? _userTb.EntitiesIQueryable()
                                                                .FirstOrDefault(u => u.UserId == x.RoleByuserId)
                                                                .UserNameEn 
                                                        : _userTb.EntitiesIQueryable()
                                                                .FirstOrDefault(u => u.UserId == x.RoleByuserId)
                                                                .UserNameAr,


                            CustomerName = isEnglishCulture? _customerTb.EntitiesIQueryable()
                                                                        .FirstOrDefault(c => c.CustCode == x.RoleCustCode)
                                                                        .CustNameEn 
                                                            :_customerTb.EntitiesIQueryable()
                                                                        .FirstOrDefault(c => c.CustCode == x.RoleCustCode)
                                                                        .CustNameAr,
                        });

            return View(roles);
        }


       //=====================================================================
        // Role Creation Part 
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }



        [HttpPost]
        public IActionResult Create(RoleViewModel RoleVM)
        {
        

            if (!ModelState.IsValid)
            {
                return View(RoleVM); // Ì⁄Ìœ «·‰„Ê–Ã „⁄ —”«∆· «·Œÿ√
            }

            var InsertedUserRole  = new UserRole()
            {
                RoleNameAr = RoleVM.RoleNameAr,
                RoleNameEn = RoleVM.RoleNameEn,
                RoleByuserId = _accessor.HttpContext.Session.GetObject<User>("UserObject").UserId, // Get from session
                RoleCustCode = _accessor.HttpContext.Session.GetObject<Customer>("CustomerObject").CustCode, // Get from session
                RoleIsactive = 1,
                RoleCdate = DateTime.Now,
                RoleUdate = DateTime.Now,
            };

            _roleTb.Add(InsertedUserRole);

            return RedirectToAction("Index", "UserRole");

        }


        //===================================================================== 
        // UserRole Edit Part
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var Role = _roleTb.GetEntity(id);
            
            if (Role == null)
            {
                return NotFound();
            }
            
            var roleVm = new RoleViewModel{
                    RoleId = Role.RoleId,
                    RoleNameAr = Role.RoleNameAr,
                    RoleNameEn = Role.RoleNameEn == "administrator" ? "admin" : Role.RoleNameEn,
                    RoleCustCode = Role.RoleCustCode,
                    RoleByuserId = Role.RoleByuserId,
                    RoleIsactive = Role.RoleIsactive == 1 ,
                    RoleCdate = Role.RoleCdate,
                    RoleUdate = Role.RoleUdate,
            };

            return View(roleVm);
        }

        [HttpPost]
        public IActionResult Edit(RoleViewModel roleVM){

            if (!ModelState.IsValid){
                return View("Edit"); 
            }

            var role = new UserRole()
                            {
                                RoleId = roleVM.RoleId.Value,
                                RoleNameAr = roleVM.RoleNameAr,
                                RoleNameEn =  roleVM.RoleNameEn == "administrator" ? "admin" : roleVM.RoleNameEn,
                                RoleCustCode = roleVM.RoleCustCode,
                                RoleIsactive = roleVM.RoleIsactive == true ? 1 : 0,
                                RoleByuserId = roleVM.RoleByuserId,
                                RoleCdate = roleVM.RoleCdate.Value,
                                RoleUdate = DateTime.Now,    
                            };
               
            _roleTb.Update(role);

            return RedirectToAction("Index", "UserRole");
        }

       //======================================================================
       // Role Delete Part
       [HttpGet]
       public IActionResult Delete(int id)
       {
            _roleTb.Remove(id);
            
            return RedirectToAction("Index", "UserRole");
       }
        //======================================================================
        [HttpGet]
        public IActionResult RolesPermission()
        {
            var roles = _roleTb.EntitiesIQueryable()
                                               .Where(r=>
                                               r.RoleCustCode == _currentCustomer.CustCode
                                               &&
                                               r.RoleIsactive == 1)
                                               .ToList();

            return View(roles);
        }

        [HttpGet]
        public IActionResult EditRolePermission(int id) // Role ID
        {
            var targetedPermission = _userPermissionTb.EntitiesIQueryable().
                Where(p=>p.PermCustCode == _currentCustomer.CustCode);
            Console.WriteLine($"*************{targetedPermission.Count()}****************************");
            var RoleWithPermission = _rolesWithPermission.EntitiesIQueryable().
                Where(p=>p.RwpCustCode == _currentCustomer.CustCode);
            Console.WriteLine($"*************{RoleWithPermission.Count()}****************************");

            var RolePermissionVM  = new RolePermissionViewModel(){
                RoleId = id,
                RoleName = isEnglishCulture ? _roleTb.GetEntity(id).RoleNameEn 
                                            : _roleTb.GetEntity(id).RoleNameAr,
                Permissions = targetedPermission.Select(x=>new UserPermissionViewModel
                {
                    PermId = x.PermId,
                    PermNameAr =  x.PermName,
                    PermNameEn =  x.PermName , 
                    PermIsactive = RoleWithPermission.FirstOrDefault(r=>r.RwpPermissionId == x.PermId && r.RwpRoleId == id) != null
                }).ToList()
            };
            

            return View(RolePermissionVM);
        }

        [HttpPost]
        public IActionResult EditRolePermission(RolePermissionViewModel RolePremVM)
        {
            RolePremVM.Permissions.ForEach(
                p => {
                    if(p.PermIsactive){
                        _rolesWithPermission.Add(new RoleWithPermission
                        {
                            RwpRoleId = RolePremVM.RoleId,
                            RwpPermissionId = p.PermId,
                            RwpByUserid = 4,
                            RwpCustCode = _currentCustomer.CustCode,
                            RwpCdate = DateTime.Now,
                            RwpUdate = DateTime.Now,
                        });
                    }else{
                        if(_rolesWithPermission.EntitiesIQueryable()
                                               .Any(r=>r.RwpPermissionId == p.PermId && r.RwpRoleId == RolePremVM.RoleId)){

                            _rolesWithPermission.Remove(
                                _rolesWithPermission.EntitiesIQueryable()
                                                    .FirstOrDefault(r=>r.RwpPermissionId == p.PermId && r.RwpRoleId == RolePremVM.RoleId).RwpId
                            );
                        }   
                    }
                }
            );
            
            return RedirectToAction("RolesPermission", "UserRole");
        }

        //======================================================================
       // Toggle active Part
       [HttpGet]
       public IActionResult ToggleState(int Id)
       {    
            var targetElement = _roleTb.GetEntity(Id);

            targetElement.RoleIsactive = targetElement.RoleIsactive == 1 ? 0 : 1;

            _roleTb.Update(targetElement);

            return Ok("success");
       }

    }
}