using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkHub.Models.DTOs.ModelDTOs
{
    public class RecruitmentDetailInfoDTO
    {
        public int Id { get; set; }
        public string JobName { get; set; } = null!;
        public string JobType { get; set; } = null!;
        public string? Location { get; set; }
        public string? Salary { get; set; }
        public bool Status { get; set; }
        public DateTime CreatedAt { get; set; }

        public string Description { get; set; } = null!;
        public string? Schedule { get; set; }
        public string UserName { get; set; } = null!;
        public string? Avatar { get; set; }
        public string? Requirements { get; set; }
        public string? Benefits { get; set; }
        public string? ExperienceLevel { get; set; }
        public string? WorkSetting { get; set; }
        public string? Category { get; set; }
        public string? CompanyBio { get; set; }
        public double CompanyRating { get; set; }
        public string? CompanyIndustry { get; set; }
    }
}
