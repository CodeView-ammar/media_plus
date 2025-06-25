using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http; // تأكد من إضافة هذه المكتبة
using Microsoft.AspNetCore.Mvc;
using MediaPlus.DBModels;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MediaPlus.Models.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace MediaPlus.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MediaController : ControllerBase
    {
        private readonly MediaPlusDbContext _context;

        public MediaController(MediaPlusDbContext context)
        {
            _context = context;
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
                    return "application/octet-stream"; // النوع الافتراضي
            }
        }

        private async Task<string> SaveAttachmentAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("لم يتم إرسال الملف");

            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}"; // استبدال بصيغة UUID
            var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "upload/show_material");

            if (!Directory.Exists(uploadPath))
                Directory.CreateDirectory(uploadPath);

            var filePath = Path.Combine(uploadPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            string contentType = GetContentType(file.FileName);
            return $"{fileName},{contentType}"; // الصيغة المطلوبة
        }

        [HttpPost("add")]
        [RequestSizeLimit(52428800)] // 50 MB مثلاً
        public async Task<IActionResult> AddMaterial([FromForm] MediaApiModel materialDto)
        {
            // تحقق من صحة النموذج
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // حفظ المرفق
            var fileName = await SaveAttachmentAsync(materialDto.MatFile);

            // تخزين المعلومات في قاعدة البيانات
            var showMaterial = new ShowMaterial
            {
                MatShowNameAr = materialDto.MatShowNameAr,
                MatShowNameEn = materialDto.MatShowNameEn,
                MatPath = fileName, // مسار الملف النسبي
                MatTypeId = materialDto.MatTypeId,
                MatCdate = DateTime.UtcNow,
                MatByuserId = 1, // أو من التوكن
                MatCustCode = materialDto.MatCustCode,
                MatIsactive = 1
            };

            await _context.ShowMaterials.AddAsync(showMaterial);
            await _context.SaveChangesAsync();

            return Ok(new { message = "تم الحفظ", id = showMaterial.MatId });
        }

        [HttpGet("show/{custCode}")]
        public async Task<IActionResult> GetMaterials(string custCode)
        {

            if (string.IsNullOrEmpty(custCode))
            {
                return BadRequest("custCode is required");
            }

            // استرجاع البيانات من قاعدة البيانات
            var materials = await _context.ShowMaterials.Where(u=>u.MatCustCode==custCode).ToListAsync();

            if (materials == null || !materials.Any())
            {
                return NotFound("لا توجد بيانات للعرض");
            }

            return Ok(materials);
        }
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteMaterial(int id)
        {
            // البحث عن الوسائط في قاعدة البيانات
            var showMaterial = await _context.ShowMaterials.FindAsync(id);

            if (showMaterial == null)
            {
                return NotFound("لا توجد وسائط بهذا المعرف");
            }

            // حذف الملف من النظام
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "upload/show_material", Path.GetFileName(showMaterial.MatPath));
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }

            // حذف السجل من قاعدة البيانات
            _context.ShowMaterials.Remove(showMaterial);
            await _context.SaveChangesAsync();

            return Ok(new { message = "تم حذف الوسائط بنجاح" });
        }
    }

}