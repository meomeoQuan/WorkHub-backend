using System;

namespace WorkHub.Models.DTOs.ModelDTOs.MyApplicationDTOs
{
    public class MyApplicationDTO
    {
        public int Id { get; set; }
        public int JobId { get; set; }
        public string JobName { get; set; }
        public string Company { get; set; }
        public string CompanyLogo { get; set; }
        public string Location { get; set; }
        public string JobType { get; set; }
        public string Salary { get; set; }
        public DateTime? AppliedDate { get; set; }
        public string Status { get; set; }
    }
}
