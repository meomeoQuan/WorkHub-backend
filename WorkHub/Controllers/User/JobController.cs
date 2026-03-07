using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WorkHub.DataAccess.Repository.IRepository;
using WorkHub.Models.DTOs;
using WorkHub.Models.DTOs.ModelDTOs;
using WorkHub.Models.DTOs.ModelDTOs.JobDTOs;
using WorkHub.Models.DTOs.ModelDTOs.JobPostDTOs;
using WorkHub.Models.Models;
using WorkHub.Utility;

namespace WorkHub.Controllers.User
{
    [Route("api/[controller]")]
    [ApiController]
    public class JobController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public JobController(IUnitOfWork unitOfWork,IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }


        [HttpGet("get-jobtypes")]
        public async Task<IActionResult> GetJobTypes()
        {
            var jobTypes = await _unitOfWork.JobTypeRepo.GetAllAsync();

            var result = _mapper.Map<List<JobTypeDTO>>(jobTypes);

            return Ok(ApiResponse<object>.Ok(result,"retrieve Jobtype success"));
        }

        [HttpGet("get-categories")]
        public async Task<IActionResult> GetCategories()
        {
            var categories = await _unitOfWork.JobCategoryRepo.GetAllAsync();

            var result = _mapper.Map<List<CategoryDTO>>(categories);

            return Ok(ApiResponse<object>.Ok(result, "retrieve Jobcategory success"));
        }

        [HttpGet("get-cities")]
        public async Task<IActionResult> GetCities()
        {
            var cities = await _unitOfWork.CityRepo.GetAllAsync();

            var result = _mapper.Map<List<CityDTO>>(cities.OrderBy(c => c.Id == 1 ? 0 : 1).ThenBy(c => c.Name));

            return Ok(ApiResponse<object>.Ok(result, "retrieve Cities success"));
        }

        [Authorize]
        [HttpPost("create-job")]
        public async Task<IActionResult> CreateJob([FromForm] CreateJobRequestDTO createJobRequest)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var user = await _unitOfWork.UserRepository.GetAsync(
                u => u.Id == userId, 
                includeProperties: SD.Join_Subscription
            );

            if (user == null)
            {
                return NotFound(ApiResponse<object>.NotFound("User not found"));
            }

            // Enforce Subscription Limits
            var subscription = user.Subscription;
            var plan = (subscription != null && subscription.IsActive) ? subscription.Plan : SD.Plan_Free;
            if (plan != SD.Plan_Gold)
            {

                var cycleStart = SD.CalculateCycleStart(user.Subscription?.StartAt ?? user.CreatedAt);
                
                var postCount = await _unitOfWork.PostRepository.CountAsync(r => 
                    r.UserId == userId && 
                    r.CreatedAt >= cycleStart
                );

                if (plan == SD.Plan_Free && postCount >= SD.Free_Post_Limit)
                {
                    return BadRequest(ApiResponse<object>.BadRequest("Bạn đã đạt giới hạn đăng bài của gói Miễn Phí. Làm ơn hãy nâng cấp gói của bạn !"));
                }
                
                if (plan == SD.Plan_Silver && postCount >= SD.Silver_Post_Limit)
                {
                    return BadRequest(ApiResponse<object>.BadRequest("Bạn đã đạt giới hạn đăng bài của gói Bạc. Làm ơn hãy nâng cấp gói của bạn !"));
                }
            }

            

            // 3. Map DTO to Recruitment and Link to Post
            var recruitment = _mapper.Map<Recruitment>(createJobRequest);
            recruitment.UserId = userId;
            recruitment.Status = "Open";
            recruitment.CreatedAt = DateTime.UtcNow;

            if (createJobRequest.CreatePost)
            {
                // Create the Post first
                var post = new Post
                {
                    UserId = userId,
                    Content = createJobRequest.Description,
                    CreatedAt = DateTime.UtcNow
                };
                _unitOfWork.PostRepository.Add(post);
                await _unitOfWork.SaveAsync(); // Get the Post ID

                // Link via join table - use navigation properties to handle IDs automatically
                _unitOfWork.PostRecruitmentRepository.Add(new PostRecruitment 
                { 
                    Post = post, 
                    Recruitment = recruitment 
                });
            }
            else
            {
                // Just add the recruitment without a post link
                _unitOfWork.RecruitmentInfoRepo.Add(recruitment);
            }

            // Manual Category Mapping
            if (!string.IsNullOrEmpty(createJobRequest.Category))
            {
                if (int.TryParse(createJobRequest.Category, out int catId))
                {
                    recruitment.CategoryId = catId;
                }
                else
                {
                    var category = await _unitOfWork.JobCategoryRepo.GetAsync(c => c.Name == createJobRequest.Category);
                    if (category != null)
                    {
                        recruitment.CategoryId = category.Id;
                    }
                    else
                    {
                        return BadRequest(ApiResponse<object>.BadRequest(null, $"Category '{createJobRequest.Category}' not found."));
                    }
                }
            }

            // Manual JobType Mapping
            if (!string.IsNullOrEmpty(createJobRequest.JobType))
            {
                if (int.TryParse(createJobRequest.JobType, out int typeId))
                {
                    recruitment.JobTypeId = typeId;
                }
                else
                {
                    var jobType = await _unitOfWork.JobTypeRepo.GetAsync(t => t.Name == createJobRequest.JobType);
                    if (jobType != null) recruitment.JobTypeId = jobType.Id;
                }
            }

            // Manual City Mapping
            if (!string.IsNullOrEmpty(createJobRequest.Location))
            {
                var city = await _unitOfWork.CityRepo.GetAsync(c => c.Name == createJobRequest.Location);
                if (city != null)
                {
                    recruitment.CityId = city.Id;
                }
            }

            _unitOfWork.RecruitmentInfoRepo.Add(recruitment);
            await _unitOfWork.SaveAsync();

            return Ok(ApiResponse<object>.Ok(null, "Create job successfully"));
        }

        [Authorize]
        [HttpGet("get-job/{id}")]
        public async Task<IActionResult> GetJob(int id)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var job = await _unitOfWork.RecruitmentInfoRepo.GetAsync(
                r => r.Id == id && r.UserId == userId,
                includeProperties: "JobType,Category,City," + SD.Collection_Join_PostRecruitments + ".Post"
            );

            if (job == null)
            {
                return NotFound(ApiResponse<object>.NotFound("Job not found or unauthorized"));
            }

            var result = _mapper.Map<JobDTO>(job);
            return Ok(ApiResponse<object>.Ok(result, "Job retrieved successfully"));
        }

        [Authorize]
        [HttpPut("update-job/{id}")]
        public async Task<IActionResult> UpdateJob(int id, [FromForm] CreateJobRequestDTO updateJobRequest)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var existingJob = await _unitOfWork.RecruitmentInfoRepo.GetAsync(
                r => r.Id == id && r.UserId == userId,
                includeProperties: SD.Collection_Join_PostRecruitments + ".Post"
            );

            if (existingJob == null)
            {
                return NotFound(ApiResponse<object>.NotFound("Job not found or unauthorized"));
            }

            // Map updates
            _mapper.Map(updateJobRequest, existingJob);
            
            // Manual Category Mapping (similar to Create)
            if (!string.IsNullOrEmpty(updateJobRequest.Category))
            {
                if (int.TryParse(updateJobRequest.Category, out int catId))
                {
                    existingJob.CategoryId = catId;
                }
                else
                {
                    var category = await _unitOfWork.JobCategoryRepo.GetAsync(c => c.Name == updateJobRequest.Category);
                    if (category != null) existingJob.CategoryId = category.Id;
                }
            }

            // Manual JobType Mapping
            if (!string.IsNullOrEmpty(updateJobRequest.JobType))
            {
                if (int.TryParse(updateJobRequest.JobType, out int typeId))
                {
                    existingJob.JobTypeId = typeId;
                }
                else
                {
                    var jobType = await _unitOfWork.JobTypeRepo.GetAsync(t => t.Name == updateJobRequest.JobType);
                    if (jobType != null) existingJob.JobTypeId = jobType.Id;
                }
            }

            // Manual City Mapping
            if (!string.IsNullOrEmpty(updateJobRequest.Location))
            {
                var city = await _unitOfWork.CityRepo.GetAsync(c => c.Name == updateJobRequest.Location);
                if (city != null) existingJob.CityId = city.Id;
            }

            // Manual Description (Post Content) Mapping - update the first linked post
            var firstPost = existingJob.PostRecruitments.FirstOrDefault()?.Post;
            if (firstPost != null && !string.IsNullOrEmpty(updateJobRequest.Description))
            {
                firstPost.Content = updateJobRequest.Description;
            }

            _unitOfWork.RecruitmentInfoRepo.Update(existingJob);
            await _unitOfWork.SaveAsync();

            return Ok(ApiResponse<object>.Ok(null, "Update job successfully"));
        }

        [Authorize]
        [HttpDelete("delete-job/{id}")]
        public async Task<IActionResult> DeleteJob(int id)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var job = await _unitOfWork.RecruitmentInfoRepo.GetAsync(r => r.Id == id && r.UserId == userId);

            if (job == null)
            {
                return NotFound(ApiResponse<object>.NotFound("Job not found or unauthorized"));
            }

            // Also remove from any posts it's attached to (handled by DB or explicit unlink)
            // If Recruitment has PostId, it will be removed
            _unitOfWork.RecruitmentInfoRepo.Remove(job);
            await _unitOfWork.SaveAsync();

            return Ok(ApiResponse<object>.Ok(null, "Delete job successfully"));
        }
    }
}
