using System.ComponentModel.DataAnnotations;

namespace WorkHub.Models.DTOs.ModelDTOs
{
    public class AdminUserUpdateDTO
    {
        [Required]
        public string FullName { get; set; } = null!;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;

        [Required]
        public int Role { get; set; }

        public string PaymentPlan { get; set; } = "free";

        [Required]
        public string Status { get; set; } = "active";
        public int? TotalJobs { get; set; }
        public int? TotalPosts { get; set; }
        public double? Rating { get; set; }
    }
}
