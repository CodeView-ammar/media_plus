using MediaPlus.DBModels;
using MediaPlus.DBModels.Repository;
using MediaPlus.Models.SecurityHelper;
using MediaPlus.Models.ViewModels;
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

        public ApiLoginController(MediaPlusDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }
        public static string EncryptToMD5(string input)
        {
            using (var md5 = System.Security.Cryptography.MD5.Create())
            {
                var inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
                var hashBytes = md5.ComputeHash(inputBytes);
                return Convert.ToHexString(hashBytes); // .NET 5+ 
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] aLoginViewModel model)
        {
            if (string.IsNullOrEmpty(model.Username) || string.IsNullOrEmpty(model.Password) || string.IsNullOrEmpty(model.CustomerCode))
                return BadRequest("البيانات غير مكتملة");

            // تشفير الباسورد
            string encryptedPassword = SecurityHelper.GetMD5(model.Password);

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.UserLoginName == model.Username && u.UserPassword == encryptedPassword);

            if (user == null)
                return Unauthorized("اسم المستخدم أو كلمة المرور غير صحيحة");

            // التحقق من اسم الجهاز (كود العميل)
            if (!string.IsNullOrEmpty(user.UserCustCode) && user.UserCustCode != model.CustomerCode)
                return Unauthorized("كود العميل غير معتمد");

            // إنشاء التوكن
            var token = GenerateJwtToken(user);

            return Ok(new
            {
                token,
                username = user.UserLoginName,
                device = model.CustomerCode
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
