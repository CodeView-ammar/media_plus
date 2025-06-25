using MediaPlus.DBModels;
using MediaPlus.DBModels.Repository;
using MediaPlus.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Drawing.Printing;
using System.Text;

namespace MediaPlus.Controllers.ApiController
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShowApiController : ControllerBase
    {
        private readonly MediaPlusDbContext _context;
        private readonly IHttpContextAccessor _accessor;
        private readonly CustomerSessionModel? _currentCustomer;
        private readonly UserSessionModel? _currentUser;
        private readonly IWebHostEnvironment _env;
        private readonly IRepository<TemplateDetail> _ShowTemplateDetailTb;
        public ShowApiController(MediaPlusDbContext context,
                                          IHttpContextAccessor accessor,
                                          IWebHostEnvironment env) // Inject all dependencies
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _accessor = accessor ?? throw new ArgumentNullException(nameof(accessor));
            _env = env ?? throw new ArgumentNullException(nameof(env)); // Add env to the constructor

            _currentUser = _accessor.HttpContext?.Session.GetObject<UserSessionModel>("UserObject");
            _currentCustomer = _accessor.HttpContext?.Session.GetObject<CustomerSessionModel>("CustomerObject");
        }

        public class ToggleStatusViewModel
        {
            public int ShowId { get; set; }
            public int ShowIsactive { get; set; }
        }

        // ✅ تفعيل أو إلغاء تفعيل القالب
        [HttpPut("toggle-status")]
        public async Task<IActionResult> ToggleShowStatus([FromBody] ToggleStatusViewModel model)
        {
            var show = await _context.Shows.FindAsync(model.ShowId);
            if (show == null)
                return NotFound(new { message = "القالب غير موجود" });

            // تغيير حالة النشاط
            show.ShowIsactive = model.ShowIsactive;
            show.ShowUdate = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Ok(new { message = "تم  القالب بنجاح" });
        }


        [HttpGet("custcode")]
        public ActionResult<IEnumerable<ShowDto>> GetShows(string custcode)
        {
            var shows = _context.Shows
                .Where(s => s.ShowCustCode == custcode)
                .Select(s => new ShowDto
                {
                    ShowId = s.ShowId,
                    ShowTime = s.ShowTime,
                    ShowTemplateId = s.ShowTemplateId,
                    ShowCode = s.ShowCode,
                    ShowOrder = s.ShowOrder,
                    ShowCdate = s.ShowCdate,
                    ShowCustCode = s.ShowCustCode,
                    ShowIsactive = s.ShowIsactive,
                    IsScheduled = s.IsScheduled,
                    ScheduledFrom = s.ScheduledFrom,
                    ScheduledTo = s.ScheduledTo,
                    ScheduledRunNo = s.ScheduledRunNo
                })
                .ToList();

            return Ok(shows);
        }




        // ✅ GET: api/showapi/5
        [HttpGet("{id}")]
        public ActionResult<Show> GetShow(int id)
        {
            var show = _context.Shows.FirstOrDefault(s => s.ShowId == id);

            if (show == null) return NotFound();

            return Ok(show);
        }

        public class ShowDetailDto
        {
            public int ShowDetailsFileId { get; set; }
            public int ShowDetailsZoneId { get; set; }
        }
        public class ShowCreateDto
        {
            public int ShowTime { get; set; }
            public int TemplateId { get; set; }
            public int deviceId { get; set; }

            public bool IsScheduled { get; set; }
            public DateTime? ScheduledFrom { get; set; }
            public DateTime? ScheduledTo { get; set; }
            public int? ScheduledRunNo { get; set; }
            public string ShowCustCode { get; set; }
            public List<ShowDetailDto>? ShowDetails { get; set; }

        }
        public class ShowCreateRequest
        {
            public List<ShowCreateDto> Shows { get; set; }
        }
        [HttpPost("CreateShows")]
        public async Task<ActionResult<IEnumerable<Show>>> CreateShows([FromBody] ShowCreateRequest request)
        {
            if (request == null || request.Shows == null || !request.Shows.Any())
            {
                return BadRequest("No shows provided in the request body.");
            }

            var createdShows = new List<Show>();
            
            var devices = await _context.AdDevices
                .Where(d => d.DevicesCustCode == request.Shows[0].ShowCustCode.ToString())
                .ToListAsync();

            if (devices == null || devices.Count == 0)
                return BadRequest("لا توجد أجهزة متاحة.");

            var showCode = Guid.NewGuid().ToString("n").Substring(0, 8);

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                int deviceIndex = 0;

                foreach (var showDto in request.Shows)
                {
                    if (deviceIndex >= devices.Count)
                        break; // لا نكرر الأجهزة أكثر من عددها

                    var device = devices[deviceIndex++];

                    // إنشاء إعداد العرض
                    var showSetting = new ShowSetting
                    {
                        ShowSettingGroupId = 1,
                        ShowSettingTotalView = 1,
                        ShowSettingDeviceId = showDto.deviceId,
                        ShowSettingPresent = 1,
                        ShowSettingNext = 2,
                        ShowSettingCdate = DateTime.Now,
                        ShowSettingUdate = DateTime.Now,
                        ShowSettingShowcode = showCode,
                        ShowSettingCustCode = showDto.ShowCustCode
                    };

                    _context.ShowSettings.Add(showSetting);
                    await _context.SaveChangesAsync();

                    // إنشاء العرض نفسه
                    var show = new Show
                    {
                        ShowSettingId = showSetting.ShowSettingId,
                        ShowCode = Guid.NewGuid().ToString("n").Substring(0, 8),
                        ShowOrder = 1,
                        ShowTime = showDto.ShowTime,
                        ShowTemplateId = showDto.TemplateId,
                        IsScheduled = showDto.IsScheduled,
                        ScheduledFrom = showDto.ScheduledFrom,
                        ScheduledTo = showDto.ScheduledTo,
                        ScheduledRunNo = showDto.ScheduledRunNo,
                        ShowCdate = DateTime.Now,
                        ShowUdate = DateTime.Now,
                        ShowByUserid = 1,
                        ShowCustCode = showDto.ShowCustCode,
                        ShowIsactive = 1,
                    };

                    _context.Shows.Add(show);
                    await _context.SaveChangesAsync();

                    createdShows.Add(show);

                    // ربط الوسائط
                    var showTemplateDetails = await _context.TemplateDetails
                        .Where(x => x.TempTempId == show.ShowTemplateId)
                        .ToListAsync();

                    if (showDto.ShowDetails != null)
                    {
                        foreach (var detailDto in showDto.ShowDetails)
                        {
                            var templateDetail = showTemplateDetails.FirstOrDefault(); // يمكن تحسينها حسب المنطقة

                            if (templateDetail != null)
                            {
                                var detail = new ShowDetail
                                {
                                    ShowDetailsShowid = show.ShowId,
                                    ShowDetailsShowcode = show.ShowCode,
                                    ShowDetailsCustCode = showDto.ShowCustCode,
                                    ShowDetailsFileId = detailDto.ShowDetailsFileId,
                                    ShowDetailsZoneId = templateDetail.TempDetail,
                                    ShowDetailsIsactive = 1,
                                    ShowDetailsCdate = DateTime.Now,
                                    ShowDetailsUdate = DateTime.Now,
                                };

                                _context.ShowDetails.Add(detail);
                            }
                        }

                        await _context.SaveChangesAsync();
                    }

                    // حفظ HTML العرض
                    var showHtml = new ShowHtmlcode
                    {
                        ShowHtmlCode1 = GetHtmlCode(show),
                        ShowCdate = DateTime.Now,
                        ShowUdate = DateTime.Now,
                        ShowCustCode = _currentCustomer?.CustCode,
                        ShowByuserId = _currentUser?.UserId,
                        ShowIsactive = 1,
                        ShowUserid = show.ShowByUserid,
                        ShowSettingId = showSetting.ShowSettingId,
                        ShowCode = showSetting.ShowSettingShowcode,
                    };

                    _context.ShowHtmlcodes.Add(showHtml);
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return CreatedAtAction(nameof(GetShows), new { custcode = _currentCustomer?.CustCode }, createdShows);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, $"حدث خطأ أثناء إنشاء العروض: {ex.Message}");
            }
        }

        // ✅ PUT: api/showapi/5
        [HttpPut("{id}")]
        public IActionResult UpdateShow(int id, [FromBody] Show updatedShow)
        {
            var show = _context.Shows.FirstOrDefault(s => s.ShowId == id);
            if (show == null) return NotFound();

            show.ShowId = updatedShow.ShowId;
            show.ScheduledFrom = updatedShow.ScheduledFrom;
            show.ShowUdate = DateTime.Now;

            _context.SaveChanges();
            return NoContent();
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteShow(int id)
        {
            var show = await _context.Shows.FirstOrDefaultAsync(s => s.ShowId == id);
            if (show == null) return NotFound();

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // الحصول على كل العروض المرتبطة بنفس الإعداد
                var showsInGroup = await _context.Shows
                    .Where(s => s.ShowSettingId == show.ShowSettingId)
                    .ToListAsync();

                foreach (var s in showsInGroup)
                {
                    // حذف التفاصيل المرتبطة بكل عرض
                    var details = await _context.ShowDetails
                        .Where(d => d.ShowDetailsShowid == s.ShowId)
                        .ToListAsync();
                    _context.ShowDetails.RemoveRange(details);

                    _context.Shows.Remove(s);
                }

                // حذف سجل الإعداد إذا كان موجوداً
                var setting = await _context.ShowSettings
                    .FirstOrDefaultAsync(ss => ss.ShowSettingId == show.ShowSettingId);
                if (setting != null)
                    _context.ShowSettings.Remove(setting);

                // حذف كود HTML المرتبط
                var htmlCodes = await _context.ShowHtmlcodes
                    .Where(h => h.ShowSettingId == show.ShowSettingId)
                    .ToListAsync();
                _context.ShowHtmlcodes.RemoveRange(htmlCodes);

                // تنفيذ الحذف بعد التأكد من أن كل شيء جاهز
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync(); // التراجع عن كل الحذف
                return StatusCode(500, $"حدث خطأ أثناء الحذف، تم التراجع عن العملية بالكامل: {ex.Message}");
            }
        }

        //======================================================================================
        private string GetHtmlCode(Show show)
        {
            // Get the show details
            var showDetails = _context.ShowDetails
                .Where(c => c.ShowDetailsShowid == show.ShowId)
                .ToList();

            int mediaCount = showDetails.Count;

            // Generate HTML based on the number of media items
            var sbHTMLCode = new StringBuilder();
            sbHTMLCode.Append($@"<!DOCTYPE html><html><head><title>Media Plus Front End</title><meta charset='utf-8'><meta name='viewport' content='width=device-width, initial-scale=1.0'><style> *{{margin:0;padding:0;box-sizing:border-box;}}  html,body{{width:100vw;height:100vh;overflow:hidden;}}.container{{width:100vw;height:100vh;display:flex;flex-direction:column;}}.row{{flex:1;display:flex;}}.cell{{flex:1;overflow:hidden;position:relative;display:flex;align-items:center;justify-content:center;background:white;}}.media-item{{width:100%;height:100%;object-fit:fill;}}.video-item{{object-fit:contain !important;}}.youtube-item{{width:100%;height:100%;border:none;}}.link-item{{padding:1rem 2rem;background:#0066cc;color:white;text-decoration:none;border-radius:8px;font-size:1.2rem;width:100%;height:100%;display:flex;align-items:center;justify-content:center;}}@keyframes scroll{{0%{{transform:translateX(100%);}}100%{{transform:translateX(-100%);}}}}.scrolling-text{{animation:scroll 15s linear infinite;font-size:1.5rem;color:#333;font-weight:bold;padding:1rem;white-space:nowrap;position:absolute;}}</style></head><body><div class='container'>");

            if (mediaCount == 1)
            {
                // Display single media item
                var item = showDetails.First();
                string mediaHtml = GetMediaHtml(item);
                sbHTMLCode.Append($"<div class='row'><div class='cell'>{mediaHtml}</div></div>");
            }
            else if (mediaCount == 2)
            {
                // Display two media items in a row
                sbHTMLCode.Append("<div class='row'>");
                foreach (var item in showDetails)
                {
                    string mediaHtml = GetMediaHtml(item);
                    sbHTMLCode.Append($"<div class='cell'>{mediaHtml}</div>");
                }
                sbHTMLCode.Append("</div>");
            }
            else if (mediaCount == 3)
            {
                sbHTMLCode.Append("<div class='row'>");
                string mediaHtml = GetMediaHtml(showDetails[0]);
                sbHTMLCode.Append($"<div class='cell'>{mediaHtml}</div>");
                sbHTMLCode.Append("</div>");
                sbHTMLCode.Append("<div class='row'>");
                for (int i = 1; i < showDetails.Count; i++)
                {
                    string mediaHtml2 = GetMediaHtml(showDetails[i]);
                    sbHTMLCode.Append($"<div class='cell'>{mediaHtml2}</div>");
                }
                sbHTMLCode.Append("</div>");

            }
            else if (mediaCount == 4)
            {
                // Display four media items in a 2x2 grid
                sbHTMLCode.Append("<div class='row'>");
                for (int i = 0; i < 2; i++)
                {
                    var item = showDetails[i];
                    string mediaHtml = GetMediaHtml(item);
                    sbHTMLCode.Append($"<div class='cell'>{mediaHtml}</div>");
                }
                sbHTMLCode.Append("</div>");

                sbHTMLCode.Append("<div class='row'>");
                for (int i = 2; i < 4; i++)
                {
                    var item = showDetails[i];
                    string mediaHtml = GetMediaHtml(item);
                    sbHTMLCode.Append($"<div class='cell'>{mediaHtml}</div>");
                }
                sbHTMLCode.Append("</div>");
            }
            else
            {
                // Handle cases with more than 4 media items or no media items
                sbHTMLCode.Append("<div>Unsupported number of media items.</div>");
            }

            sbHTMLCode.Append("</div></body></html>");
            return sbHTMLCode.ToString();
        }

        private string GetMediaHtml(ShowDetail item)
        {
            var material = _context.ShowMaterials.Find(item.ShowDetailsFileId);
            if (material == null)
            {
                Console.WriteLine($"Warning: ShowMaterial with ID {item.ShowDetailsFileId} not found.");
                return ""; // Return empty string or placeholder
            }

            var materialType = _context.MaterialTypes.Find(material.MatTypeId);
            if (materialType == null)
            {
                Console.WriteLine($"Warning: MaterialType with ID {material.MatTypeId} not found.");
                return "";
            }

            string placeholderValue = material.MatPath.Split(",")[0];
            string domainName = "http://" + _accessor.HttpContext.Request.Host.Value;
            placeholderValue = $"{domainName}/upload/show_material/{placeholderValue}";

            string fileExtension = Path.GetExtension(placeholderValue).ToLower();

            if (new[] { ".mp4", ".webm", ".ogg" }.Contains(fileExtension))
            {
                // Video
                return $@"<video id='myVideo' class='media-item' autoplay muted loop controls>
                            <source src='{placeholderValue}' type='video/{fileExtension.Substring(1)}'>
                            Your browser does not support the video tag.
                        </video>
                        <script>
                            const video = document.getElementById('myVideo');

                            video.addEventListener('ended', function() {{
                                video.currentTime = 0;
                                video.play();
                            }});

                            window.addEventListener('load', function() {{
                                video.play().catch(error => {{
                                    console.log('Autoplay prevented: ', error);
                                }});
                            }});
                        </script>";
            }
            else if (new[] { ".jpg", ".jpeg", ".png", ".gif" }.Contains(fileExtension))
            {
                // Image
                return $"<img src='{placeholderValue}' class='media-item'>";
            }
            else if (materialType.MtypeNameEn.ToLower() == "youtube")
            {
                //YouTube
                return $"<iframe class='youtube-item' src='{GetEmbedUrl(placeholderValue)}' frameborder='0' allowfullscreen></iframe>";
            }
            else
            {
                return $"<div>Unsupported media type or file extension: {materialType.MtypeNameEn} ({fileExtension})</div>";
            }
        }
        private string GetEmbedUrl(string url)
        {
            if (url.Contains("youtube.com") || url.Contains("youtu.be"))
            {
                var videoId = System.Text.RegularExpressions.Regex.Match(
                    url, @"(?:youtu\.be\/|v=|\/embed\/|\/v\/|\/vi\/|\/user\/\S+\/\S+\/|watch\?v=)([\w-]+)")
                    .Groups[1].Value;
                return $"https://www.youtube.com/embed/{videoId}?autoplay=1&mute=1&controls=0&loop=1&playlist={videoId}&modestbranding=1&showinfo=0&rel=0&fs=0&disablekb=1&iv_load_policy=3";
            }

            return url;
        }

    }

}