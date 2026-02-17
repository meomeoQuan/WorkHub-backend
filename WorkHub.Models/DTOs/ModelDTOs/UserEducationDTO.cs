using System;

namespace WorkHub.Models.DTOs.ModelDTOs
{
    public class UserEducationDTO
    {
        public int Id { get; set; }
        public string School { get; set; } = null!;
        public string Degree { get; set; } = null!;
        public string FieldOfStudy { get; set; } = null!;
        public string StartYear { get; set; } = null!;
        public string? EndYear { get; set; }
        public string? Description { get; set; }
    }
}
