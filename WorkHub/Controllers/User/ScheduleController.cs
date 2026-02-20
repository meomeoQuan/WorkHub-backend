using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WorkHub.DataAccess.Repository.IRepository;
using WorkHub.Models.DTOs;
using WorkHub.Models.DTOs.ModelDTOs.ScheduleDTOs;
using WorkHub.Models.Models;

namespace WorkHub.Controllers.User
{
    [Route("api/[controller]")]
    [ApiController]
    public class ScheduleController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ScheduleController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Get()
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
                var schedules = await _unitOfWork.UserScheduleRepository.GetAllAsync(u => u.UserId == userId);
                var scheduleDTOs = _mapper.Map<IEnumerable<ScheduleViewDTO>>(schedules);
                return Ok(ApiResponse<IEnumerable<ScheduleViewDTO>>.Ok(scheduleDTOs, "Schedules retrieved successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.Error(500, $"Internal server error: {ex.Message}"));
            }
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] CreateScheduleDTO createDTO)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
                var schedule = _mapper.Map<UserSchedule>(createDTO);
                schedule.UserId = userId;
                schedule.CreatedAt = DateTime.UtcNow; // Or Now, depending on project convention

                _unitOfWork.UserScheduleRepository.Add(schedule);
                await _unitOfWork.SaveAsync();

                var viewDTO = _mapper.Map<ScheduleViewDTO>(schedule);
                return Ok(ApiResponse<ScheduleViewDTO>.Ok(viewDTO, "Schedule created successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.Error(500, $"Internal server error: {ex.Message}"));
            }
        }

        [HttpPut]
        [Authorize]
        public async Task<IActionResult> Update([FromBody] UpdateScheduleDTO updateDTO)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
                var schedule = await _unitOfWork.UserScheduleRepository.GetAsync(s => s.Id == updateDTO.Id);

                if (schedule == null)
                {
                    return NotFound(ApiResponse<object>.Error(404, "Schedule not found"));
                }

                if (schedule.UserId != userId)
                {
                    return Forbid();
                }

                // Manual mapping for partial update to avoid overwrite with nulls
                if (updateDTO.Title != null) schedule.Title = updateDTO.Title;
                if (updateDTO.StartTime.HasValue) schedule.StartTime = updateDTO.StartTime.Value;
                if (updateDTO.EndTime.HasValue) schedule.EndTime = updateDTO.EndTime.Value;
                
                // _mapper.Map(updateDTO, schedule); // Alternative if configured to ignore nulls

                // _unitOfWork.UserScheduleRepository.Update(schedule); // Not needed if tracking is on, but safe to include if repo has it. 
                                                                        // Checked earlier: Repository<T> doesn't have Update. 
                                                                        // Relying on tracking.

                await _unitOfWork.SaveAsync();

                return Ok(ApiResponse<object>.Ok(null, "Schedule updated successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.Error(500, $"Internal server error: {ex.Message}"));
            }
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
                var schedule = await _unitOfWork.UserScheduleRepository.GetAsync(s => s.Id == id);

                if (schedule == null)
                {
                    return NotFound(ApiResponse<object>.Error(404, "Schedule not found"));
                }

                if (schedule.UserId != userId)
                {
                    return Forbid();
                }

                _unitOfWork.UserScheduleRepository.Remove(schedule);
                await _unitOfWork.SaveAsync();

                return Ok(ApiResponse<object>.Ok(null, "Schedule deleted successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.Error(500, $"Internal server error: {ex.Message}"));
            }
        }
    }
}
