using System;
using System.Collections.Generic;
using WorkHub.Models.DTOs.ModelDTOs;

namespace WorkHub.Models.DTOs.ModelDTOs.ApplicationDetailDTOs
{
    public class ApplicationDetailDTO
    {
        // Candidate Info
        public int ApplicantId { get; set; }
        public string ApplicantName { get; set; }
        public string ApplicantEmail { get; set; }
        public string ApplicantPhone { get; set; }
        public string ApplicantLocation { get; set; }
        public string ApplicantAvatar { get; set; }
        
        // Application Info
        public DateTime? AppliedDate { get; set; }
        public string? CoverLetter { get; set; }
        public string? CvUrl { get; set; }
        public string Status { get; set; }

        // Job Info
        public int RecruitmentId { get; set; }
        public string JobTitle { get; set; }
        public string CompanyName { get; set; }
        public string JobLocation { get; set; }

        // Collections (Simplified or Full DTOs depending on need, using existing DTOs for reuse)
        public List<UserEducationDTO> Educations { get; set; }
        public List<UserExperienceDTO> Experiences { get; set; }
    }
}
