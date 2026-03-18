using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WorkHub.DataAccess.Data;
using WorkHub.Models.DTOs;
using WorkHub.Models.DTOs.ModelDTOs;
using WorkHub.Models.DTOs.ModelDTOs.JobDTOs;
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
                    .Include(u => u.Orders)
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
                    .Include(u => u.Orders)
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
                user.Phone = dto.PhoneNumber;

                // Sync profile metrics to UserDetail if it exists
                if (user.UserDetail == null)
                {
                    user.UserDetail = new UserDetail { UserId = user.Id };
                }
                
                user.UserDetail.Bio = dto.Bio;
                user.UserDetail.Location = dto.Location;
                user.UserDetail.School = dto.School;
                user.UserDetail.IndustryFocus = dto.IndustryFocus;
                user.UserDetail.Website = dto.Website;
                user.UserDetail.CompanySize = dto.CompanySize;
                user.UserDetail.FoundedYear = dto.FoundedYear;
                user.UserDetail.GoogleMapsEmbedUrl = dto.GoogleMapsEmbedUrl;
                
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

        [HttpGet("stats")]
        public async Task<IActionResult> GetDashboardStats([FromQuery] string timeRange = "7d")
        {
            try
            {
                var totalUsers = await _context.Users.CountAsync();
                var totalJobs = await _context.Recruitments.CountAsync();
                var totalRevenue = await _context.Orders.Where(o => o.Status == SD.OrderStatus_Paid).SumAsync(o => o.Amount);
                var premiumUsers = await _context.Users.CountAsync(u => u.Subscription != null && u.Subscription.Plan != "free");

                var maxOrderDateDb = await _context.Orders.AnyAsync() 
                    ? await _context.Orders.MaxAsync(o => o.CreatedAt) 
                    : DateTime.UtcNow;
                var today = maxOrderDateDb.Date;
                var chartData = new List<DailyRevenueDTO>();

                if (timeRange == "12m")
                {
                    var last12Months = Enumerable.Range(0, 12).Select(i => new DateTime(today.Year, today.Month, 1).AddMonths(-i)).Reverse().ToList();
                    var minDate = last12Months.First();
                    
                    var monthlyRevenue = await _context.Orders
                        .Where(o => o.Status == SD.OrderStatus_Paid && o.CreatedAt >= minDate)
                        .GroupBy(o => new { o.CreatedAt.Year, o.CreatedAt.Month })
                        .Select(g => new { Year = g.Key.Year, Month = g.Key.Month, Revenue = g.Sum(o => o.Amount) })
                        .ToListAsync();

                    chartData = last12Months.Select(m => new DailyRevenueDTO
                    {
                        Day = m.ToString("MM/yyyy"),
                        Revenue = monthlyRevenue.FirstOrDefault(d => d.Year == m.Year && d.Month == m.Month)?.Revenue ?? 0
                    }).ToList();
                }
                else if (timeRange == "5y")
                {
                    var last5Years = Enumerable.Range(0, 5).Select(i => today.Year - i).Reverse().ToList();
                    var minYear = last5Years.First();

                    var yearlyRevenue = await _context.Orders
                        .Where(o => o.Status == SD.OrderStatus_Paid && o.CreatedAt.Year >= minYear)
                        .GroupBy(o => o.CreatedAt.Year)
                        .Select(g => new { Year = g.Key, Revenue = g.Sum(o => o.Amount) })
                        .ToListAsync();

                    chartData = last5Years.Select(y => new DailyRevenueDTO
                    {
                        Day = y.ToString(),
                        Revenue = yearlyRevenue.FirstOrDefault(d => d.Year == y)?.Revenue ?? 0
                    }).ToList();
                }
                else
                {
                    // Default to days (7d or 30d)
                    int days = timeRange == "30d" ? 30 : 7;
                    var lastXDays = Enumerable.Range(0, days).Select(i => today.AddDays(-i)).Reverse().ToList();
                    
                    var dailyRevenue = await _context.Orders
                        .Where(o => o.Status == SD.OrderStatus_Paid && o.CreatedAt >= today.AddDays(-(days - 1)))
                        .GroupBy(o => o.CreatedAt.Date)
                        .Select(g => new { Day = g.Key, Revenue = g.Sum(o => o.Amount) })
                        .ToListAsync();

                    chartData = lastXDays.Select(day => new DailyRevenueDTO
                    {
                        Day = days > 7 ? day.ToString("dd/MM") : day.ToString("ddd"),
                        Revenue = dailyRevenue.FirstOrDefault(d => d.Day == day)?.Revenue ?? 0
                    }).ToList();
                }

                // Dynamic Growth Metrics (Last 30 days)
                var thirtyDaysAgo = DateTime.UtcNow.Date.AddDays(-30);
                var recentUsers = await _context.Users.CountAsync(u => u.CreatedAt >= thirtyDaysAgo);
                var recentJobs = await _context.Recruitments.CountAsync(r => r.CreatedAt >= thirtyDaysAgo);

                var last30DaysRevenue = await _context.Orders.Where(o => o.Status == SD.OrderStatus_Paid && o.CreatedAt >= thirtyDaysAgo).SumAsync(o => o.Amount);
                var prev30DaysRevenue = await _context.Orders.Where(o => o.Status == SD.OrderStatus_Paid && o.CreatedAt >= thirtyDaysAgo.AddDays(-30) && o.CreatedAt < thirtyDaysAgo).SumAsync(o => o.Amount);
                var revenueGrowth = prev30DaysRevenue == 0 ? (last30DaysRevenue > 0 ? 100 : 0) : (int)((last30DaysRevenue - prev30DaysRevenue) / prev30DaysRevenue * 100);

                var recentPremium = await _context.Users.CountAsync(u => u.Subscription != null && u.Subscription.Plan != "free" && u.Subscription.StartAt >= thirtyDaysAgo);
                var prevPremium = await _context.Users.CountAsync(u => u.Subscription != null && u.Subscription.Plan != "free" && u.Subscription.StartAt >= thirtyDaysAgo.AddDays(-30) && u.Subscription.StartAt < thirtyDaysAgo);
                var premiumGrowth = prevPremium == 0 ? (recentPremium > 0 ? 100 : 0) : (int)((double)(recentPremium - prevPremium) / prevPremium * 100);

                var stats = new DashboardStatsDTO
                {
                    TotalRevenue = totalRevenue,
                    TotalUsers = totalUsers,
                    TotalJobs = totalJobs,
                    TotalPremiumUsers = premiumUsers,
                    RevenueChartData = chartData,
                    RevenueGrowthPercentage = revenueGrowth,
                    UserGrowthCount = recentUsers,
                    JobGrowthCount = recentJobs,
                    PremiumGrowthPercentage = premiumGrowth
                };

                return Ok(ApiResponse<DashboardStatsDTO>.Ok(stats, "Stats retrieved successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.Error(500, "Failed to retrieve stats", ex.Message));
            }
        }

        [HttpGet("orders")]
        public async Task<IActionResult> GetAllOrders()
        {
            try
            {
                var orders = await _context.Orders
                    .Include(o => o.User)
                    .ThenInclude(u => u.Subscription)
                    .OrderByDescending(o => o.CreatedAt)
                    .ToListAsync();

                var orderDTOs = _mapper.Map<IEnumerable<AdminOrderDTO>>(orders);
                return Ok(ApiResponse<IEnumerable<AdminOrderDTO>>.Ok(orderDTOs, "Orders retrieved successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.Error(500, "Failed to retrieve orders", ex.Message));
            }
        }

        [HttpGet("posts")]
        public async Task<IActionResult> GetAllPosts()
        {
            try
            {
                var posts = await _context.Posts
                    .Include(p => p.User)
                    .Include(p => p.PostRecruitments)
                    .ThenInclude(pr => pr.Recruitment)
                    .OrderByDescending(p => p.CreatedAt)
                    .ToListAsync();

                var postDTOs = _mapper.Map<IEnumerable<AdminPostDTO>>(posts);
                return Ok(ApiResponse<IEnumerable<AdminPostDTO>>.Ok(postDTOs, "Posts retrieved successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.Error(500, "Failed to retrieve posts", ex.Message));
            }
        }

        [HttpGet("categories")]
        public async Task<IActionResult> GetAllCategories()
        {
            try
            {
                var categories = await _context.Categories
                    .Include(c => c.Recruitments)
                    .ToListAsync();

                var results = categories.Select(c => new {
                    id = c.Id,
                    name = c.Name,
                    count = c.Recruitments.Count
                });

                return Ok(ApiResponse<object>.Ok(results, "Categories retrieved successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.Error(500, "Failed to retrieve categories", ex.Message));
            }
        }

        [HttpGet("jobtypes")]
        public async Task<IActionResult> GetAllJobTypes()
        {
            try
            {
                var jobTypes = await _context.JobTypes
                    .Include(j => j.Recruitments)
                    .ToListAsync();

                var results = jobTypes.Select(j => new {
                    id = j.Id,
                    name = j.Name,
                    count = j.Recruitments.Count
                });

                return Ok(ApiResponse<object>.Ok(results, "Job types retrieved successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.Error(500, "Failed to retrieve job types", ex.Message));
            }
        }

        [HttpPost("categories")]
        public async Task<IActionResult> CreateCategory([FromBody] CategoryDTO dto)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(dto.Name)) return BadRequest(ApiResponse<object>.BadRequest("Name is required"));
                var category = new Category { Name = dto.Name };
                _context.Categories.Add(category);
                await _context.SaveChangesAsync();
                return Ok(ApiResponse<object>.Ok(new { id = category.Id, name = category.Name, count = 0 }, "Category created successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.Error(500, "Failed to create category", ex.Message));
            }
        }

        [HttpPut("categories/{id}")]
        public async Task<IActionResult> UpdateCategory(int id, [FromBody] CategoryDTO dto)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(dto.Name)) return BadRequest(ApiResponse<object>.BadRequest("Name is required"));
                var category = await _context.Categories.FindAsync(id);
                if (category == null) return NotFound(ApiResponse<object>.NotFound("Category not found"));
                category.Name = dto.Name;
                await _context.SaveChangesAsync();
                return Ok(ApiResponse<object>.Ok(new { id = category.Id, name = category.Name }, "Category updated successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.Error(500, "Failed to update category", ex.Message));
            }
        }

        [HttpPost("jobtypes")]
        public async Task<IActionResult> CreateJobType([FromBody] JobTypeDTO dto)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(dto.Name)) return BadRequest(ApiResponse<object>.BadRequest("Name is required"));
                var jobType = new JobType { Name = dto.Name };
                _context.JobTypes.Add(jobType);
                await _context.SaveChangesAsync();
                return Ok(ApiResponse<object>.Ok(new { id = jobType.Id, name = jobType.Name, count = 0 }, "JobType created successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.Error(500, "Failed to create job type", ex.Message));
            }
        }

        [HttpPut("jobtypes/{id}")]
        public async Task<IActionResult> UpdateJobType(int id, [FromBody] JobTypeDTO dto)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(dto.Name)) return BadRequest(ApiResponse<object>.BadRequest("Name is required"));
                var jobType = await _context.JobTypes.FindAsync(id);
                if (jobType == null) return NotFound(ApiResponse<object>.NotFound("JobType not found"));
                jobType.Name = dto.Name;
                await _context.SaveChangesAsync();
                return Ok(ApiResponse<object>.Ok(new { id = jobType.Id, name = jobType.Name }, "JobType updated successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.Error(500, "Failed to update job type", ex.Message));
            }
        }

        [HttpGet("reports")]
        public async Task<IActionResult> GetAllReports()
        {
            try
            {
                // No reports table yet, returning empty list
                return Ok(ApiResponse<IEnumerable<object>>.Ok(new List<object>(), "Reports retrieved successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.Error(500, "Failed to retrieve reports", ex.Message));
            }
        }
    }
}
