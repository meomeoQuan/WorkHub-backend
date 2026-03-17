using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WorkHub.DataAccess.Repository.IRepository;
using WorkHub.Models.DTOs;
using WorkHub.Models.DTOs.ModelDTOs;
using WorkHub.Models.DTOs.ModelDTOs.JobPostDTOs;
using WorkHub.Models.Models;
using WorkHub.Utility;
using WorkHub.Business.Service.IService;

namespace WorkHub.Controllers.Users
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserProfileController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IMediaService _mediaService;

        public UserProfileController(IUnitOfWork unitOfWork, IMapper mapper, IMediaService mediaService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _mediaService = mediaService;
        }

        [HttpGet("show-profile/{userId}")]
        public async Task<IActionResult> ShowProfile(int userId)
        {
            var user = await _unitOfWork.UserRepository.GetAsync(
                u => u.Id == userId,
                includeProperties: "UserDetail,UserExperiences,UserEducations,UserSchedules,Subscription"
            );

            if (user == null)
            {
                return NotFound(ApiResponse<object>.NotFound("User not found"));
            }

            var userProfileDTO = _mapper.Map<UserProfileDTO>(user);

            return Ok(ApiResponse<UserProfileDTO>.Ok(userProfileDTO, "User profile retrieved successfully"));
        }

        [Authorize]
        [HttpPost("edit-profile")]
        public async Task<IActionResult> EditProfile([FromBody] UserProfileDTO userProfileDTO)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var user = await _unitOfWork.UserRepository.GetAsync(
                u => u.Id == userId,
                includeProperties: "UserDetail,UserExperiences,UserEducations,UserSchedules,Subscription"
            );

            if (user == null)
            {
                return NotFound(ApiResponse<object>.NotFound("User not found"));
            }

            // Update Basic Info & UserDetail via AutoMapper
            _mapper.Map(userProfileDTO, user);
            
            // Handle UserDetail creation if it was null but now mapped? 
            // AutoMapper might instantiate it if configured, but here we are mapping to existing `user` entity.
            // If `user.UserDetail` is null, AutoMapper will try to map to it. 
            // Let's ensure UserDetail exists if it's null, or let AutoMapper handle it if it can (for nested objects, usually needs existing instance or creates new if property is null).
            // Better approach: explicit check as before for safety or let EF Core fixup.
            if (user.UserDetail == null)
            {
                 user.UserDetail = new UserDetail { UserId = userId };
                 // Re-map to ensure the new instance gets populated
                 _mapper.Map(userProfileDTO, user.UserDetail);
            }


            // Update Experiences (Full Sync Strategy)
            // 1. Remove existing not in new list
            var existingExperienceIds = user.UserExperiences.Select(e => e.Id).ToList();
            var newExperienceIds = userProfileDTO.Experiences.Where(e => e.Id != 0).Select(e => e.Id).ToList();
            var experiencesToDelete = user.UserExperiences.Where(e => !newExperienceIds.Contains(e.Id)).ToList();
            
            foreach (var exp in experiencesToDelete)
            {
                _unitOfWork.UserExperienceRepository.Remove(exp); 
            }

            // 2. Add or Update
            foreach (var expDto in userProfileDTO.Experiences)
            {
                if (expDto.Id == 0)
                {
                    // Add
                    var newExp = _mapper.Map<UserExperience>(expDto);
                    newExp.UserId = userId;
                    _unitOfWork.UserExperienceRepository.Add(newExp); // Use Repo
                }
                else
                {
                    // Update
                    var existingExp = user.UserExperiences.FirstOrDefault(e => e.Id == expDto.Id);
                    if (existingExp != null)
                    {
                        _mapper.Map(expDto, existingExp);
                    }
                }
            }

            // Update Educations (Full Sync Strategy)
            var newEducationIds = userProfileDTO.Educations.Where(e => e.Id != 0).Select(e => e.Id).ToList();
            var educationsToDelete = user.UserEducations.Where(e => !newEducationIds.Contains(e.Id)).ToList();

            foreach (var edu in educationsToDelete)
            {
                _unitOfWork.UserEducationRepository.Remove(edu);
            }

            foreach (var eduDto in userProfileDTO.Educations)
            {
                if (eduDto.Id == 0)
                {
                    var newEdu = _mapper.Map<UserEducation>(eduDto);
                    newEdu.UserId = userId;
                    _unitOfWork.UserEducationRepository.Add(newEdu);
                }
                else
                {
                    var existingEdu = user.UserEducations.FirstOrDefault(e => e.Id == eduDto.Id);
                    if (existingEdu != null)
                    {
                        _mapper.Map(eduDto, existingEdu);
                    }
                }
            }
            
            // Update Schedules (Full Sync Strategy)
             var newScheduleIds = userProfileDTO.Schedules.Where(s => s.Id != 0).Select(s => s.Id).ToList();
            var schedulesToDelete = user.UserSchedules.Where(s => !newScheduleIds.Contains(s.Id)).ToList();

             foreach (var sch in schedulesToDelete)
            {
                _unitOfWork.UserScheduleRepository.Remove(sch);
            }

            foreach (var schDto in userProfileDTO.Schedules)
            {
                if (schDto.Id == 0)
                {
                    var newSch = _mapper.Map<UserSchedule>(schDto);
                    newSch.UserId = userId;
                     _unitOfWork.UserScheduleRepository.Add(newSch);
                }
                else
                {
                    var existingSch = user.UserSchedules.FirstOrDefault(s => s.Id == schDto.Id);
                    if (existingSch != null)
                    {
                        _mapper.Map(schDto, existingSch);
                    }
                }
            }


            _unitOfWork.UserRepository.Update(user); // or just SaveAsync since entities are tracked
            await _unitOfWork.SaveAsync();

            return Ok(ApiResponse<object>.Ok(null, "Profile updated successfully"));
        }

        [Authorize]
        [HttpPost("upload-avatar")]
        public async Task<IActionResult> UploadAvatar(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest(ApiResponse<object>.BadRequest("No file uploaded"));

            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var user = await _unitOfWork.UserRepository.GetAsync(
                u => u.Id == userId,
                includeProperties: "UserDetail"
            );

            if (user == null)
                return NotFound(ApiResponse<object>.NotFound("User not found"));

            // Ensure UserDetail exists
            if (user.UserDetail == null)
            {
                user.UserDetail = new UserDetail { UserId = userId };
                _unitOfWork.UserDetailRepository.Add(user.UserDetail);
            }

            // Upload to Cloudinary
            var avatarUrl = await _mediaService.UploadAsync(file, "avatars");
            if (string.IsNullOrEmpty(avatarUrl))
                return StatusCode(500, ApiResponse<object>.Error(500, "Failed to upload avatar to third-party storage"));

            user.UserDetail.AvatarUrl = avatarUrl;
            
            await _unitOfWork.SaveAsync();

            return Ok(ApiResponse<object>.Ok(new { avatarUrl }, "Avatar uploaded successfully"));
        }

        [Authorize]
        [HttpPost("upload-resume")]
        public async Task<IActionResult> UploadResume(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest(ApiResponse<object>.BadRequest("No file uploaded"));

            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var user = await _unitOfWork.UserRepository.GetAsync(
                u => u.Id == userId,
                includeProperties: "UserDetail"
            );

            if (user == null)
                return NotFound(ApiResponse<object>.NotFound("User not found"));

            // Ensure UserDetail exists
            if (user.UserDetail == null)
            {
                user.UserDetail = new UserDetail { UserId = userId };
                _unitOfWork.UserDetailRepository.Add(user.UserDetail);
            }

            // Upload to Cloudinary (raw/auto for PDFs)
            var cvUrl = await _mediaService.UploadFileAsync(file, "cvs");
            if (string.IsNullOrEmpty(cvUrl))
                return StatusCode(500, ApiResponse<object>.Error(500, "Failed to upload resume to storage"));

            user.UserDetail.CvUrl = cvUrl;
            await _unitOfWork.SaveAsync();

            return Ok(ApiResponse<object>.Ok(new { cvUrl, fileName = file.FileName }, "Resume uploaded successfully"));
        }

        [Authorize]
        [HttpDelete("delete-resume")]
        public async Task<IActionResult> DeleteResume()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var user = await _unitOfWork.UserRepository.GetAsync(
                u => u.Id == userId,
                includeProperties: "UserDetail"
            );

            if (user == null)
                return NotFound(ApiResponse<object>.NotFound("User not found"));

            if (user.UserDetail != null)
            {
                user.UserDetail.CvUrl = null;
                await _unitOfWork.SaveAsync();
            }

            return Ok(ApiResponse<object>.Ok(null, "Resume removed successfully"));
        }


        [Authorize]
        [HttpGet("all-user-jobs")]
        public async Task<IActionResult> GetAllUserJobs()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var jobs = await _unitOfWork.RecruitmentInfoRepo.GetAllAsync(
                r => r.UserId == userId,
                includeProperties: "JobType,Category"
            );

            var result = _mapper.Map<List<JobDTO>>(jobs);

            return Ok(ApiResponse<List<JobDTO>>.Ok(result, "User jobs retrieved successfully"));
        }


        [Authorize]
        [HttpGet("all-user-posts")]
        public async Task<IActionResult> GetAllUserPosts()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var posts = await _unitOfWork.PostRepository.GetAllAsync(
                p => p.UserId == userId,
                includeProperties: SD.Join_User + ","
                    + SD.Collection_Join_Comments + ","
                    + SD.Collection_Join_PostLikes + ","
                    + SD.Collection_Join_PostRecruitments + ".Recruitment.JobType," 
                    + SD.Collection_Join_PostRecruitments + ".Recruitment.Category"
            );

            var result = _mapper.Map<List<JobPostDTO>>(posts);

            return Ok(ApiResponse<List<JobPostDTO>>.Ok(result, "User posts retrieved successfully"));
        }

        [HttpGet("public-profile/{userId}")]
        public async Task<IActionResult> GetPublicProfile(int userId)
        {
            var user = await _unitOfWork.UserRepository.GetAsync(
                u => u.Id == userId,
                includeProperties: "UserDetail,UserExperiences,UserEducations,UserSchedules,Subscription"
            );

            if (user == null)
            {
                return NotFound(ApiResponse<object>.NotFound("User not found"));
            }

            var userProfileDTO = _mapper.Map<UserProfileDTO>(user);

            return Ok(ApiResponse<UserProfileDTO>.Ok(userProfileDTO, "Public profile retrieved successfully"));
        }

        [HttpGet("public-user-jobs/{userId}")]
        public async Task<IActionResult> GetPublicUserJobs(int userId)
        {
            var jobs = await _unitOfWork.RecruitmentInfoRepo.GetAllAsync(
                r => r.UserId == userId,
                includeProperties: "JobType,Category"
            );

            var result = _mapper.Map<List<JobDTO>>(jobs);

            return Ok(ApiResponse<List<JobDTO>>.Ok(result, "Public user jobs retrieved successfully"));
        }

        [HttpGet("public-user-posts/{userId}")]
        public async Task<IActionResult> GetPublicUserPosts(int userId)
        {
            var posts = await _unitOfWork.PostRepository.GetAllAsync(
                p => p.UserId == userId,
                includeProperties: SD.Join_User + ","
                    + SD.Collection_Join_Comments + ","
                    + SD.Collection_Join_PostLikes + ","
                    + SD.Collection_Join_PostRecruitments + ".Recruitment.JobType," 
                    + SD.Collection_Join_PostRecruitments + ".Recruitment.Category"
            );

            var result = _mapper.Map<List<JobPostDTO>>(posts);

            return Ok(ApiResponse<List<JobPostDTO>>.Ok(result, "Public user posts retrieved successfully"));
        }
        [Authorize]
        [HttpPost("report-user/{userId}")]
        public async Task<IActionResult> ReportUser(int userId)
        {
            var targetUser = await _unitOfWork.UserRepository.GetAsync(
                u => u.Id == userId,
                includeProperties: "UserDetail"
            );

            if (targetUser == null)
            {
                return NotFound(ApiResponse<object>.NotFound("User not found"));
            }

            if (targetUser.UserDetail == null)
            {
                targetUser.UserDetail = new UserDetail { UserId = userId, Rating = 4.0 }; // Default rating if null
                _unitOfWork.UserDetailRepository.Add(targetUser.UserDetail);
            }

            // Penalize rating by 1 star
            targetUser.UserDetail.Rating = Math.Max(0.0, (targetUser.UserDetail.Rating ?? 4.0) - 0.25);

            await _unitOfWork.SaveAsync();

            return Ok(ApiResponse<object>.Ok(null, "User reported. Rating penalized."));
        }
    }
}
