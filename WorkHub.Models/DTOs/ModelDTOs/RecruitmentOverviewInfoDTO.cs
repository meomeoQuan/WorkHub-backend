using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkHub.Models.DTOs.ModelDTOs
{
    public class RecruitmentOverviewInfoDTO
    {
        public int Id { get; set; }
        public string JobName { get; set; } = null!;
        public string JobType { get; set; } = null!;
        public string? Location { get; set; }
        public string? Salary { get; set; }
        public bool Status { get; set; }

        public string description { get; set; } = null!;
    }
}
