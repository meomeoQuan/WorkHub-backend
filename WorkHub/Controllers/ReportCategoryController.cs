using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WorkHub.DataAccess.Data;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace WorkHub.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportCategoryController : ControllerBase
    {
        private readonly WorkHubDbContext _context;

        public ReportCategoryController(WorkHubDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetReportCategories()
        {
            try
            {
                var categories = await _context.ReportCategories
                    .OrderBy(c => c.Id)
                    .Select(c => new {
                        c.Id,
                        c.Name,
                        c.Description
                    })
                    .ToListAsync();

                return Ok(new { success = true, data = categories });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Internal server error" });
            }
        }
    }
}
