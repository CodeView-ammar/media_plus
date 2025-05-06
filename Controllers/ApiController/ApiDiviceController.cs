using MediaPlus.DBModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MediaPlus.Controllers.ApiController
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApiDiviceController : ControllerBase
    {
        private readonly MediaPlusDbContext _context;

        public ApiDiviceController(MediaPlusDbContext context)
        {
            _context = context;
        }

        // عرض جميع الأجهزة
        [HttpGet("show/{customerCode}")]
        public async Task<ActionResult<IEnumerable<AdDevice>>> GetDevices(string customerCode)
        {
            var devices = await _context.AdDevices
                .Where(d => d.DevicesCustCode == customerCode)
                .ToListAsync();

            return devices;
        }

        // عرض جهاز محدد
        [HttpGet("show/{customerCode}/{id}")]
        public async Task<ActionResult<AdDevice>> GetDevice(string customerCode, int id)
        {
            var device = await _context.AdDevices
                .Where(d => d.DevicesCustCode == customerCode && d.DevicesId == id)
                .FirstOrDefaultAsync();

            if (device == null)
            {
                return NotFound();
            }

            return device;
        }
        private string GetContentType(string fileName)
        {
            var extension = Path.GetExtension(fileName).ToLowerInvariant();
            switch (extension)
            {
                case ".jpg":
                case ".jpeg":
                    return "image/jpeg";
                case ".png":
                    return "image/png";
                case ".gif":
                    return "image/gif";
                case ".mp4":
                    return "video/mp4";
                case ".avi":
                    return "video/x-msvideo";
                case ".mov":
                    return "video/quicktime";
                default:
                    return "application/octet-stream"; // Default type
            }
        }

        private async Task<string> SaveAttachmentAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("لم يتم إرسال الملف");

            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}"; // Use UUID for unique file names
            var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "upload/device/photo");

            if (!Directory.Exists(uploadPath))
                Directory.CreateDirectory(uploadPath);

            var filePath = Path.Combine(uploadPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            string contentType = GetContentType(file.FileName);
            return $"{fileName},{contentType}"; // Return filename and content type
        }

        // Adding a new device
        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<AdDevice>> PostDevice([FromForm] AdDevice device, IFormFile devicesOfflinePhoto)
        {
            if (devicesOfflinePhoto != null)
            {
                // Use the SaveAttachmentAsync method to save the image


                // حفظ فقط اسم الملف
                //device.DevicesOfflinePhoto = parts[0];

            }
            var result = await SaveAttachmentAsync(devicesOfflinePhoto);
            var parts = result;
            var _AdDevice = new AdDevice
            {
                DevicesName = device.DevicesName,
                DevicesCdate = device.DevicesCdate,
                DevicesGroupid = device.DevicesGroupid,
                DevicesUdate = DateTime.Now, // Set the update date if needed
                DevicesCustCode = device.DevicesCustCode,
                DeviceIsInterrupt = device.DeviceIsInterrupt,
                DevicesOnoff = device.DevicesOnoff,
                DevicesOfflinePhoto = result,
                DevicesByUserid = device.DevicesByUserid
            };
            await _context.AdDevices.AddAsync(_AdDevice);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetDevice", new { id = device.DevicesId }, device);
        }


        [HttpPut]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<AdDevice>> PutDevice([FromForm] AdDevice device, IFormFile devicesOfflinePhoto)
        {
            if (device.DevicesId<0)
            {
                return BadRequest("Device ID mismatch.");
            }

            var existingDevice = await _context.AdDevices.FindAsync(device.DevicesId);
            if (existingDevice == null)
            {
                return NotFound("Device not found.");
            }

            // تحديث خصائص الجهاز
            existingDevice.DevicesName = device.DevicesName;
            existingDevice.DevicesGroupid = device.DevicesGroupid;
            existingDevice.DevicesCustCode = device.DevicesCustCode;
            existingDevice.DeviceIsInterrupt = device.DeviceIsInterrupt;
            existingDevice.DevicesOnoff = device.DevicesOnoff;
            existingDevice.DevicesUdate = DateTime.Now; // تحديث تاريخ التعديل

            if (devicesOfflinePhoto != null)
            {
                // حفظ الصورة الجديدة
                var result = await SaveAttachmentAsync(devicesOfflinePhoto);
                existingDevice.DevicesOfflinePhoto = result; // حفظ اسم الملف
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return NotFound("Error updating the device.");
            }
            catch (Exception ex)
            {
                // يمكنك تسجيل الاستثناء هنا إذا لزم الأمر
                return StatusCode(500, "Internal server error.");
            }

            return Ok(existingDevice); // إرجاع الجهاز المحدث
        }
        // حذف جهاز
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDevice(int id)
        {
            var device = await _context.AdDevices.FindAsync(id);
            if (device == null)
            {
                return NotFound(new { message = "الجهاز غير موجود" });
            }

            _context.AdDevices.Remove(device);
            await _context.SaveChangesAsync();

            return Ok(new { message = "تم حذف الجهاز بنجاح" });
        }
        private bool DeviceExists(int id)
        {
            return _context.AdDevices.Any(e => e.DevicesId == id);
        }
    }
}