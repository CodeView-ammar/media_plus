using MediaPlus.DBModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace MediaPlus.Controllers.ApiController
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApiLiveShowsController : ControllerBase
    {
        private readonly MediaPlusDbContext _context;

        public ApiLiveShowsController(MediaPlusDbContext context)
        {
            _context = context;
        }

        public class LiveShowResponse
        {
            public int ShowId { get; set; }
            public string ShowCode { get; set; }
            public string DeviceName { get; set; }
            public string Deviceimg { get; set; }
            public string GroupName { get; set; }
            public string HtmlCode { get; set; }
            public DateTime? ScheduledFrom { get; set; }
            public DateTime? ScheduledTo { get; set; }
            public bool IsActive { get; set; }
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<LiveShowResponse>>> GetLiveShows()
        {
            var currentTime = DateTime.Now;

            var liveShows = await (
                from show in _context.Shows
                join setting in _context.ShowSettings on show.ShowSettingId equals setting.ShowSettingId
                join device in _context.AdDevices on setting.ShowSettingDeviceId equals device.DevicesId
                join AdGroup in _context.AdGroups on setting.ShowSettingGroupId equals AdGroup.GroupId
                join htmlCode in _context.ShowHtmlcodes on show.ShowSettingId equals htmlCode.ShowSettingId
                where show.ShowIsactive == 1
                    && (!show.IsScheduled.HasValue || show.IsScheduled == false
                        || (show.IsScheduled == true
                            && show.ScheduledFrom <= currentTime
                            && show.ScheduledTo >= currentTime))
                    && device.DevicesOnoff == 1
                    && setting.ShowSettingPresent == 1
                select new LiveShowResponse
                {
                    ShowId = show.ShowId,
                    ShowCode = show.ShowCode,
                    DeviceName = device.DevicesName,
                    Deviceimg=device.DevicesOfflinePhoto,
                    GroupName = AdGroup.GroupName,
                    HtmlCode = htmlCode.ShowHtmlCode1,
                    ScheduledFrom = show.ScheduledFrom,
                    ScheduledTo = show.ScheduledTo,
                    IsActive = show.ShowIsactive == 1
                }).ToListAsync();

            return Ok(liveShows);
        }

        [HttpGet("{deviceId}")]
        public async Task<ActionResult<LiveShowResponse>> GetLiveShowByDevice(int deviceId)
        {
            var currentTime = DateTime.Now;

            var liveShow = await (
                from show in _context.Shows
                join setting in _context.ShowSettings on show.ShowSettingId equals setting.ShowSettingId
                join device in _context.AdDevices on setting.ShowSettingDeviceId equals device.DevicesId
                join AdGroup in _context.AdGroups on setting.ShowSettingGroupId equals AdGroup.GroupId
                join htmlCode in _context.ShowHtmlcodes on show.ShowSettingId equals htmlCode.ShowSettingId
                where device.DevicesId == deviceId
                    && show.ShowIsactive == 1
                    && (!show.IsScheduled.HasValue || show.IsScheduled == false
                        || (show.IsScheduled == true
                            && show.ScheduledFrom <= currentTime
                            && show.ScheduledTo >= currentTime))
                    && device.DevicesOnoff == 1
                    && setting.ShowSettingPresent == 1
                select new LiveShowResponse
                {
                    ShowId = show.ShowId,
                    ShowCode = show.ShowCode,
                    DeviceName = device.DevicesName,
                    GroupName = AdGroup.GroupName,
                    HtmlCode = htmlCode.ShowHtmlCode1,
                    ScheduledFrom = show.ScheduledFrom,
                    ScheduledTo = show.ScheduledTo,
                    IsActive = show.ShowIsactive == 1
                }).FirstOrDefaultAsync();

            if (liveShow == null)
            {
                return NotFound();
            }

            return Ok(liveShow);
        }
    }
}
