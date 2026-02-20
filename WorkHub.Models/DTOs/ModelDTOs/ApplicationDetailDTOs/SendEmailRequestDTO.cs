using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace WorkHub.Models.DTOs.ModelDTOs.ApplicationDetailDTOs
{
    public class SendEmailRequestDTO
    {
        [Required]
        public int ApplicationId { get; set; }

        [Required]
        public string Subject { get; set; } = null!;

        [Required]
        public string Body { get; set; } = null!;

        public List<IFormFile>? Attachments { get; set; }
    }
}
