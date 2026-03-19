using System.ComponentModel.DataAnnotations;

namespace WorkHub.Models.Models
{
    public class ReportCategory
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        public string? Description { get; set; }
    }
}
