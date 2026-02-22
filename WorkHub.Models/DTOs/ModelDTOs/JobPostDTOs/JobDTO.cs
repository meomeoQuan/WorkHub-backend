using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkHub.Models.DTOs.ModelDTOs.JobPostDTOs
{
    public class JobDTO
    {
        public int Id { get; set; }
        public string JobName { get; set; }
        public string Location { get; set; }
        public string Salary { get; set; }
        public string JobType { get; set; }
        public string? Category { get; set; }
    }
}
