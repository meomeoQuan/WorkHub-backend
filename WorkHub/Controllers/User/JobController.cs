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

            // 1. Handle Image Uploads
            string? firstImagePath = null;
            if (createJobRequest.JobImages != null && createJobRequest.JobImages.Count > 0)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "jobs");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                foreach (var file in createJobRequest.JobImages)
                {
                    var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
                    var filePath = Path.Combine(uploadsFolder, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    if (firstImagePath == null)
                    {
                        firstImagePath = $"/uploads/jobs/{fileName}";
                    }
                }
            }

            // 2. Create and Save Post (Required for Recruitment)
            var post = new Post
            {
                UserId = userId,
                Content = createJobRequest.JobDescription,
                CreatedAt = DateTime.Now,
                PostImageUrl = firstImagePath
            };

            _unitOfWork.PostRepository.Add(post);
            await _unitOfWork.SaveAsync(); // Save to generate Post.Id

            // 3. Map DTO to Recruitment and Link to Post
            var recruitment = _mapper.Map<Recruitment>(createJobRequest);
            recruitment.UserId = userId;
            recruitment.PostId = post.Id;
            recruitment.Status = "Open";
            recruitment.CreatedAt = DateTime.Now;

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

            _unitOfWork.RecruitmentInfoRepo.Add(recruitment);
            await _unitOfWork.SaveAsync();

            return Ok(ApiResponse<object>.Ok(null, "Create job successfully"));
        }

    }
}
