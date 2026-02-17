using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WorkHub.DataAccess.Repository.IRepository;
using WorkHub.Models.DTOs;
using WorkHub.Models.DTOs.ModelDTOs;
using WorkHub.Models.Models;
using WorkHub.Utility;

namespace WorkHub.Controllers.User
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserProfileController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UserProfileController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        [Authorize]
        [HttpGet("show-profile")]
        public async Task<IActionResult> ShowProfile()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var user = await _unitOfWork.UserRepository.GetAsync(
                u => u.Id == userId,
                includeProperties: "UserDetail,UserExperiences,UserEducations,UserSchedules"
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
                includeProperties: "UserDetail,UserExperiences,UserEducations,UserSchedules"
            );

            if (user == null)
            {
                return NotFound(ApiResponse<object>.NotFound("User not found"));
            }

            // Update Basic Info
            user.FullName = userProfileDTO.FullName;
            user.Phone = userProfileDTO.Phone;
            // Email is usually not editable freely due to verification, but allowing for now if needed or ignored
            // user.Email = userProfileDTO.Email; 

            // Update UserDetail
            if (user.UserDetail == null)
            {
                user.UserDetail = new UserDetail { UserId = userId };
                // Add to context if it was null? Accessing via navigation should be tracked if we attach it.
                // But safer to let EF Core handle it via navigation.
            }

            user.UserDetail.AvatarUrl = userProfileDTO.AvatarUrl;
            user.UserDetail.Location = userProfileDTO.Location;
            user.UserDetail.Bio = userProfileDTO.Bio;
            user.UserDetail.JobPreference = userProfileDTO.Title; // Map Title -> JobPreference
            user.UserDetail.CvUrl = userProfileDTO.CvUrl;
            user.UserDetail.Website = userProfileDTO.Website;
            user.UserDetail.CompanySize = userProfileDTO.CompanySize;
            user.UserDetail.FoundedYear = userProfileDTO.FoundedYear;
            user.UserDetail.IndustryFocus = userProfileDTO.Industry;
            
            // Join Skills list to string
            user.UserDetail.Skills = string.Join(",", userProfileDTO.Skills);


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
    }
}
