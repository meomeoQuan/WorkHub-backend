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
                return NotFound(ApiResponse<object>.BadRequest(null, "User not found"));
            }

            // Enforce Subscription Limits
            var plan = user.Subscription?.Plan ?? SD.Plan_Free;
            if (plan != SD.Plan_Gold)
            {
                var currentMonth = DateTime.Now.Month;
                var currentYear = DateTime.Now.Year;
                
                var postCount = await _unitOfWork.RecruitmentInfoRepo.CountAsync(r => 
                    r.UserId == userId && 
                    r.CreatedAt.Value.Month == currentMonth && 
                    r.CreatedAt.Value.Year == currentYear
                );

                if (plan == SD.Plan_Free && postCount >= SD.Free_Post_Limit)
                {
                    return BadRequest(ApiResponse<object>.BadRequest(null, $"Bạn đã đạt giới hạn đăng bài của gói Miễn Phí ({SD.Free_Post_Limit} bài/tháng). Vui lòng nâng cấp để tiếp tục."));
                }
                
                if (plan == SD.Plan_Silver && postCount >= SD.Silver_Post_Limit)
                {
                    return BadRequest(ApiResponse<object>.BadRequest(null, $"Bạn đã đạt giới hạn đăng bài của gói Silver ({SD.Silver_Post_Limit} bài/tháng). Vui lòng nâng cấp lên Gold để đăng không giới hạn."));
                }
            }

            

            // 3. Map DTO to Recruitment and Link to Post
            var recruitment = _mapper.Map<Recruitment>(createJobRequest);
            recruitment.UserId = userId;
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

    }
}
