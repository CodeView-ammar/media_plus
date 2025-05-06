using MediaPlus.DBModels;
using MediaPlus.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace MediaPlus.Controllers.ApiController
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApiShowTemplateController : ControllerBase
    {
        private readonly MediaPlusDbContext _context;

        public ApiShowTemplateController(MediaPlusDbContext context)
        {
            _context = context;
        }

        // ✅ عرض جميع القوالب حسب كود العميل
        [HttpGet("show/{customerCode}")]
        public async Task<ActionResult<IEnumerable<ShowTemplate>>> GetTemplates(string customerCode)
        {

            var showTemplates = await _context.ShowTemplates
               .Include(t => t.TempDetails) // تضمين التفاصيل
               .Where(t => t.TempCustCode == customerCode)
               .Select(t => new ShowTemplateViewModel
               {
            
                   TempId = t.TempId,
                   TempNameAr = t.TempNameAr,
                   TempRowNo=t.TempRowNo,
                   TempColNo = t.TempColNo,
                   TempNameEng = t.TempNameEng,
                   TempCdate = t.TempCdate,
                   TempUdate = t.TempUdate,
                   TempByUserid= t.TempByUserid,
                   TempCustCode = t.TempCustCode,
                   TempIsactive = t.TempIsactive == 1,
                   TempDetails = _context.TemplateDetails
                    .Where(td => td.TempTempId == t.TempId) // مقارنة TempId مع TempTempId
                    .ToList()
               })
               .ToListAsync();

            return Ok(showTemplates);
        }
    

        // ✅ عرض قالب واحد
        [HttpGet("show/{customerCode}/{id}")]
        public async Task<ActionResult<ShowTemplate>> GetTemplate(string customerCode, int id)
        {
            var showTemplates = await _context.ShowTemplates
           .Include(t => t.TempDetails)
           .Where(t => t.TempCustCode == customerCode && t.TempId == id)
           .Select(t => new ShowTemplateViewModel
           {
               TempId = t.TempId,
               TempNameAr = t.TempNameAr,
               TempNameEng = t.TempNameEng,
               TempCdate = t.TempCdate,
               TempUdate = t.TempUdate,
               TempIsactive = bool.Parse(t.TempIsactive.ToString()),
               TempDetails = _context.TemplateDetails
                .Where(td => td.TempTempId == t.TempId) // مقارنة TempId مع TempTempId
                .ToList()
           })
   .ToListAsync();

            return Ok(showTemplates);

         }


        [HttpPost]
        public async Task<ActionResult<ShowTemplate>> PostTemplate([FromBody] ShowTemplate template)
        {
            // تحقق من أن النموذج غير فارغ
            if (template == null)
            {
                return BadRequest("بيانات القالب المطلوبة مفقودة.");
            }

            var newShowTemplate = new ShowTemplate()
            {
                TempNameAr = template.TempNameAr,
                TempNameEng = template.TempNameEng,
                TempColNo = template.TempColNo,
                TempRowNo = template.TempRowNo,
                TempByUserid = 1, // يمكن تعديلها لتكون من الجلسة
                TempCustCode = template.TempCustCode, // تأكد من وجود هذا الحقل
                TempCdate = DateTime.Now,
                TempUdate = DateTime.Now,
                TempIsactive = 1,
            };

            // إضافة القالب الجديد إلى قاعدة البيانات
            _context.ShowTemplates.Add(newShowTemplate);
            await _context.SaveChangesAsync();

            // حساب الأبعاد الافتراضية
            var defaultHeight = 100 / newShowTemplate.TempRowNo;
            var defaultWidth = 100 / newShowTemplate.TempColNo;
            var ZoneCode = 0.0;

            // إضافة تفاصيل القالب
            for (int i = 1; i <= newShowTemplate.TempRowNo; i++)
            {
                ZoneCode = i;

                for (int j = 1; j <= newShowTemplate.TempColNo; j++)
                {
                    ZoneCode = Math.Round(ZoneCode + .1, 1);

                    // إضافة تفاصيل القالب
                    _context.TemplateDetails.Add(new TemplateDetail
                    {
                        TempTempId = newShowTemplate.TempId,
                        TempByUserid = 1, // يمكن تعديلها لتكون من الجلسة
                        TempCustCode = template.TempCustCode, // تأكد من وجود هذا الحقل
                        TempUdate = DateTime.Now,
                        TempCdate = DateTime.Now,
                        TempZoneCode = ZoneCode,
                        TempZoneHeight = defaultHeight,
                        TempZoneWidth = defaultWidth,
                        TempIsactive = 1,
                    });
                }
            }

            // حفظ التفاصيل في قاعدة البيانات
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTemplate), new { customerCode = template.TempCustCode, id = newShowTemplate.TempId }, newShowTemplate);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutTemplate(int id, [FromBody] ShowTemplate template)
        {
            if (id != template.TempId)
                return BadRequest("معرف غير متطابق.");

            var existingTemplate = await _context.ShowTemplates.FindAsync(id);
            if (existingTemplate == null)
                return NotFound();

            // تحديث الحقول الرئيسية
            existingTemplate.TempNameAr = template.TempNameAr;
            existingTemplate.TempNameEng = template.TempNameEng;
            existingTemplate.TempColNo = template.TempColNo;
            existingTemplate.TempRowNo = template.TempRowNo;
            existingTemplate.TempIsactive = template.TempIsactive;
            existingTemplate.TempByUserid = template.TempByUserid;
            existingTemplate.TempUdate = DateTime.UtcNow;

            // حفظ التغييرات في الحقول الرئيسية
            await _context.SaveChangesAsync();

            // حساب الأبعاد الافتراضية للتفاصيل الجديدة
            var defaultHeight = 100 / existingTemplate.TempRowNo;
            var defaultWidth = 100 / existingTemplate.TempColNo;
            var ZoneCode = 0.0;

            // حذف تفاصيل القالب القديمة قبل إضافتها مرة أخرى (إذا كان هذا هو المطلوب)
            var existingDetails = await _context.TemplateDetails
                .Where(td => td.TempTempId == existingTemplate.TempId)
                .ToListAsync();
            _context.TemplateDetails.RemoveRange(existingDetails);

            // إضافة تفاصيل جديدة بناءً على الأبعاد
            for (int i = 1; i <= existingTemplate.TempRowNo; i++)
            {
                ZoneCode = i;

                for (int j = 1; j <= existingTemplate.TempColNo; j++)
                {
                    ZoneCode = Math.Round(ZoneCode + .1, 1);

                    // إضافة تفاصيل القالب الجديدة
                    _context.TemplateDetails.Add(new TemplateDetail
                    {
                        TempTempId = existingTemplate.TempId,
                        TempByUserid = template.TempByUserid,
                        TempCustCode = template.TempCustCode, // تأكد من وجود هذا الحقل
                        TempUdate = DateTime.Now,
                        TempCdate = DateTime.Now,
                        TempZoneCode = ZoneCode,
                        TempZoneHeight = defaultHeight,
                        TempZoneWidth = defaultWidth,
                        TempIsactive = 1, // افترض أن القالب نشط عند التعديل
                    });
                }
            }

            // حفظ التفاصيل في قاعدة البيانات
            await _context.SaveChangesAsync();

            return Ok(existingTemplate);
        }

        [HttpPut("detail/{TempTempId}")]
        public async Task<IActionResult> PutTemplateDetail(int TempTempId, [FromBody] TemplateDetail updatedDetail)
        {
            var detail = await _context.TemplateDetails
                .FirstOrDefaultAsync(td => td.TempTempId == TempTempId);

            if (detail == null)
            {
                return NotFound(new { message = "تفاصيل القالب غير موجودة" });
            }

            // تحديث الطول والعرض فقط
            detail.TempZoneWidth = updatedDetail.TempZoneWidth;
            detail.TempZoneHeight = updatedDetail.TempZoneHeight;
            detail.TempUdate = DateTime.UtcNow;

            // حفظ التغييرات في قاعدة البيانات
            await _context.SaveChangesAsync();

            return Ok(new { message = "تم تعديل التفاصيل بنجاح", detail });
        }
        public class ToggleStatusViewModel
        {
            public int? TempIsactive { get; set; }
        }
        // ✅ تفعيل أو إلغاء تفعيل القالب
        [HttpPut("toggle-status/{id}")]
        public async Task<IActionResult> ToggleTemplateStatus(int id, [FromBody] ToggleStatusViewModel model)
        {
            var template = await _context.ShowTemplates.FindAsync(id);
            if (template == null)
                return NotFound(new { message = "القالب غير موجود" });

            // تغيير حالة النشاط
            template.TempIsactive = model.TempIsactive;
            template.TempUdate = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Ok(new { message = "تم  القالب بنجاح" });
        }


        // ✅ حذف قالب
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTemplate(int id)
        {
            var template = await _context.ShowTemplates.FindAsync(id);
            if (template == null)
                return NotFound(new { message = "القالب غير موجود" });

            _context.ShowTemplates.Remove(template);
            await _context.SaveChangesAsync();

            return Ok(new { message = "تم حذف القالب بنجاح" });
        }
    }
}
