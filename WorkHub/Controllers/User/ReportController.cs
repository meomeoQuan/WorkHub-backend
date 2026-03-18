using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using WorkHub.DataAccess.Data;
using WorkHub.Models.DTOs;
using WorkHub.Models.Models;
using WorkHub.Utility;

namespace WorkHub.Controllers.User
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportController : ControllerBase
    {
        private readonly WorkHubDbContext _context;

        public ReportController(WorkHubDbContext context)
        {
            _context = context;
        }

        public class ReportCreateDTO
        {
            public int ReporterId { get; set; }
            public int ReportedUserId { get; set; }
            public string Reason { get; set; }
            public string Description { get; set; }
        }

        [HttpPost]
        public async Task<IActionResult> CreateReport([FromBody] ReportCreateDTO dto)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ApiResponse<object>.BadRequest("Invalid data"));
                
                // 1. Spam Prevention: Check if already reported
                var alreadyReported = await _context.Reports.AnyAsync(r => r.ReporterId == dto.ReporterId && r.ReportedUserId == dto.ReportedUserId);
                if (alreadyReported) 
                {
                    return BadRequest(ApiResponse<object>.BadRequest("You have already reported this user."));
                }

                var reportedUser = await _context.Users
                    .Include(u => u.UserDetail)
                    .FirstOrDefaultAsync(u => u.Id == dto.ReportedUserId);
                var reporter = await _context.Users.FindAsync(dto.ReporterId);

                if (reportedUser == null || reporter == null) 
                    return NotFound(ApiResponse<object>.NotFound("User not found."));

                // 2. Save the Report
                var report = new Report
                {
                    ReporterId = dto.ReporterId,
                    ReportedUserId = dto.ReportedUserId,
                    Reason = dto.Reason,
                    Description = dto.Description,
                    CreatedAt = DateTime.UtcNow,
                    Status = "Pending"
                };
                _context.Reports.Add(report);

                // 3. Increase Report Count
                reportedUser.ReportCount += 1;

                // 4. Auto Actions Threshold Logic
                if (reportedUser.ReportCount == 3)
                {
                    // Decrease rating by 0.5 (min 0)
                    if (reportedUser.UserDetail != null)
                    {
                        var newRating = Math.Max(0.0, (reportedUser.UserDetail.Rating ?? 4.0) - 0.5);
                        reportedUser.UserDetail.Rating = newRating;
                    }
                }
                else if (reportedUser.ReportCount == 5)
                {
                    // Temporary restrict (+ decrease rating by an additional 0.5)
                    reportedUser.Status = "restricted";
                    if (reportedUser.UserDetail != null)
                    {
                        var newRating = Math.Max(0.0, (reportedUser.UserDetail.Rating ?? 4.0) - 0.5);
                        reportedUser.UserDetail.Rating = newRating;
                    }
                }
                else if (reportedUser.ReportCount >= 10)
                {
                    // Auto suspend
                    reportedUser.Status = SD.UserStatus_Suspended;
                }

                await _context.SaveChangesAsync();

                return Ok(ApiResponse<object>.Ok(new { reportId = report.Id }, "Report submitted successfully."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.Error(500, "Failed to submit report", ex.Message));
            }
        }
    }
}
