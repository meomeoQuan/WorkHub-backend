using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WorkHub.DataAccess.Data;
using WorkHub.DataAccess.Repository.IRepository;
using WorkHub.Models.DTOs;
using WorkHub.Models.DTOs.ModelDTOs;
using WorkHub.Models.DTOs.ModelDTOs.HomeDTOs;
using WorkHub.Utility;

namespace WorkHub.Controllers.User
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public HomeController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        [HttpGet("top4")]
        public async Task<IActionResult> GetTop4() { 

            var entities = await _unitOfWork.RecruitmentInfoRepo.GetTopAsync(4, orderBy: r => r.CreatedAt, descending: true, includeProperties: SD.Join_Post + "," + SD.Join_User); // descending is latest first
            var result = _mapper.Map<List<RecruitmentOverviewInfoDTO>>(entities);

            var response = ApiResponse<List<RecruitmentOverviewInfoDTO>>.Ok(result, "Top 4 recruitment info retrieved successfully");

            return Ok(response);
        }


        // Get top 6 users with credibility rating >= 4 who have been registered for at least 7 days
        [HttpGet("top-credibility-user")]
        public async Task<IActionResult> GetTopCredibilityUser()
        {
            var sevenDaysAgo = DateTime.UtcNow.AddDays(-7);
            var adminRole = RoleMapper.MapRoleToRoleNumber(SD.Role_Admin);

            var entities = await _unitOfWork.UserRepository.GetTopAsync(
                4,
                filter: c =>
                    c.UserDetail != null &&
                    c.UserDetail.Rating >= 4 &&
                    c.CreatedAt <= sevenDaysAgo &&
                    c.Role != adminRole,
                orderBy: c => c.UserDetail!.Rating,
                descending: true,
                includeProperties: SD.Join_UserDetail + "," + SD.Collection_Join_Recruitments
            );

            var result = _mapper.Map<List<UserFeatureDTO>>(entities);

            var response = ApiResponse<List<UserFeatureDTO>>.Ok(result, "Top 4 credibility users retrieved successfully");

            return Ok(response);
        }





        [HttpGet("job-detail/{jobId}")]
        public async Task<IActionResult> JobDetails(string jobId)
        {
            var entity = await _unitOfWork.RecruitmentInfoRepo.GetAsync(r => r.Id.ToString() == jobId, includeProperties: SD.Join_User + "," + SD.Join_Post + ",JobType,Category," + SD.Join_User + "." + SD.Join_UserDetail);

            if (entity == null)
            {
                return NotFound(ApiResponse<string>.BadRequest("Job not found"));
            }

            var result = _mapper.Map<RecruitmentDetailInfoDTO>(entity);

            var response = ApiResponse<RecruitmentDetailInfoDTO>.Ok(result, "Job details retrieved successfully");

            return Ok(response);
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllJob()
        {
            var entities = await _unitOfWork.RecruitmentInfoRepo.GetAllPagedAsync(pageIndex: 1, pageSize: 5, orderBy: r => r.CreatedAt, descending: true); // descending is latest first
            var result = _mapper.Map<List<RecruitmentOverviewInfoDTO>>(entities);

            var response = ApiResponse<List<RecruitmentOverviewInfoDTO>>.Ok(result, "Top 5 recruitment info retrieved successfully");

            return Ok(response);
        }




    }
}
