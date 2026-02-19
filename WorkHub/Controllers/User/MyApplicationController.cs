using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;
using System.Security.Claims;
using WorkHub.DataAccess.Repository.IRepository;
using WorkHub.Models.DTOs;
using WorkHub.Models.DTOs.ModelDTOs.MyApplicationDTOs;
using WorkHub.Models.Models;
using WorkHub.Utility;
using System.IO;

namespace WorkHub.Controllers.User
{
    [Route("api/[controller]")]
    [ApiController]
    public class MyApplicationController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public MyApplicationController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
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
                    UnderReview = applications.Count(a => a.Status == ApplicationStatus.Reviewing || 
                                                      a.Status == ApplicationStatus.Shortlisted || 
                                                      a.Status == ApplicationStatus.Interviewed),
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
                         filterExpression = filterExpression.And(a => a.Status == ApplicationStatus.Reviewing || 
                                                                      a.Status == ApplicationStatus.Shortlisted || 
                                                                      a.Status == ApplicationStatus.Interviewed);
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

                    // Define folder path: wwwroot/uploads/cvs
                    var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "cvs");
                    if (!Directory.Exists(folderPath))
                    {
                        Directory.CreateDirectory(folderPath);
                    }

                    // Generate unique filename
                    var fileName = $"{Guid.NewGuid()}{Path.GetExtension(applicationDTO.CvFile.FileName)}";
                    var filePath = Path.Combine(folderPath, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await applicationDTO.CvFile.CopyToAsync(stream);
                    }

                    // Set URL (assuming static file serving is enabled for wwwroot)
                    // URL format: /uploads/cvs/filename
                    // In a real app, use a proper storage service URL or relative path helper
                    var request = HttpContext.Request;
                    var baseUrl = $"{request.Scheme}://{request.Host}";
                    cvUrl = $"{baseUrl}/uploads/cvs/{fileName}";
                }

                // 3. Create Application Entity
                var application = new Application
                {
                    UserId = userId,
                    RecruitmentId = applicationDTO.RecruitmentId,
                    Status = ApplicationStatus.New, // "Pending" for applicant
                    CoverLetter = applicationDTO.CoverLetter,
                    CvUrl = cvUrl,
                    CreatedAt = DateTime.Now
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
