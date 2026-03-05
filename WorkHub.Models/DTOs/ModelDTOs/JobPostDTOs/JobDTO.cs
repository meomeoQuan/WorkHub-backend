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
        public decimal MinSalary { get; set; }
        public decimal? MaxSalary { get; set; }
        public string? SalaryCurrency { get; set; }
        public string? SalaryCycle { get; set; }
        public string JobType { get; set; }
        public string? Category { get; set; }
        public string? Description { get; set; }
        public string? Requirements { get; set; }
        public string? Benefits { get; set; }
        public string? WorkTime { get; set; }
    }
}
