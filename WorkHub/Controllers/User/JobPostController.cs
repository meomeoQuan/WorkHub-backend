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


        [HttpPost("single-post")]
        public async Task<IActionResult> GetCommentByPost(SinglePostRequestDTO singlePostRequest)
        {
            var post = await _unitOfWork.PostRepository.GetAsync(
                c => c.Id == singlePostRequest.PostId,
                includeProperties: SD.Join_User + ","
                                + SD.Collection_Join_Comments + ","
                                + SD.Collection_Join_PostLikes + ","
                                + SD.Collection_Join_Recruitments
            );

            if (post == null)
                return NotFound(ApiResponse<object>.NotFound("Post not found !"));

            var result = _mapper.Map<JobPostDTO>(post);

            var responseData = new
            {
                postId = singlePostRequest.PostId,
                post = result
            };

            return Ok(ApiResponse<object>.Ok(responseData, "Post retrieved successfully"));
        }


        [HttpPost("all-comments-post")]
        public async Task<IActionResult> GetCommentByPost(AllCommentRequestDTO allCommentRequest)
        {
            var comments = await _unitOfWork.CommentRepository.GetAllAsync(
       c => c.PostId == allCommentRequest.PostId,
       includeProperties: SD.Join_User
   );

            var flat = _mapper.Map<List<CommentDTO>>(comments);

            // Root comments
            var roots = flat
                .Where(x => x.ParentCommentId == null)
                .Select(c => new CommentTreeDTO
                {
                    Id = c.Id,
                    UserName = c.UserName,
                    Content = c.Content,
                    CreatedAt = c.CreatedAt
                })
                .ToList();

            // Replies
            foreach (var root in roots)
            {
                root.Replies = flat
                    .Where(r => r.ParentCommentId == root.Id)
                    .Select(r => new CommentTreeDTO
                    {
                        Id = r.Id,
                        UserName = r.UserName,
                        Content = r.Content,
                        CreatedAt = r.CreatedAt
                    })
                    .ToList();
            }

            var responseData = new
            {
                postId = allCommentRequest.PostId,
                comments = roots
            };

            return Ok(ApiResponse<object>.Ok(responseData, "All Comments retrieved successfully"));

        }
    }
}
