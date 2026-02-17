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

        [Authorize]
        [HttpPost("create-job")]
        public async Task<IActionResult> CreateJob([FromForm] CreateJobRequestDTO createJobRequest)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var user = await _unitOfWork.UserRepository.GetAsync(u => u.Id == userId);

            if (user == null)
            {
                return NotFound(ApiResponse<object>.BadRequest(null, "User not found"));
            }

            // 1. Create and Save Post (Required for Recruitment)
            var post = new Post
            {
                UserId = userId,
                Content = createJobRequest.JobDescription,
                CreatedAt = DateTime.Now,
                PostImageUrl = null // TODO: Handle Image Upload if needed
            };

            _unitOfWork.PostRepository.Add(post);
            await _unitOfWork.SaveAsync(); // Save to generate Post.Id

            // 2. Map DTO to Recruitment and Link to Post
            var recruitment = _mapper.Map<Recruitment>(createJobRequest);
            recruitment.UserId = userId;
            recruitment.PostId = post.Id; // Link to the newly created Post

            // Manual Category Mapping (Handle "IT" string or "1" int string)
            if (!string.IsNullOrEmpty(createJobRequest.Category))
            {
                if (int.TryParse(createJobRequest.Category, out int catId))
                {
                    recruitment.CategoryId = catId;
                }
                else
                {
                    // Look up by Name
                    var category = await _unitOfWork.JobCategoryRepo.GetAsync(c => c.Name == createJobRequest.Category);
                    if (category != null)
                    {
                        recruitment.CategoryId = category.Id;
                    }
                    else
                    {
                         // Handle case where category name not found (Optional: Create or Default)
                         // For now, let it fail FK if 0 or maybe set to a default if exists.
                         // But better to just log or let it be 0 and catch FK error.
                         // Or return BadRequest
                         return BadRequest(ApiResponse<object>.BadRequest(null, $"Category '{createJobRequest.Category}' not found."));
                    }
                }
            }

             _unitOfWork.RecruitmentInfoRepo.Add(recruitment);
            await _unitOfWork.SaveAsync();

            return Ok(ApiResponse<object>.Ok(null, "Create job successfully"));
        }

    }
}
