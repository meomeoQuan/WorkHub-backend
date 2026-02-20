using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WorkHub.DataAccess.Repository.IRepository;
using WorkHub.Models.DTOs;
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

        public ApplicationDetailController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
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
    }
}
