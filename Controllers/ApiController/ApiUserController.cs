// تم إصلاح الخطأ AmbiguousMatchException بجعل التوقيع مختلفًا عن Get() الأساسي
// عبر إضافة اسم مميز للأكشن أو استخدام [HttpGet("route")] محدد

using MediaPlus.DBModels.Repository;
using MediaPlus.DBModels;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;

[Route("api/[controller]")]
[ApiController]
public class ApiUserController : ControllerBase
{
    private readonly IRepository<User> _userTb;
    private readonly UnitOfWork _unitOfWork;
    private readonly MediaPlusDbContext _context;

    public ApiUserController(
        MediaPlusDbContext context,
        IConfiguration configuration,
        IHttpContextAccessor accessor)
    {
        _unitOfWork = new UnitOfWork();
        _userTb = _unitOfWork.GetRepositoryInstance<User>();
        _context = context;
    }

    // GET: api/ApiUser/all
    [HttpGet("all")]
    public ActionResult<IEnumerable<User>> GetAllUsers()
    {
        var users = _userTb.GetAll();
        if (users == null || !users.Any())
        {
            return NotFound();
        }
        return Ok(users);
    }
    // GET: api/ApiUser/bycode?custCode=123?userid=1
    [HttpGet("perinfo/{userid}")]
    public ActionResult<IEnumerable<User>> GetUsersByCustCode(
        
        [FromQuery] int userid) // إضافة userid كمعامل
    {

        var users = _userTb.GetAll().Where(u => u.UserId== userid).ToList();

        if (!users.Any())
        {
            return NotFound();
        }

        return Ok(users);
    }

    // GET: api/ApiUser/statistics/custcode
    [HttpGet("statistics/{custCode}")]
    public async Task<IActionResult> GetStatistics(string custCode)
    {
        if (string.IsNullOrEmpty(custCode))
        {
            return BadRequest("custCode is required");
        }

        var activeContentCount = await _context.Shows
            .Where(s => s.ShowIsactive == 1 && s.ShowCustCode == custCode)
            .CountAsync();

        var totalPlayMinutes = await _context.AdDevices
            .Where(d => d.DevicesOnoff == 1 && d.DevicesCdate != null && d.DevicesCustCode == custCode)
            .Select(d => EF.Functions.DateDiffMinute(d.DevicesCdate.Value, DateTime.Now))
            .SumAsync();

        var totalPlayHours = totalPlayMinutes / 60.0;

        var managedScreensCount = await _context.AdDevices
            .Where(d => d.DevicesOnoff == 1 && d.DevicesCustCode == custCode)
            .CountAsync();

        return Ok(new
        {
            ActiveContentCount = activeContentCount,
            TotalPlayHours = totalPlayHours,
            ManagedScreensCount = managedScreensCount
        });
    }

}
