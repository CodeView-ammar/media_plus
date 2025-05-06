using MediaPlus.DBModels.Repository;
using MediaPlus.DBModels;
using Microsoft.AspNetCore.Mvc;
using MediaPlus.Models.SecurityHelper;
using MediaPlus.Models.ViewModels;
using Microsoft.EntityFrameworkCore;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MediaPlus.Controllers.ApiController
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
  
        private readonly UnitOfWork _unitOfWork = new();
         private readonly IRepository<User> _userTb;
        private readonly IRepository<Customer> _customerTb;
        private readonly IRepository<License> _licenseTb;
        private readonly IRepository<AdDevice> _adDeviceTb;
        public AuthController()
        {
             _userTb = _unitOfWork.GetRepositoryInstance<User>();
            _customerTb = _unitOfWork.GetRepositoryInstance<Customer>();
            _licenseTb = _unitOfWork.GetRepositoryInstance<License>();
            _adDeviceTb = _unitOfWork.GetRepositoryInstance<AdDevice>();
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] ApiLoginViewModel loginVM)
        {
            // البحث عن العميل باستخدام كود العميل والتوكن
            var customer = _customerTb.EntitiesIQueryable().AsNoTracking()
                .FirstOrDefault(c => c.CustCode == loginVM.CustomerCode );

            if (customer == null)
            {
                return Unauthorized("كود العميل أو التوكن غير صحيح");
            }

            // البحث عن الجهاز باستخدام اسم الجهاز وكود العميل
            var device = _adDeviceTb.EntitiesIQueryable().AsNoTracking()
                .FirstOrDefault(d => d.DevicesCustCode == loginVM.CustomerCode
                && d.DevicesName == loginVM.DeviceCode);

            if (device == null)
            {
                return Unauthorized("كود الجهاز غير صحيح");
            }

            device = _adDeviceTb.EntitiesIQueryable().AsNoTracking()
            .FirstOrDefault(d => d.DevicesCustCode == loginVM.CustomerCode
            && d.DevicesName == loginVM.DeviceCode && d.DevicesOnoff == 1);

            if (device == null)
            {
                return Unauthorized("الجهاز مغلق من لوحة التحكم");
            }

            
            // التحقق من أن العميل يمتلك ترخيص ساري
            var validLicense = _licenseTb.EntitiesIQueryable()
                .FirstOrDefault(l => l.LicCustCode == loginVM.CustomerCode && l.LicIsactive == 1
                                     && l.LicStartAt < DateTime.Now && DateTime.Now < l.LicEndAt);

            if (validLicense == null)
            {
                return BadRequest("الترخيص غير صالح أو منتهي");
            }

            var result = new
            {
                Customer = customer,
                Device = device
            };

            return Ok(result);
        }


    }
}
