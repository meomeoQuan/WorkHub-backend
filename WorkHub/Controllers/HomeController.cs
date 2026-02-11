using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WorkHub.DataAccess.Data;
using WorkHub.DataAccess.Repository.IRepository;
using WorkHub.Models.DTOs;
using WorkHub.Models.DTOs.ModelDTOs;

namespace WorkHub.Controllers
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

            var entities = await _unitOfWork.RecruitmentInfoRepo.GetTopAsync(4,descending: true); // descending is latest first
            var result = _mapper.Map<List<RecruitmentOverviewInfoDTO>>(entities);

            var response = ApiResponse<List<RecruitmentOverviewInfoDTO>>.Ok(result, "Top 5 recruitment info retrieved successfully");

            return Ok(response);
        }


        [HttpPost]
        public async Task<IActionResult> JobDetails(string jobId)
        {
            var entity = await _unitOfWork.RecruitmentInfoRepo.GetAsync(r => r.Id.ToString() == jobId, includeProperties: "Company,Employer");

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
            var entities = await _unitOfWork.RecruitmentInfoRepo.GetAllPagedAsync(pageIndex: 1 , pageSize: 5); // descending is latest first
            var result = _mapper.Map<List<RecruitmentOverviewInfoDTO>>(entities);

            var response = ApiResponse<List<RecruitmentOverviewInfoDTO>>.Ok(result, "Top 5 recruitment info retrieved successfully");

            return Ok(response);
        }


        
    }
}
