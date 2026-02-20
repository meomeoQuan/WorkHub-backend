using System;
using System.ComponentModel.DataAnnotations;

namespace WorkHub.Models.DTOs.ModelDTOs.ScheduleDTOs
{
    public class UpdateScheduleDTO
    {
        [Required]
        public int Id { get; set; }

        public string? Title { get; set; }

        public DateTime? StartTime { get; set; }

        public DateTime? EndTime { get; set; }
    }
}
