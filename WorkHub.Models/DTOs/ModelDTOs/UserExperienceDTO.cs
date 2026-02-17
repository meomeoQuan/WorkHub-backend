using System;

namespace WorkHub.Models.DTOs.ModelDTOs
{
    public class UserExperienceDTO
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string Company { get; set; } = null!;
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? Description { get; set; }
    }
}
