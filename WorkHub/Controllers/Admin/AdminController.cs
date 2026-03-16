using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WorkHub.DataAccess.Data;
using WorkHub.Models.DTOs;
using WorkHub.Models.DTOs.ModelDTOs;
using WorkHub.Models.Models;
using WorkHub.Utility;
using AutoMapper;

namespace WorkHub.Controllers.Admin
{
    [Route("api/[controller]")]
    [ApiController]
    // [Authorize(Roles = SD.Role_Admin)] // Uncomment when roles are fully implemented
    public class AdminController : ControllerBase
    {
        private readonly WorkHubDbContext _context;
        private readonly IMapper _mapper;

        public AdminController(WorkHubDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet("users")]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                var users = await _context.Users
                    .Include(u => u.UserDetail)
                    .Include(u => u.Subscription)
                    .ToListAsync();

                var userDTOs = _mapper.Map<IEnumerable<UserDTO>>(users);

                return Ok(ApiResponse<IEnumerable<UserDTO>>.Ok(userDTOs, "Users retrieved successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.Error(500, "Failed to retrieve users", ex.Message));
            }
        }

        [HttpGet("users/{id}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            try
            {
                var user = await _context.Users
                    .Include(u => u.UserDetail)
                    .Include(u => u.Subscription)
                    .FirstOrDefaultAsync(u => u.Id == id);

                if (user == null)
                {
                    return NotFound(ApiResponse<object>.NotFound("User not found"));
                }

                var userDTO = _mapper.Map<UserDTO>(user);

                return Ok(ApiResponse<UserDTO>.Ok(userDTO, "User retrieved successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.Error(500, "Failed to retrieve user", ex.Message));
            }
        }

        [HttpPost("users/{id}/toggle-status")]
        public async Task<IActionResult> ToggleUserStatus(int id)
        {
            try
            {
                var user = await _context.Users.FindAsync(id);
                if (user == null)
                {
                    return NotFound(ApiResponse<object>.NotFound("User not found"));
                }

                user.Status = user.Status == SD.UserStatus_Active ? SD.UserStatus_Suspended : SD.UserStatus_Active;
                await _context.SaveChangesAsync();

                return Ok(ApiResponse<object>.Ok(new { status = user.Status }, "User status updated successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.Error(500, "Failed to toggle user status", ex.Message));
            }
        }
    }
}
