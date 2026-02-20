using System;

namespace WorkHub.Models.DTOs.ModelDTOs.ScheduleDTOs
{
    public class ScheduleViewDTO
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int UserId { get; set; }
    }
}
