using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;
using System.Security.Claims;
using WorkHub.DataAccess.Repository.IRepository;
using WorkHub.Models.DTOs;
using WorkHub.Models.DTOs.ModelDTOs.ApplicationDTOs;
using WorkHub.Models.Models;
using WorkHub.Utility;

namespace WorkHub.Controllers.User
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApplicationController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ApplicationController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetApplications([FromQuery] ApplicationFilterDTO filter)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
                Expression<Func<Application, bool>> filterExpression = a => a.Recruitment.UserId == userId;

                // 1. Filter by Status
                if (!string.IsNullOrEmpty(filter.Status) && filter.Status != "All Status")
                {
                    filterExpression = filterExpression.And(a => a.Status == filter.Status);
                }

                // 2. Filter by Job (RecruitmentId)
                if (filter.JobId.HasValue && filter.JobId.Value != 0)
                {
                    filterExpression = filterExpression.And(a => a.RecruitmentId == filter.JobId.Value);
                }

                // 3. Search by Name or Email
                if (!string.IsNullOrEmpty(filter.SearchTerm))
                {
                    var term = filter.SearchTerm.ToLower();
                    filterExpression = filterExpression.And(a => a.User.FullName.Contains(term) || a.User.Email.Contains(term));
                }

                // Get All with Includes
                // Explicitly include User, UserDetail (for Avatar), and Recruitment (for Job Title)
                var applications = await _unitOfWork.ApplicationRepository.GetAllAsync(
                    filter: filterExpression,
                    includeProperties: "User,User.UserDetail,Recruitment"
                );

                var applicationDTOs = _mapper.Map<IEnumerable<ApplicationDTO>>(applications);

                return Ok(ApiResponse<IEnumerable<ApplicationDTO>>.Ok(applicationDTOs, "Applications retrieved successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.Error(500, $"Internal server error: {ex.Message}"));
            }
        }

        [HttpGet("summary")]
        public async Task<IActionResult> GetApplicationSummary()
        {
            try
            {
                var summary = new ApplicationSummaryDTO
                {
                    TotalApplications = await _unitOfWork.ApplicationRepository.CountAsync(),
                    New = await _unitOfWork.ApplicationRepository.CountAsync(a => a.Status == ApplicationStatus.New),
                    Reviewing = await _unitOfWork.ApplicationRepository.CountAsync(a => a.Status == ApplicationStatus.Reviewing),
                    Shortlisted = await _unitOfWork.ApplicationRepository.CountAsync(a => a.Status == ApplicationStatus.Shortlisted),
                    Interviewed = await _unitOfWork.ApplicationRepository.CountAsync(a => a.Status == ApplicationStatus.Interviewed)
                };

                return Ok(ApiResponse<ApplicationSummaryDTO>.Ok(summary, "Application summary retrieved successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.Error(500, $"Internal server error: {ex.Message}"));
            }
        }

        [Authorize]
        [HttpGet("get-jobs")]
        public async Task<IActionResult> GetJobNames()
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

                var jobs = await _unitOfWork.RecruitmentInfoRepo.GetAllAsync(
                    filter: r => r.UserId == userId
                );

                var jobDTOs = _mapper.Map<IEnumerable<JobNameDTO>>(jobs);

                return Ok(ApiResponse<IEnumerable<JobNameDTO>>.Ok(jobDTOs, "Job names retrieved successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.Error(500, $"Internal server error: {ex.Message}"));
            }
        }

        [HttpGet("get-statuses")]
        public IActionResult GetApplicationStatuses()
        {
            var statuses = new List<string>
            {
                ApplicationStatus.New,
                ApplicationStatus.Reviewing,
                ApplicationStatus.Shortlisted,
                ApplicationStatus.Interviewed,
                ApplicationStatus.Rejected,
                ApplicationStatus.Accepted
            };

            return Ok(ApiResponse<List<string>>.Ok(statuses, "Application statuses retrieved successfully"));
        }
    }
}

    // Helper extension methods for Expression combining if not already present in Utility

