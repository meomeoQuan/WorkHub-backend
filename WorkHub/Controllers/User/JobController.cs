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
            var categories = await _unitOfWork.CommentRepository.GetAllAsync();

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

            // ✅ DTO → Recruitment
            var recruitment = _mapper.Map<Recruitment>(createJobRequest);
            recruitment.UserId = userId;

             _unitOfWork.RecruitmentInfoRepo.Add(recruitment);
            await _unitOfWork.SaveAsync();

            return Ok(ApiResponse<object>.Ok(null, "Create job successfully"));
        }

    }
}
