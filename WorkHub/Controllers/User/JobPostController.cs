using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WorkHub.DataAccess.Repository.IRepository;
using WorkHub.Models.DTOs;
using WorkHub.Models.DTOs.ModelDTOs.HomeDTOs;
using WorkHub.Models.DTOs.ModelDTOs.JobsDTOs;
using WorkHub.Models.Models;
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
        public async Task<IActionResult> GetAllJobPosts(int pageIndex = 1, int pageSize = 10)
        {
            var jobPosts = await _unitOfWork.PostRepository.GetAllPagedAsync(
                pageIndex,
                pageSize,
                includeProperties: SD.Join_User + ","
                + SD.Collection_Join_Comments + ","
                + SD.Collection_Join_PostLikes + ","
                + SD.Collection_Join_Recruitments,
                orderBy: p => p.CreatedAt,
                descending: true
            );

            var result = _mapper.Map<List<JobPostDTO>>(jobPosts);

            var response = ApiResponse<List<JobPostDTO>>.Ok(result, "All Post retrieved successfully");

            return Ok(response);
        }

        [Authorize]
        [HttpGet("all-post-following")]
        public async Task<IActionResult> GetAllFollowingPosts(int pageIndex = 1, int pageSize = 10)
        {
            var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);

            // 1. Get IDs of users I follow
            var followingIds = (await _unitOfWork.userFollowRepository.GetAllAsync(
                x => x.FollowerId == userId
            )).Select(x => x.FollowingId).ToList();

            // If following nobody → empty feed
            if (!followingIds.Any())
                return Ok(ApiResponse<List<JobPostDTO>>.Ok(new List<JobPostDTO>(), "No following posts"));

            // 2. Get posts from followed users
            var jobPosts = await _unitOfWork.PostRepository.GetAllPagedAsync(
                pageIndex,
                pageSize,
                p => followingIds.Contains(p.UserId),
                includeProperties: SD.Join_User + ","
                    + SD.Collection_Join_Comments + ","
                    + SD.Collection_Join_PostLikes + ","
                    + SD.Collection_Join_Recruitments,
                orderBy: p => p.CreatedAt,
                descending: true
            );

            var result = _mapper.Map<List<JobPostDTO>>(jobPosts);

            return Ok(ApiResponse<List<JobPostDTO>>.Ok(result, "Following posts retrieved"));
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


        //     [HttpPost("all-comments-post")]
        //     public async Task<IActionResult> GetCommentByPost(AllCommentRequestDTO allCommentRequest)
        //     {
        //         var comments = await _unitOfWork.CommentRepository.GetAllAsync(
        //    c => c.PostId == allCommentRequest.PostId,
        //    includeProperties: SD.Join_User
        //);

        //         var flat = _mapper.Map<List<CommentDTO>>(comments);

        //         // Root comments
        //         var roots = flat
        //             .Where(x => x.ParentCommentId == null)
        //             .Select(c => new CommentTreeDTO
        //             {
        //                 Id = c.Id,
        //                 UserName = c.UserName,
        //                 Content = c.Content,
        //                 CreatedAt = c.CreatedAt
        //             })
        //             .ToList();

        //         // Replies
        //         foreach (var root in roots)
        //         {
        //             root.Replies = flat
        //                 .Where(r => r.ParentCommentId == root.Id)
        //                 .Select(r => new CommentTreeDTO
        //                 {
        //                     Id = r.Id,
        //                     UserName = r.UserName,
        //                     Content = r.Content,
        //                     CreatedAt = r.CreatedAt
        //                 })
        //                 .ToList();
        //         }

        //         var responseData = new
        //         {
        //             postId = allCommentRequest.PostId,
        //             comments = roots
        //         };

        //         return Ok(ApiResponse<object>.Ok(responseData, "All Comments retrieved successfully"));

        //     }
        [Authorize]
        [HttpPost("all-comments-post")]
        public async Task<IActionResult> GetCommentByPost(AllCommentRequestDTO allCommentRequest)
        {
            var comments = await _unitOfWork.CommentRepository.GetAllAsync(
                c => c.PostId == allCommentRequest.PostId,
                includeProperties: SD.Join_User
            );

            var flat = _mapper.Map<List<CommentDTO>>(comments);

            // Recursive tree
            var tree = BuildTree(flat, null);

            var responseData = new
            {
                postId = allCommentRequest.PostId,
                comments = tree
            };

            return Ok(ApiResponse<object>.Ok(responseData, "All Comments retrieved successfully"));
        }

        private List<CommentTreeDTO> BuildTree(List<CommentDTO> comments, int? parentId)
        {
            return comments
                .Where(x => x.ParentCommentId == parentId)
                .Select(x => new CommentTreeDTO
                {
                    Id = x.Id,
                    UserName = x.UserName,
                    Content = x.Content,
                    CreatedAt = x.CreatedAt,
                    Replies = BuildTree(comments, x.Id)
                })
                .ToList();
        }


        [Authorize]
        [HttpPost("create-post")]
        [Authorize]
        public async Task<IActionResult> CreatePost(CreatePostDTO dto)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var post = new Post
            {
                UserId = userId,
                Content = dto.Content,
                PostImageUrl = dto.PostImageUrl,
                CreatedAt = DateTime.UtcNow
            };

            _unitOfWork.PostRepository.Add(post);
            await _unitOfWork.SaveAsync(); // generate Post.Id

            // Attach recruitments (optional)
            if (dto.RecruitmentIds?.Any() == true)
            {
                var recruitments = await _unitOfWork.RecruitmentInfoRepo
                    .GetAllAsync(r => dto.RecruitmentIds.Contains(r.Id));

                foreach (var r in recruitments)
                {
                    r.PostId = post.Id;   // ✅ LINK TO POST
                }

                await _unitOfWork.SaveAsync();
            }

            return Ok(ApiResponse<object>.Ok(null, "Post created successfully"));
        }




        [Authorize]
        [HttpPost("toggle-like")]
        public async Task<IActionResult> ToggleLike(ToggleLikeDTO dto)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var existingLike = await _unitOfWork.PostLikeRepository.GetAsync(
                x => x.UserId == userId && x.PostId == dto.PostId
            );

            if (existingLike != null)
            {
                // UNLIKE
                _unitOfWork.PostLikeRepository.Remove(existingLike);
                await _unitOfWork.SaveAsync();

                return Ok(ApiResponse<object>.Ok(null, "Unliked"));
            }

            // LIKE
            var newLike = new PostLike
            {
                UserId = userId,
                PostId = dto.PostId
            };

             _unitOfWork.PostLikeRepository.Add(newLike);
            await _unitOfWork.SaveAsync();

            return Ok(ApiResponse<object>.Ok(null, "Liked"));
        }

        [Authorize]
        [HttpPost("add-comment")]
        public async Task<IActionResult> AddComment(AddCommentDTO dto)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var comment = new Comment
            {
                PostId = dto.PostId,
                Content = dto.Content,
                ParentCommentId = dto.ParentCommentId, // NULL = root, ID = reply
                UserId = userId
            };

             _unitOfWork.CommentRepository.Add(comment);
            await _unitOfWork.SaveAsync();

            return Ok(ApiResponse<object>.Ok(null, "Comment added"));
        }


        [Authorize]
        [HttpPost("toggle-follow")]
        public async Task<IActionResult> ToggleFollow(ToggleFollowDTO dto)
        {
            var followerId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            if (followerId == dto.FollowingId)
                return BadRequest(ApiResponse<object>.BadRequest("You cannot follow yourself"));

            var existing = await _unitOfWork.userFollowRepository.GetAsync(
                x => x.FollowerId == followerId && x.FollowingId == dto.FollowingId
            );

            if (existing != null)
            {
                // UNFOLLOW
                _unitOfWork.userFollowRepository.Remove(existing);
                await _unitOfWork.SaveAsync();

                return Ok(ApiResponse<object>.Ok(null, "Unfollowed"));
            }

            // FOLLOW
            var follow = new UserFollow
            {
                FollowerId = followerId,
                FollowingId = dto.FollowingId
            };

           _unitOfWork.userFollowRepository.Add(follow);
            await _unitOfWork.SaveAsync();

            return Ok(ApiResponse<object>.Ok(null, "Followed"));
        }

     


    }
}
