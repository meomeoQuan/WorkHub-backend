using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkHub.Models.DTOs.ModelDTOs.JobsDTOs
{
    public class JobDTO
    {
        public int Id { get; set; }
        public string JobName { get; set; }
        public string Location { get; set; }
        public string Salary { get; set; }
        public string JobType { get; set; }
        public string? ExperienceLevel { get; set; }
        public string? Category { get; set; }
        public string? WorkSetting { get; set; }
        public string? CompanySize { get; set; }
    }

}
