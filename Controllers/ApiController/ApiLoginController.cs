using MediaPlus.DBModels;
using MediaPlus.DBModels.Repository;
using MediaPlus.Models.SecurityHelper;
using MediaPlus.Models.ViewModels;
using MediaPlus.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MediaPlus.Controllers.ApiController
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApiLoginController : ControllerBase
    {
        private readonly MediaPlusDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly UnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _accessor;
        private readonly IRepository<User> _userTb;
        private readonly IRepository<Customer> _customerTb;
        private readonly IRepository<License> _licenseTb;
        private readonly IRepository<UserRole> _userRoleTb;
        private readonly IRepository<RoleWithPermission> _roleWithPermission;
        private readonly IRepository<UserPermission> _userPermission;

        public ApiLoginController(
            MediaPlusDbContext context,
            IConfiguration configuration,
            IHttpContextAccessor accessor)
        {
            _context = context;
            _configuration = configuration;
            _accessor = accessor;
            _unitOfWork = new UnitOfWork();
            _userTb = _unitOfWork.GetRepositoryInstance<User>();
            _userPermission = _unitOfWork.GetRepositoryInstance<UserPermission>();
            _userRoleTb = _unitOfWork.GetRepositoryInstance<UserRole>();
            _roleWithPermission = _unitOfWork.GetRepositoryInstance<RoleWithPermission>();
            _customerTb = _unitOfWork.GetRepositoryInstance<Customer>();
            _licenseTb = _unitOfWork.GetRepositoryInstance<License>();
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginViewModel loginVM)
        {
            if (loginVM == null || !ModelState.IsValid)
                return BadRequest("بيانات غير صالحة");

            // التحقق من حساب الادمن الثابت
            if (SecurityHelper.GetMD5(loginVM.Username) == "21232f297a57a5a743894a0e4a801fc3"
                && SecurityHelper.GetMD5(loginVM.Password) == "21232f297a57a5a743894a0e4a801fc3")
            {
                var adminUser = new UserSessionModel
                {
                    UserId = 1,
                    UserNameAr = "مدير النظام",
                    UserNameEn = "Admin",
                    UserLoginName = "Administrator",
                    UserCustCode = "SuperAdmin",
                    UserPermissions = new List<string>()
                };

                var adminCustomer = new CustomerSessionModel
                {
                    CustCode = "SuperAdmin",
                    CustNameAr = "المدير",
                    CustNameEn = "Admin"
                };

                return Ok(new
                {
                    token = "", // يمكنك إضافة التوكن هنا
                    User = adminUser,
                    Customer = adminCustomer,
                    
                    userid = adminUser.UserId,
                    username = adminUser.UserLoginName,
                    device = loginVM.CompanyCode
                });
            }

            var user = await _userTb.EntitiesIQueryable().FirstOrDefaultAsync(u =>
                u.UserLoginName == loginVM.Username &&
                u.UserPassword == SecurityHelper.GetMD5(loginVM.Password) &&
                u.UserCustCode == loginVM.CompanyCode);

            if (user == null)
                return Unauthorized("اسم المستخدم أو كلمة المرور غير صحيحة");

            // التحقق من الترخيص
            var validLicense = await _licenseTb.EntitiesIQueryable()
                .FirstOrDefaultAsync(l => l.LicCustCode == loginVM.CompanyCode && l.LicIsactive == 1);

            if (validLicense == null || validLicense.LicStartAt > DateTime.Now || validLicense.LicEndAt < DateTime.Now)
                return BadRequest("الترخيص منتهي أو غير صالح، يرجى التواصل مع الدعم");

            // بناء بيانات الجلسة
            var userSession = new UserSessionModel
            {
                UserId = user.UserId,
                UserNameAr = user.UserNameAr,
                UserNameEn = user.UserNameEn,
                UserLoginName = user.UserLoginName,
                UserCustCode = user.UserCustCode,
                UserPhoto = user.UserPhoto,
                UserRoleId = user.UserRoleId,
                RoleName = (await _userRoleTb.EntitiesIQueryable().FirstOrDefaultAsync(r => r.RoleId == user.UserRoleId))?.RoleNameEn,
                UserPermissions = await _userPermission.EntitiesIQueryable()
                    .Where(p => p.PermCustCode == loginVM.CompanyCode &&
                                _roleWithPermission.EntitiesIQueryable()
                                    .Any(r => r.RwpRoleId == user.UserRoleId &&
                                              r.RwpPermissionId == p.PermId &&
                                              r.RwpCustCode == loginVM.CompanyCode))
                    .Select(p => p.PermName)
                    .ToListAsync()
            };

            var customer = await _customerTb.EntitiesIQueryable()
                .Where(c => c.CustCode == loginVM.CompanyCode)
                .Select(c => new CustomerSessionModel
                {
                    CustId = c.CustId,
                    CustCode = c.CustCode,
                    CustNameAr = c.CustNameAr,
                    CustNameEn = c.CustNameEn
                })
                .FirstOrDefaultAsync();

            return Ok(new
            {
                User = userSession,
                Customer = customer
            });
        }

        private string GenerateJwtToken(User user)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, user.UserLoginName),
                new Claim("UserId", user.UserId.ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}