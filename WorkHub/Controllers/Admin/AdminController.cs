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

        [HttpGet("users/{id}/profile")]
        public async Task<IActionResult> GetUserProfile(int id)
        {
            try
            {
                var user = await _context.Users
                    .Include(u => u.UserDetail)
                    .Include(u => u.Subscription)
                    .Include(u => u.UserExperiences)
                    .Include(u => u.UserEducations)
                    .Include(u => u.UserSchedules)
                    .FirstOrDefaultAsync(u => u.Id == id);

                if (user == null)
                {
                    return NotFound(ApiResponse<object>.NotFound("User not found"));
                }

                var profileDTO = _mapper.Map<UserProfileDTO>(user);

                return Ok(ApiResponse<UserProfileDTO>.Ok(profileDTO, "User profile retrieved successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.Error(500, "Failed to retrieve user profile", ex.Message));
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

        [HttpPut("users/{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] AdminUserUpdateDTO dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ApiResponse<object>.BadRequest("Invalid data"));
                }

                var user = await _context.Users
                    .Include(u => u.UserDetail)
                    .FirstOrDefaultAsync(u => u.Id == id);

                if (user == null)
                {
                    return NotFound(ApiResponse<object>.NotFound("User not found"));
                }

                // Update only core User model fields
                user.FullName = dto.FullName;
                user.Email = dto.Email;
                user.Role = dto.Role;
                user.Status = dto.Status;

                // Sync profile metrics to UserDetail if it exists
                if (user.UserDetail == null)
                {
                    user.UserDetail = new UserDetail { UserId = user.Id };
                }
                
                if (dto.TotalJobs.HasValue) user.UserDetail.TotalJobs = dto.TotalJobs.Value;
                if (dto.TotalPosts.HasValue) user.UserDetail.TotalPosts = dto.TotalPosts.Value;
                if (dto.Rating.HasValue) user.UserDetail.Rating = dto.Rating.Value;

                await _context.SaveChangesAsync();
                
                var userDTO = _mapper.Map<UserDTO>(user);
                return Ok(ApiResponse<UserDTO>.Ok(userDTO, "User updated successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.Error(500, "Failed to update user", ex.Message));
            }
        }

        [HttpPost("users")]
        public async Task<IActionResult> CreateUser([FromBody] AdminUserCreateDTO dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ApiResponse<object>.BadRequest("Invalid data"));
                }

                var existing = await _context.Users.AnyAsync(u => u.Email.ToLower() == dto.Email.ToLower());
                if (existing)
                {
                    return BadRequest(ApiResponse<object>.BadRequest("Email already exists"));
                }

                var user = new User
                {
                    FullName = dto.FullName,
                    Email = dto.Email,
                    PasswordHash = BCryptHelper.Encode(dto.Password),
                    Role = dto.Role,
                    Status = SD.UserStatus_Active,
                    IsVerified = true, // Admin-created users are verified by default
                    CreatedAt = DateTime.UtcNow,
                    UserDetail = new UserDetail { Rating = 4 }
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                var userDTO = _mapper.Map<UserDTO>(user);
                return Ok(ApiResponse<UserDTO>.Ok(userDTO, "User created successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.Error(500, "Failed to create user", ex.Message));
            }
        }
    }
}
