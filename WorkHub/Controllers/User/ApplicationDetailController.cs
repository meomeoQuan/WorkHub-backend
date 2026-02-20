using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WorkHub.DataAccess.Repository.IRepository;
using WorkHub.Models.DTOs;
using WorkHub.Models.DTOs.ModelDTOs.ApplicationDetailDTOs;

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

                // Security Check: Only the Recruiter (Employer) OR the Applicant can view this
                // User requested "Employer Application Detail View", so Recruiter check is primary.
                // Assuming "MyApplicationDetail" implies "Details of an application I received" (Employer) 
                // OR "Details of my application" (Applicant). 
                // Given the fields requested (candidate info), it's strongly implied for Employer.
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
    }
}
