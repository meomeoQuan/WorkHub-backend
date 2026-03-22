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
        public string? PhoneNumber { get; set; }
        public string? Bio { get; set; }
        public string? Location { get; set; }
        public string? School { get; set; }
        public int? TotalJobs { get; set; }
        public int? TotalPosts { get; set; }
        public double? Rating { get; set; }
        public string? IndustryFocus { get; set; }
        public string? Website { get; set; }
        public string? CompanySize { get; set; }
        public int? FoundedYear { get; set; }
        public string? GoogleMapsEmbedUrl { get; set; }
        public bool? IsVerified { get; set; }
    }
}
