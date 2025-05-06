using MediaPlus.DBModels.Repository;
using MediaPlus.DBModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MediaPlus.Models.ViewModels;

namespace MediaPlus.Controllers.ApiController
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApiShowController : ControllerBase
    {
        private readonly UnitOfWork _unitOfWork = new();
    

        [HttpGet("getShows")]
        public async Task<IActionResult> GetActiveShows(int devicesGroupid, string custCode)
        {
            // Check if the parameters are valid
            if (devicesGroupid <= 0 || string.IsNullOrEmpty(custCode))
            {
                return BadRequest("Invalid parameters.");
            }

            // Get the active shows with HTML code
            var showsWithHtml = await GetShowsWithHtmlCode(devicesGroupid, custCode);

         

            return Ok(showsWithHtml);
        }

        [HttpGet("getShowsScheduled")]
        public async Task<IActionResult> GetActiveShowsScheduled(int devicesGroupid, string custCode)
        {
            // تحقق من صحة المعلمات
            if (devicesGroupid <= 0 || string.IsNullOrEmpty(custCode))
            {
                return BadRequest("Invalid parameters.");
            }

            // الحصول على العروض النشطة مع كود HTML
            var showsWithHtml = await CheckForScheduledShows(devicesGroupid, custCode);

            // تحقق مما إذا كان هناك عرض واحد على الأقل يحتوي على IsScheduled يساوي true
            bool hasScheduledShows = showsWithHtml;

            if (hasScheduledShows)
            {
                return Ok(showsWithHtml); // إرجاع 200 إذا كان هناك عروض مجدولة
            }
            else
            {
                return BadRequest("No scheduled shows found."); // إرجاع 400 إذا لم يكن هناك عروض مجدولة
            }
        }

      private async Task<bool> CheckForScheduledShows(int devicesGroupid, string custCode)
        {
            var now = DateTime.Now;
            var today = now.Date;
            var currentTime = now.TimeOfDay;

            // تحقق من وجود عروض مجدولة تتوافق مع التاريخ والوقت الحالي
            var scheduledShowsExist = await _unitOfWork.GetRepositoryInstance<Show>()
                .EntitiesIQueryable()
                .AnyAsync(s =>
                    s.ShowIsactive == 1 &&
                    s.ShowCustCode == custCode &&
                    s.IsScheduled == true &&
                    s.ScheduledFrom.HasValue &&
                    s.ScheduledTo.HasValue &&
                    s.ScheduledFrom.Value.Date <= today &&
                    s.ScheduledTo.Value.Date >= today &&
                    s.ScheduledFrom.Value.TimeOfDay <= currentTime &&
                    s.ScheduledTo.Value.TimeOfDay >= currentTime
                );

            return scheduledShowsExist;
        }

        private async Task<List<ShowWithHtmlCode>> GetShowsWithHtmlCode(int devicesGroupid, string custCode)
        {

            // استعلام لجلب العروض النشطة
            var now = DateTime.Now;
            var today = now.Date;
            var currentTime = now.TimeOfDay;

            // أولاً: نجيب العروض المجدولة المتوافقة مع الوقت الحالي
            var scheduledShows = await _unitOfWork.GetRepositoryInstance<Show>()
                .EntitiesIQueryable()
                .Where(s => s.ShowIsactive == 1
                    && s.ShowCustCode == custCode
                    && s.IsScheduled == true
                    && s.ScheduledFrom.HasValue
                    && s.ScheduledTo.HasValue
                    && s.ScheduledFrom.Value.Date <= today
                    && s.ScheduledTo.Value.Date >= today
                    && s.ScheduledFrom.Value.TimeOfDay <= currentTime
                    && s.ScheduledTo.Value.TimeOfDay >= currentTime)
                .ToListAsync();

            List<Show> shows;

            if (scheduledShows.Any())
            {
                shows = scheduledShows;
            }
            else
            {
                shows = await _unitOfWork.GetRepositoryInstance<Show>()
                    .EntitiesIQueryable()
                    .Where(s => s.ShowIsactive == 1
                        && s.ShowCustCode == custCode
                        && s.IsScheduled == false)
                    .ToListAsync();
            }

            // استعلام لجلب أكواد HTML للعروض النشطة
            var showHtmlCodes = await _unitOfWork.GetRepositoryInstance<ShowHtmlcode>()
                .EntitiesIQueryable()
                .Where(h => h.ShowIsactive == 1)
                .ToListAsync();

            // استعلام لجلب إعدادات العروض بناءً على groupId
            var showSettings = await _unitOfWork.GetRepositoryInstance<ShowSetting>()
                .EntitiesIQueryable()
                .Where(ss => ss.ShowSettingGroupId == devicesGroupid)
                .ToListAsync();

            // استعلام لدمج العروض مع إعداداتها وأكواد HTML بناءً على show_code
            var result = from show in shows
                         join setting in showSettings on show.ShowSettingId equals setting.ShowSettingId
                         join html in showHtmlCodes on show.ShowCode equals html.ShowCode into showGroup
                         from html in showGroup.DefaultIfEmpty() // Left join to include shows without HTML codes
                         where show.ShowIsactive == 1
                               && show.ShowCustCode == custCode
                               && setting.ShowSettingGroupId == devicesGroupid // تصفية بناءً على deviceGroupId
                         select new ShowWithHtmlCode
                         {
                             ShowId = show.ShowId,
                             ShowCode = show.ShowCode,
                             ShowTime = show.ShowTime,
                             ShowOrder = show.ShowOrder,
                             ShowByUserId = show.ShowByUserid,
                             ShowCDate = show.ShowCdate,
                             ShowUDate = show.ShowUdate,
                             ShowCustCode = show.ShowCustCode,
                             ShowIsActive = show.ShowIsactive,
                             ShowHtmlCode = html?.ShowHtmlCode1 // الحصول على كود HTML
                         };

            return result.ToList();
        }
    }
}
