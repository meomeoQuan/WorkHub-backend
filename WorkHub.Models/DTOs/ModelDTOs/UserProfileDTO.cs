using System;
using System.Collections.Generic;

namespace WorkHub.Models.DTOs.ModelDTOs
{
    public class UserProfileDTO
    {
        // Basic User Info
        public int Id { get; set; }
        public string FullName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? Phone { get; set; }
        public string? AvatarUrl { get; set; }
        public string? Location { get; set; }
        public int Role { get; set; }
        public string? Provider { get; set; }

        // Extended Profile Info (from UserDetail)
        public string? Bio { get; set; }
        public string? Title { get; set; } // Mapped from JobPreference or new field if needed, using JobPreference for now based on context
        public string? About { get; set; } // Often same as Bio
        public string? Description { get; set; }
        public string? CvUrl { get; set; }
        
        // Company Info (for Recruiters/Companies)
        public string? Website { get; set; }
        public string? CompanySize { get; set; }
        public int? FoundedYear { get; set; }
        public string? Industry { get; set; } // Mapped from IndustryFocus

        // Collections
        public List<string> Skills { get; set; } = new List<string>(); // Skills is stored as string in DB, will need split
        public List<UserExperienceDTO> Experiences { get; set; } = new List<UserExperienceDTO>();
        public List<UserEducationDTO> Educations { get; set; } = new List<UserEducationDTO>();
        public List<UserScheduleDTO> Schedules { get; set; } = new List<UserScheduleDTO>();
    }
}
