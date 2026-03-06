using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkHub.Models.DTOs.ModelDTOs.JobDTOs
{
    public class CreateJobRequestDTO
    {
        // ===== Job =====
        public string JobTitle { get; set; } = null!;
        public string? Location { get; set; }
        public string? Category { get; set; }
        public string? JobType { get; set; }              // name or id as string
        public string? WorkTime { get; set; }
        public decimal MinSalary { get; set; }
        public decimal? MaxSalary { get; set; }
        public string? SalaryCurrency { get; set; }
        public string? SalaryCycle { get; set; }
        public string? SalaryRange { get; set; }

        // ===== Description =====
        public string? Description { get; set; }

        // Multiline textarea (one per line)
        public string? Requirements { get; set; }
        public string? Benefits { get; set; }

        // Optional images
        //public List<IFormFile>? JobImages { get; set; }
    }

}
