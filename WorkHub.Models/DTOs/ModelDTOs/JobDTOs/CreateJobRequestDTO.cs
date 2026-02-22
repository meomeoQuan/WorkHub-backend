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
        public int JobType { get; set; }              // enum
        public string? WorkTime { get; set; }
        public string? SalaryRange { get; set; }

        // ===== Description =====
        public string? JobDescription { get; set; }

        // Multiline textarea (one per line)
        public string? Requirements { get; set; }
        public string? Benefits { get; set; }

        // Optional images
        //public List<IFormFile>? JobImages { get; set; }
    }

}
