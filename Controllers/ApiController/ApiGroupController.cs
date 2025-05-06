using MediaPlus.DBModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MediaPlus.Controllers.ApiController
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApiGroupController : ControllerBase
    {
        private readonly MediaPlusDbContext _context;

        public ApiGroupController(MediaPlusDbContext context)
        {
            _context = context;
        }

        // عرض جميع الأجهزة
        [HttpGet("show/{customerCode}")]
        public async Task<ActionResult<IEnumerable<AdGroup>>> GetGroups(string customerCode)
        {
                return await _context.AdGroups.Where(d => d.GroupCustCode == customerCode).ToListAsync();
        }

        // عرض جهاز محدد
        [HttpGet("show/{customerCode}/{id}")]
        public async Task<ActionResult<AdGroup>> GetGroup(string customerCode, int id)
        {
            var group = await _context.AdGroups.FindAsync(id);

            if (group == null)
            {
                return NotFound();
            }

            return group;
        }

        //// إضافة جهاز جديد
        //[HttpPost]
        //public async Task<ActionResult<AdGroup>> PostGroup(AdGroup group)
        //{
        //    _context.AdGroups.Add(group);
        //    await _context.SaveChangesAsync();

        //    return CreatedAtAction("GetGroup", new { id = group.GroupId }, group);
        //}


        [HttpPost]
        public async Task<ActionResult<AdGroup>> PostGroup([FromForm] AdGroup group)
        {
            if (group == null)
            {
                return BadRequest("Group cannot be null"); // Return 400 if the input is invalid
            }

            // Create a new AdGroup instance
            var newAdGroup = new AdGroup
            {
                GroupName = group.GroupName,
                GroupCdate = group.GroupCdate,
                GroupUdate = DateTime.Now, // Set the update date
                GroupCustCode = group.GroupCustCode,
                GroupByUserid = group.GroupByUserid,
            };

            // Add the new group to the context
            await _context.AdGroups.AddAsync(newAdGroup);
            await _context.SaveChangesAsync();

            // Return 201 Created with the location of the new resource
            return CreatedAtAction("GetGroup", new { id = newAdGroup.GroupId }, newAdGroup);
        }



        // تعديل جهاز موجود
        [HttpPut("edit/{id}")]
        public async Task<IActionResult> PutGroup(int id, AdGroup group)
        {
            if (id != group.GroupId)
            {
                return BadRequest();
            }

            _context.Entry(group).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!GroupExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // حذف جهاز
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGroup(int id)
        {
            var group = await _context.AdGroups.FindAsync(id);
            if (group == null)
            {
                return NotFound(new { message = "المجموعة غير موجود" });
            }

            _context.AdGroups.Remove(group);
            await _context.SaveChangesAsync();

            return Ok(new { message = "تم حذف المجموعة بنجاح" });
        }
        private bool GroupExists(int id)
        {
            return _context.AdGroups.Any(e => e.GroupId == id);
        }
    }
}