using System;

namespace WorkHub.Models.DTOs.ModelDTOs.ApplicationDTOs
{
    public class ApplicationDTO
    {
        public int Id { get; set; }
        public int ApplicantId { get; set; }
        public string ApplicantName { get; set; }
        public string ApplicantEmail { get; set; }
        public string ApplicantAvatar { get; set; }
        public string JobTitle { get; set; }
        public string Status { get; set; }
        public DateTime? AppliedDate { get; set; }
    }
}
