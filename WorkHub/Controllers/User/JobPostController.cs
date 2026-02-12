using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WorkHub.DataAccess.Repository.IRepository;
using WorkHub.Models.DTOs;
using WorkHub.Models.DTOs.ModelDTOs.HomeDTOs;
using WorkHub.Models.DTOs.ModelDTOs.JobsDTOs;
using WorkHub.Utility;

namespace WorkHub.Controllers.User
{
    [Route("api/[controller]")]
    [ApiController]
    public class JobPostController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public JobPostController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        [HttpGet("all-post")]
        public async Task<IActionResult> GetAllJobPosts()
        {
            var jobPosts = await _unitOfWork.PostRepository.GetAllAsync(includeProperties: SD.Join_User + "," 
                                                                                        + SD.Collection_Join_Comments + "," 
                                                                                        + SD.Collection_Join_PostLikes + "," 
                                                                                        + SD.Collection_Join_Recruitments);

            var result = _mapper.Map<List<JobPostDTO>>(jobPosts);

            var response = ApiResponse<List<JobPostDTO>>.Ok(result, "All Post retrieved successfully");

            return Ok(response);
        }

    }
}
