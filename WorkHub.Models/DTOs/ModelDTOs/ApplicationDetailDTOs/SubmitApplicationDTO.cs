using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace WorkHub.Models.DTOs.ModelDTOs.ApplicationDetailDTOs
{
    public class SubmitApplicationDTO
    {
        [Required]
        public int RecruitmentId { get; set; }

        public string? CoverLetter { get; set; }

        public IFormFile? CvFile { get; set; }

        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
    }
}
