using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Linq.Expressions;
using System.Security.Claims;
using WorkHub.DataAccess.Repository.IRepository;
using WorkHub.Models.DTOs;
using WorkHub.Models.DTOs.ModelDTOs;
using WorkHub.Models.DTOs.ModelDTOs.HomeDTOs;
using WorkHub.Models.DTOs.ModelDTOs.JobPostDTOs;
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
                + SD.Collection_Join_PostRecruitments + ".Recruitment.JobType," 
                + SD.Collection_Join_PostRecruitments + ".Recruitment.Category",
                orderBy: p => p.CreatedAt,
                descending: true
            );

            var result = _mapper.Map<List<JobPostDTO>>(jobPosts);

            // Set IsLiked status if user is authenticated
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim != null)
            {
                var userId = int.Parse(userIdClaim.Value);
                foreach (var post in result)
                {
                    var originalPost = jobPosts.FirstOrDefault(p => p.Id == post.PostId);
                    if (originalPost != null)
                    {
                        post.IsLiked = originalPost.PostLikes.Any(l => l.UserId == userId);
                    }
                }
            }

            var response = ApiResponse<List<JobPostDTO>>.Ok(result, "All Post retrieved successfully");

            return Ok(response);
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchJobPosts(
            string? searchQuery,
            string? jobType,
            string? location,
            string? salaryRange,
            decimal? minSalary,
            decimal? maxSalary,
            string? salaryCurrency,
            string? salaryCycle,
            string? postedDate,
            string? category,
            int pageIndex = 1,
            int pageSize = 10)
        {
            // Refactoring to use combined expressions for EF Core compatibility
            var queryFilter = PredicateBuilder.True<Post>();

            if (!string.IsNullOrWhiteSpace(searchQuery))
            {
                var s = searchQuery.ToLower();
                queryFilter = queryFilter.And(p => p.Content.ToLower().Contains(s) ||
                                                   p.User.FullName.ToLower().Contains(s) ||
                                                   p.PostRecruitments.Any(pr => pr.Recruitment.JobName.ToLower().Contains(s) || pr.Recruitment.Location.ToLower().Contains(s)));
            }

            if (!string.IsNullOrWhiteSpace(jobType) && !jobType.Equals("all", StringComparison.OrdinalIgnoreCase))
            {
                 queryFilter = queryFilter.And(p =>
                 p.PostRecruitments.Any(pr => pr.Recruitment.JobType.Name == jobType)
 );

            }

            if (!string.IsNullOrWhiteSpace(location) && !location.Equals("all-cities", StringComparison.OrdinalIgnoreCase))
            {
                queryFilter = queryFilter.And(p => p.PostRecruitments.Any(pr => pr.Recruitment.Location.Contains(location)));
            }

            if (!string.IsNullOrWhiteSpace(category) && !category.Equals("all", StringComparison.OrdinalIgnoreCase))
            {
                queryFilter = queryFilter.And(p => p.PostRecruitments.Any(pr => pr.Recruitment.Category.Name == category));
            }

            // Structured Salary Filtering
            if (minSalary.HasValue)
            {
                queryFilter = queryFilter.And(p => p.PostRecruitments.Any(pr => pr.Recruitment.MinSalary >= minSalary.Value));
            }
            if (maxSalary.HasValue)
            {
                queryFilter = queryFilter.And(p => p.PostRecruitments.Any(pr => pr.Recruitment.MinSalary <= maxSalary.Value));
            }
            if (!string.IsNullOrWhiteSpace(salaryCurrency) && !salaryCurrency.Equals("all", StringComparison.OrdinalIgnoreCase))
            {
                queryFilter = queryFilter.And(p => p.PostRecruitments.Any(pr => pr.Recruitment.SalaryCurrency == salaryCurrency));
            }
            if (!string.IsNullOrWhiteSpace(salaryCycle) && !salaryCycle.Equals("all", StringComparison.OrdinalIgnoreCase))
            {
                queryFilter = queryFilter.And(p => p.PostRecruitments.Any(pr => pr.Recruitment.SalaryCycle == salaryCycle));
            }

            if (!string.IsNullOrWhiteSpace(postedDate) && !postedDate.Equals("anytime", StringComparison.OrdinalIgnoreCase))
            {
                DateTime cutoff = DateTime.UtcNow;
                if (postedDate == "1d") cutoff = cutoff.AddDays(-1);
                else if (postedDate == "1w") cutoff = cutoff.AddDays(-7);
                else if (postedDate == "1m") cutoff = cutoff.AddDays(-30);
                else if (postedDate == "1y") cutoff = cutoff.AddYears(-1);

                queryFilter = queryFilter.And(p => p.CreatedAt >= cutoff);
            }

            var jobPosts = await _unitOfWork.PostRepository.GetAllPagedAsync(
                pageIndex,
                pageSize,
                queryFilter,
                includeProperties: SD.Join_User + ","
                + SD.Collection_Join_Comments + ","
                + SD.Collection_Join_PostLikes + ","
                + SD.Collection_Join_PostRecruitments + ".Recruitment.JobType,"
                + SD.Collection_Join_PostRecruitments + ".Recruitment.Category",
                orderBy: p => p.CreatedAt,
                descending: true
            );

            var result = _mapper.Map<List<JobPostDTO>>(jobPosts);

            // Set IsLiked status if user is authenticated
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim != null)
            {
                var userId = int.Parse(userIdClaim.Value);
                foreach (var post in result)
                {
                    var originalPost = jobPosts.FirstOrDefault(p => p.Id == post.PostId);
                    if (originalPost != null)
                    {
                        post.IsLiked = originalPost.PostLikes.Any(l => l.UserId == userId);
                    }
                }
            }

            return Ok(ApiResponse<List<JobPostDTO>>.Ok(result, "Search results retrieved successfully"));
        }

        [HttpGet("cities-filter")]
        public async Task<IActionResult> GetCitiesFilter()
        {
            var recruitments = await _unitOfWork.RecruitmentInfoRepo.GetAllAsync();
            var cities = recruitments
                .Where(r => !string.IsNullOrEmpty(r.Location))
                .Select(r => r.Location)
                .Distinct()
                .ToList();

            return Ok(ApiResponse<List<string>>.Ok(cities, "Cities retrieved successfully"));
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
                    + SD.Collection_Join_PostRecruitments + ".Recruitment.JobType,"
                    + SD.Collection_Join_PostRecruitments + ".Recruitment.Category",
                orderBy: p => p.CreatedAt,
                descending: true
            );

            var result = _mapper.Map<List<JobPostDTO>>(jobPosts);

            return Ok(ApiResponse<List<JobPostDTO>>.Ok(result, "Following posts retrieved"));
        }




        [HttpGet("single-post")]
        public async Task<IActionResult> GetSinglePost([FromQuery] SinglePostRequestDTO singlePostRequest)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            int? userId = userIdClaim != null ? int.Parse(userIdClaim.Value) : null;

            if (userId != null)
            {
                var user = await _unitOfWork.UserRepository.GetAsync(u => u.Id == userId);
                if (user == null)
                {
                    return BadRequest(ApiResponse<object>.BadRequest("User unauthorized!"));
                }
            }
              

            var post = await _unitOfWork.PostRepository.GetAsync(
                c => c.Id == singlePostRequest.PostId,
                includeProperties: SD.Join_User + ","
                                + SD.Collection_Join_Comments + ","
                                + SD.Collection_Join_PostLikes + ","
                                + SD.Collection_Join_PostRecruitments + ".Recruitment.JobType,"
                                + SD.Collection_Join_PostRecruitments + ".Recruitment.Category"
            );

            if (post == null)
                return NotFound(ApiResponse<object>.NotFound("Post not found !"));

            var result = _mapper.Map<JobPostDTO>(post);

            if (userId != null)
            {
                result.IsLiked = post.PostLikes.Any(l => l.UserId == userId);
            }

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
        [HttpGet("all-comments-post")]
        public async Task<IActionResult> GetAllComments([FromQuery] AllCommentRequestDTO allCommentRequest)
        {
            var comments = await _unitOfWork.CommentRepository.GetAllAsync(
                c => c.PostId == allCommentRequest.PostId,
                includeProperties: SD.Join_User + "," + SD.Join_User + "." + SD.Join_UserDetail
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
                    UserId = x.UserId,
                    UserName = x.UserName,
                    Content = x.Content,
                    UserUrl = x.UserUrl,
                    CreatedAt = x.CreatedAt,
                    Replies = BuildTree(comments, x.Id)
                })
                .ToList();
        }

        [Authorize]
        [HttpGet("loading-create-post")]
        public async Task<IActionResult> LoadingCreatePost()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var jobs = await _unitOfWork.RecruitmentInfoRepo.GetAllAsync(r => r.UserId == userId);

            if(jobs !=null && jobs.Any())
            {
                var jobDTOs = _mapper.Map<List<RecruitmentSelectDTO>>(jobs);
                var responseData = new
                {
                    jobs = jobDTOs.ToList(),
                };
                return Ok(ApiResponse<object>.Ok(responseData, "Jobs retrieved successfully"));
            }

            return Ok(ApiResponse<object>.Ok(null, "The user has no jobs"));
        }


        [Authorize]
        [HttpPost("create-post")]
        public async Task<IActionResult> CreatePost(CreatePostDTO dto)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var user = await _unitOfWork.UserRepository.GetAsync(
                u => u.Id == userId,
                includeProperties: SD.Join_Subscription
            );
            
            if (user == null)
            {
                return BadRequest(ApiResponse<object>.BadRequest("User unauthorize !"));
            }

            // Enforce Subscription Limits
            var subscription = user.Subscription;
            var plan = (subscription != null && subscription.IsActive) ? subscription.Plan : SD.Plan_Free;
            if (plan != SD.Plan_Gold)
            {
                var cycleStart = SD.CalculateCycleStart(user.Subscription?.StartAt ?? user.CreatedAt);

                var postCount = await _unitOfWork.PostRepository.CountAsync(r =>
                    r.UserId == userId &&
                    r.CreatedAt >= cycleStart
                );

                if (plan == SD.Plan_Free && postCount >= SD.Free_Post_Limit)
                {
                    return BadRequest(ApiResponse<object>.BadRequest("Bạn đã đạt giới hạn đăng bài của gói Miễn Phí. Làm ơn hãy nâng cấp gói của bạn !"));
                }

                if (plan == SD.Plan_Silver && postCount >= SD.Silver_Post_Limit)
                {
                    return BadRequest(ApiResponse<object>.BadRequest("Bạn đã đạt giới hạn đăng bài của gói Bạc. Làm ơn hãy nâng cấp gói của bạn !"));
                }
            }

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

                foreach (var rId in dto.RecruitmentIds)
                {
                    post.PostRecruitments.Add(new PostRecruitment { PostId = post.Id, RecruitmentId = rId });
                }

                await _unitOfWork.SaveAsync();
            }
            return Ok(ApiResponse<object>.Ok(null, "Post created successfully"));
        }

        [Authorize]
        [HttpDelete("delete-post/{id}")]
        public async Task<IActionResult> DeletePost(int id)
        {
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdStr)) return Unauthorized();
            var userId = int.Parse(userIdStr);

            var post = await _unitOfWork.PostRepository.GetAsync(p => p.Id == id);

            if (post == null)
            {
                return NotFound(ApiResponse<object>.BadRequest("Post not found"));
            }

            if (post.UserId != userId)
            {
                return Forbid();
            }

            // Unlink recruitments (cascade delete will handle this, but manual clearing is fine)
            post.PostRecruitments.Clear();

            _unitOfWork.PostRepository.Remove(post);
            await _unitOfWork.SaveAsync();

            return Ok(ApiResponse<object>.Ok(null, "Post deleted successfully"));
        }

        [Authorize]
        [HttpPut("update-post/{id}")]
        public async Task<IActionResult> UpdatePost(int id, [FromForm] UpdatePostDTO dto)
        {
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdStr)) return Unauthorized();
            var userId = int.Parse(userIdStr);

            var post = await _unitOfWork.PostRepository.GetAsync(p => p.Id == id);

            if (post == null)
            {
                return NotFound(ApiResponse<object>.BadRequest("Post not found"));
            }

            if (post.UserId != userId)
            {
                return Forbid();
            }

            post.Content = dto.Content;
            
            // Handle image upload
            if (dto.PostImage != null && dto.PostImage.Length > 0)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "posts");
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(dto.PostImage.FileName)}";
                var filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await dto.PostImage.CopyToAsync(stream);
                }

                post.PostImageUrl = $"/uploads/posts/{fileName}";
            }
            else if (!string.IsNullOrEmpty(dto.PostImageUrl))
            {
                post.PostImageUrl = dto.PostImageUrl;
            }
            else
            {
                post.PostImageUrl = null;
            }
            // Update recruitment links
            // 1. Unlink current
            // 1. Unlink current
            var currentLinks = await _unitOfWork.PostRecruitmentRepository.GetAllAsync(r => r.PostId == id);
            foreach (var link in currentLinks)
            {
                _unitOfWork.PostRecruitmentRepository.Remove(link);
            }

            // 2. Link new ones
            if (dto.RecruitmentIds?.Any() == true)
            {
                foreach (var rId in dto.RecruitmentIds)
                {
                    post.PostRecruitments.Add(new PostRecruitment { PostId = id, RecruitmentId = rId });
                }
            }

            _unitOfWork.PostRepository.Update(post);
            await _unitOfWork.SaveAsync();

            return Ok(ApiResponse<object>.Ok(null, "Post updated successfully"));
        }

        [HttpPost("toggle-like")]
        public async Task<IActionResult> ToggleLike(ToggleLikeDTO dto)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var user = await _unitOfWork.UserRepository.GetAsync(u => u.Id == userId);

            if (user == null)
            {
                return BadRequest(ApiResponse<object>.BadRequest("User unauthorize !"));
            }

            var existingLike = await _unitOfWork.PostLikeRepository.GetAsync(
                x => x.UserId == userId && x.PostId == dto.PostId
            );

            if (existingLike != null)
            {
                // UNLIKE
                _unitOfWork.PostLikeRepository.Remove(existingLike);
                await _unitOfWork.SaveAsync();

                return Ok(ApiResponse<int>.Ok(await _unitOfWork.PostLikeRepository.CountAsync(x => x.PostId == dto.PostId), "Unliked"));
            }

            // LIKE
            var newLike = new PostLike
            {
                UserId = userId,
                PostId = dto.PostId
            };

             _unitOfWork.PostLikeRepository.Add(newLike);
            await _unitOfWork.SaveAsync();

            return Ok(ApiResponse<int>.Ok(await _unitOfWork.PostLikeRepository.CountAsync(x => x.PostId == dto.PostId), "Liked"));
        }

        [Authorize]
    [HttpPost("add-comment")]
    public async Task<IActionResult> AddComment(AddCommentDTO dto)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        var user = await _unitOfWork.UserRepository.GetAsync(u => u.Id == userId);

        if (user == null)
        {
            return BadRequest(ApiResponse<object>.BadRequest("User unauthorize !"));
        }

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
    [HttpPut("update-comment/{id}")]
    public async Task<IActionResult> UpdateComment(int id, [FromBody] UpdateCommentDTO dto)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var comment = await _unitOfWork.CommentRepository.GetAsync(c => c.Id == id);

        if (comment == null)
            return NotFound(ApiResponse<object>.NotFound("Comment not found"));

        if (comment.UserId != userId)
            return BadRequest(ApiResponse<object>.BadRequest("You can only edit your own comments"));

        comment.Content = dto.Content;
        await _unitOfWork.SaveAsync();

        return Ok(ApiResponse<object>.Ok(null, "Comment updated"));
    }

    [Authorize]
    [HttpDelete("delete-comment/{id}")]
    public async Task<IActionResult> DeleteComment(int id)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var comment = await _unitOfWork.CommentRepository.GetAsync(c => c.Id == id);

        if (comment == null)
            return NotFound(ApiResponse<object>.NotFound("Comment not found"));

        // Allow owner or admin to delete
        var user = await _unitOfWork.UserRepository.GetAsync(u => u.Id == userId);
        var adminRole = RoleMapper.MapRoleToRoleNumber(SD.Role_Admin);
        
        if (comment.UserId != userId && (user == null || user.Role != adminRole))
            return BadRequest(ApiResponse<object>.BadRequest("You can only delete your own comments"));

        _unitOfWork.CommentRepository.Remove(comment);
        await _unitOfWork.SaveAsync();

        return Ok(ApiResponse<object>.Ok(null, "Comment deleted"));
    }


        [Authorize]
        [HttpPost("toggle-follow")]
        public async Task<IActionResult> ToggleFollow(ToggleFollowDTO dto)
        {
            var followerId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
        

            var user = await _unitOfWork.UserRepository.GetAsync(u => u.Id == followerId);

            if (user == null)
            {
                return BadRequest(ApiResponse<object>.BadRequest("User unauthorize !"));
            }

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

        [Authorize]
        [HttpGet("following-count")]
        public async Task<IActionResult> GetFollowingCount()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var count = await _unitOfWork.userFollowRepository.CountAsync(x => x.FollowerId == userId);
            return Ok(ApiResponse<int>.Ok(count, "Following count retrieved successfully"));
        }

        [HttpGet("like-count/{postId}")]
        public async Task<IActionResult> GetLikeCount(int postId)
        {
            var count = await _unitOfWork.PostLikeRepository.CountAsync(x => x.PostId == postId);
            return Ok(ApiResponse<int>.Ok(count, "Like count retrieved successfully"));
        }

     


    }
}
