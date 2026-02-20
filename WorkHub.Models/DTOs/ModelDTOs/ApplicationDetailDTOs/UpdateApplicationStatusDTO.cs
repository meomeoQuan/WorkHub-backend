using System.ComponentModel.DataAnnotations;

namespace WorkHub.Models.DTOs.ModelDTOs.ApplicationDetailDTOs
{
    public class UpdateApplicationStatusDTO
    {
        [Required]
        public int ApplicationId { get; set; }
        
        [Required]
        public string Status { get; set; } = null!;
    }
}
