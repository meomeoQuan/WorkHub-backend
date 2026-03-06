using System.ComponentModel.DataAnnotations;

namespace WorkHub.Models.DTOs.ModelDTOs.JobPostDTOs
{
    public class UpdateCommentDTO
    {
        [Required]
        public string Content { get; set; }
    }
}
