using System.ComponentModel.DataAnnotations;

namespace WorkHub.Models.DTOs.ModelDTOs
{
    public class AdminUserCreateDTO
    {
        [Required]
        public string FullName { get; set; } = null!;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;

        [Required]
        [MinLength(6)]
        public string Password { get; set; } = null!;

        [Required]
        public int Role { get; set; }
    }
}
