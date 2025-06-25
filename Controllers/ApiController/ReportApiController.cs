using MediaPlus.DBModels;
using MediaPlus.DBModels.Repository;
using MediaPlus.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MediaPlus.Controllers.ApiController
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportApiController : ControllerBase
    {
        private readonly MediaPlusDbContext _context;

        public ReportApiController(MediaPlusDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        // 1. تقرير أداء الأجهزة
        [HttpGet("devices-performance")]
        public async Task<IActionResult> GetDevicesPerformanceReport([FromQuery] string custCode)
        {
            var result = await _context.AdDevices
                .Where(d => d.DevicesCustCode == custCode)
                .Select(d => new
                {
                    d.DevicesId,
                    d.DevicesName,
                    d.DevicesOnoff,
                    d.DeviceIsInterrupt,
                    LastUpdate = _context.AdDevices
                        .Where(u => u.DevicesId == d.DevicesId)
                        .OrderByDescending(u => u.DevicesUdate)
                        .Select(u => u.DevicesUdate)
                        .FirstOrDefault()
                })
                .ToListAsync();

            return Ok(result);
        }

        // 2. تقرير العروض لكل جهاز
        [HttpGet("shows-per-device")]
        public async Task<IActionResult> GetShowsPerDevice([FromQuery] string custCode)
        {
            var result = await _context.ShowSettings
                .Where(s => s.ShowSettingCustCode == custCode)
                .Select(s => new
                {
                    s.ShowSettingDeviceId,
                    DeviceName = _context.AdDevices
                        .Where(d => d.DevicesId == s.ShowSettingDeviceId)
                        .Select(d => d.DevicesName)
                        .FirstOrDefault(),
                    ShowsCount = _context.Shows.Count(sh => sh.ShowSettingId == s.ShowSettingId)
                })
                .ToListAsync();

            return Ok(result);
        }

        // باقي التقارير يتم تمرير custCode لها بنفس الطريقة عبر [FromQuery] string custCode
    }
}
