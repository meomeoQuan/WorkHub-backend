using System;
using System.ComponentModel.DataAnnotations;

namespace WorkHub.Models.DTOs.ModelDTOs.ScheduleDTOs
{
    public class CreateScheduleDTO
    {
        [Required]
        public string Title { get; set; } = null!;

        [Required]
        public DateTime StartTime { get; set; }

        [Required]
        public DateTime EndTime { get; set; }
    }
}
