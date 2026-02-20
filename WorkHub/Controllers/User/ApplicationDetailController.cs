using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WorkHub.DataAccess.Repository.IRepository;
using WorkHub.Models.DTOs;
using WorkHub.Business.Service.IService;
using WorkHub.Models.DTOs.ModelDTOs;
using WorkHub.Models.DTOs.ModelDTOs.ApplicationDetailDTOs;
using WorkHub.Utility;

namespace WorkHub.Controllers.User
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApplicationDetailController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IEmailService _emailService;

        public ApplicationDetailController(IUnitOfWork unitOfWork, IMapper mapper, IEmailService emailService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _emailService = emailService;
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

                var application = await _unitOfWork.ApplicationRepository.GetAsync(
                    a => a.Id == id && a.UserId != userId,
                    includeProperties: "User,User.UserDetail,User.UserExperiences,User.UserEducations,Recruitment,Recruitment.User"
                );

                if (application == null)
                {
                    return NotFound(ApiResponse<object>.Error(404, "Application not found"));
                }

                if (application.Recruitment.UserId != userId && application.UserId != userId)
                {
                    return Forbid();
                }

                var applicationDetailDTO = _mapper.Map<ApplicationDetailDTO>(application);

                return Ok(ApiResponse<ApplicationDetailDTO>.Ok(applicationDetailDTO, "Application details retrieved successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.Error(500, $"Internal server error: {ex.Message}"));
            }
        }

        [HttpPost("update-status")]
        [Authorize]
        public async Task<IActionResult> UpdateStatus([FromBody] UpdateApplicationStatusDTO updateDTO)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

                var application = await _unitOfWork.ApplicationRepository.GetAsync(a => a.Id == updateDTO.ApplicationId);

                if (application == null)
                {
                    return NotFound(ApiResponse<object>.Error(404, "Application not found"));
                }

                // Security Check: Applicant cannot update their own status
                if (application.UserId == userId)
                {
                     return Forbid();
                }

                // Validate Status
                var validStatuses = new List<string> 
                { 
                    ApplicationStatus.New, 
                    ApplicationStatus.Reviewing, 
                    ApplicationStatus.Shortlisted, 
                    ApplicationStatus.Interviewed, 
                    ApplicationStatus.Accepted, 
                    ApplicationStatus.Rejected 
                };

                if (!validStatuses.Contains(updateDTO.Status))
                {
                    return BadRequest(ApiResponse<object>.BadRequest(null, "Invalid status value."));
                }

                application.Status = updateDTO.Status;
                //_unitOfWork.ApplicationRepository.Update(application);
                await _unitOfWork.SaveAsync();

                return Ok(ApiResponse<object>.Ok(null, "Application status updated successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.Error(500, $"Internal server error: {ex.Message}"));
            }
        }

        [HttpPost("send-email")]
        [Authorize]
        [Consumes("multipart/form-data")] // Required for IFormFile
        public async Task<IActionResult> SendEmail([FromForm] SendEmailRequestDTO requestDTO)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

                var application = await _unitOfWork.ApplicationRepository.GetAsync(
                    a => a.Id == requestDTO.ApplicationId,
                    includeProperties: "User,Recruitment"
                );

                if (application == null)
                {
                    return NotFound(ApiResponse<object>.Error(404, "Application not found"));
                }

                // Security Check: Only Recruiter (Employer) can send emails
                // Assuming Recruiter is the owner of the Recruitment
                if (application.Recruitment.UserId != userId)
                {
                     return Forbid();
                }

                // Handle Attachments
                List<string> attachmentPaths = new List<string>();
                if (requestDTO.Attachments != null && requestDTO.Attachments.Any())
                {
                    var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "temp_attachments");
                    if (!Directory.Exists(uploadPath))
                    {
                        Directory.CreateDirectory(uploadPath);
                    }

                    foreach (var file in requestDTO.Attachments)
                    {
                        if (file.Length > 0)
                        {
                            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                            var filePath = Path.Combine(uploadPath, fileName);
                            using (var stream = new FileStream(filePath, FileMode.Create))
                            {
                                await file.CopyToAsync(stream);
                            }
                            attachmentPaths.Add(filePath);
                        }
                    }
                }

                var emailRequest = new WorkHub.Models.DTOs.AuthDTOs.EmailRequestDTO
                {
                    To = application.User.Email, // Sending to Applicant
                    Subject = requestDTO.Subject,
                    Body = requestDTO.Body,
                    Attachments = attachmentPaths
                };

                await _emailService.SendEmailAsync(emailRequest);

                // Cleanup Temp Files
                foreach (var path in attachmentPaths)
                {
                    if (System.IO.File.Exists(path))
                    {
                        System.IO.File.Delete(path);
                    }
                }

                return Ok(ApiResponse<object>.Ok(null, "Email sent successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.Error(500, $"Internal server error: {ex.Message}"));
            }
        }
    }
}
