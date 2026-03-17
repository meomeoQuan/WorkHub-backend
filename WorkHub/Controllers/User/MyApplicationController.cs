using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;
using System.Security.Claims;
using WorkHub.DataAccess.Repository.IRepository;
using WorkHub.Models.DTOs;
using WorkHub.Models.DTOs.ModelDTOs.ApplicationDetailDTOs;
using WorkHub.Models.DTOs.ModelDTOs.MyApplicationDTOs;
using WorkHub.Models.Models;
using WorkHub.Utility;
using WorkHub.Business.Service.IService;
using System.IO;

namespace WorkHub.Controllers.Users
{
    [Route("api/[controller]")]
    [ApiController]
    public class MyApplicationController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IMediaService _mediaService;

        public MyApplicationController(IUnitOfWork unitOfWork, IMapper mapper, IMediaService mediaService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _mediaService = mediaService;
        }

        [HttpGet("my-application-summary")]
        [Authorize]
        public async Task<IActionResult> GetMyApplicationSummary()
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

                // Get all applications for this user
                var applications = await _unitOfWork.ApplicationRepository.GetAllAsync(a => a.UserId == userId);
                
                var summary = new MyApplicationSummaryDTO
                {
                    TotalApplications = applications.Count(),
                    Pending = applications.Count(a => a.Status == ApplicationStatus.New),
                    UnderReview = applications.Count(a => a.Status == ApplicationStatus.Reviewing),
                    Accepted = applications.Count(a => a.Status == ApplicationStatus.Accepted),
                    Rejected = applications.Count(a => a.Status == ApplicationStatus.Rejected)
                };

                return Ok(ApiResponse<MyApplicationSummaryDTO>.Ok(summary, "My application summary retrieved successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.Error(500, $"Internal server error: {ex.Message}"));
            }
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetMyApplications([FromQuery] MyApplicationFilterDTO filter)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

                Expression<Func<Application, bool>> filterExpression = a => a.UserId == userId;

                // 1. Search by Job Name or Company (Employer Name)
                if (!string.IsNullOrEmpty(filter.SearchTerm))
                {
                    var term = filter.SearchTerm.ToLower();
                    filterExpression = filterExpression.And(a => a.Recruitment.JobName.Contains(term) || 
                                                                 a.Recruitment.User.FullName.Contains(term));
                }

                // 2. Filter by Status (UI Status -> Backend Status)
                if (!string.IsNullOrEmpty(filter.Status) && filter.Status != "All Status")
                {
                    if (filter.Status == "Pending")
                    {
                        filterExpression = filterExpression.And(a => a.Status == ApplicationStatus.New);
                    }
                    else if (filter.Status == "Under Review")
                    {
                         filterExpression = filterExpression.And(a => a.Status == ApplicationStatus.Reviewing);
                    }
                    else if (filter.Status == "Accepted")
                    {
                        filterExpression = filterExpression.And(a => a.Status == ApplicationStatus.Accepted);
                    }
                    else if (filter.Status == "Rejected")
                    {
                        filterExpression = filterExpression.And(a => a.Status == ApplicationStatus.Rejected);
                    }
                }

                var applications = await _unitOfWork.ApplicationRepository.GetAllAsync(
                    filter: filterExpression,
                    includeProperties: "Recruitment,Recruitment.User,Recruitment.User.UserDetail,Recruitment.JobType"
                );

                var myApplicationDTOs = _mapper.Map<IEnumerable<MyApplicationDTO>>(applications);

                return Ok(ApiResponse<IEnumerable<MyApplicationDTO>>.Ok(myApplicationDTOs, "My applications retrieved successfully"));
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
                "Pending",
                "Under Review",
                "Accepted",
                "Rejected"
            };

            return Ok(ApiResponse<List<string>>.Ok(statuses, "Application statuses retrieved successfully"));
        }
        [HttpPost("apply")]
        [Authorize]
        public async Task<IActionResult> Apply([FromForm] SubmitApplicationDTO applicationDTO)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

                // 1. Check if user already applied
                var existingApplication = await _unitOfWork.ApplicationRepository.GetAsync(
                    a => a.UserId == userId && a.RecruitmentId == applicationDTO.RecruitmentId
                );



                // 0. Check if Recruitment exists
                var recruitment = await _unitOfWork.RecruitmentInfoRepo.GetAsync(r => r.Id == applicationDTO.RecruitmentId);
                if (recruitment == null)
                {
                    return NotFound(ApiResponse<object>.Error(404, "Job post not found."));
                }

                if (recruitment.UserId == userId)
                {
                   return BadRequest(ApiResponse<object>.BadRequest(null, "You cannot apply to a job you posted."));
                }

                if (existingApplication != null)
                {
                    return BadRequest(ApiResponse<object>.BadRequest(null, "You have already applied for this job."));
                }

                // 2. Handle File Upload (CV)
                string? cvUrl = null;
                if (applicationDTO.CvFile != null)
                {
                    if (applicationDTO.CvFile.Length > 10 * 1024 * 1024) // 10MB Limit
                    {
                        return BadRequest(ApiResponse<object>.BadRequest(null, "CV file size exceeds the 10MB limit."));
                    }

                    // Upload to Cloudinary
                    cvUrl = await _mediaService.UploadFileAsync(applicationDTO.CvFile, "cvs");
                    if (string.IsNullOrEmpty(cvUrl))
                    {
                        return StatusCode(500, ApiResponse<object>.Error(500, "Failed to upload CV to third-party storage"));
                    }
                }
                else if (!string.IsNullOrEmpty(applicationDTO.ProfileCvUrl))
                {
                    // Use the profile CV URL directly (already uploaded to Cloudinary)
                    cvUrl = applicationDTO.ProfileCvUrl;
                }

                var user = await _unitOfWork.UserRepository.GetAsync(
                    u => u.Id == userId, 
                    includeProperties: $"{SD.Join_UserDetail},{SD.Join_Subscription}"
                );

                // Enforce Application Limits for Free Plan
                var subscription = user.Subscription;
                var plan = (subscription != null && subscription.IsActive) ? subscription.Plan : SD.Plan_Free;
                if (plan == SD.Plan_Free)
                {
                    var cycleStart = SD.CalculateCycleStart(user.Subscription?.StartAt ?? user.CreatedAt);

                    var applyCount = await _unitOfWork.ApplicationRepository.CountAsync(a => 
                        a.UserId == userId && 
                        a.CreatedAt >= cycleStart
                    );

                    if (applyCount >= SD.Free_Apply_Limit)
                    {
                        return BadRequest(ApiResponse<object>.BadRequest("you are exceed amount your plan , please upgrade plan ."));
                    }
                }

                bool profileUpdated = false;

                // Update Phone if missing
                if (string.IsNullOrEmpty(user.Phone) && !string.IsNullOrEmpty(applicationDTO.Phone))
                {
                    user.Phone = applicationDTO.Phone;
                    profileUpdated = true;
                }

                // Update Education if missing
                if (user.UserDetail != null && string.IsNullOrEmpty(user.UserDetail.EducationLevel) && !string.IsNullOrEmpty(applicationDTO.Education))
                {
                    user.UserDetail.EducationLevel = applicationDTO.Education;
                    profileUpdated = true;
                }

                // Sync CV to profile if user doesn't have one yet
                if (!string.IsNullOrEmpty(cvUrl) && user.UserDetail != null && string.IsNullOrEmpty(user.UserDetail.CvUrl))
                {
                    user.UserDetail.CvUrl = cvUrl;
                    profileUpdated = true;
                }

                if (profileUpdated)
                {
                    await _unitOfWork.SaveAsync();
                }

                // 3. Create Application Entity
                var application = new Application
                {
                    UserId = userId,
                    RecruitmentId = applicationDTO.RecruitmentId,
                    Status = ApplicationStatus.New, // "Pending" for applicant
                    CoverLetter = applicationDTO.CoverLetter,
                    CvUrl = cvUrl,
                    CreatedAt = DateTime.UtcNow
                };

                _unitOfWork.ApplicationRepository.Add(application);
                await _unitOfWork.SaveAsync();

                return Ok(ApiResponse<object>.Ok(null, "Application submitted successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.Error(500, $"Internal server error: {ex.Message}"));
            }
        }
    }
}
