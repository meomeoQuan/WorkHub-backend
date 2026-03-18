using System;

namespace WorkHub.Models.DTOs.ModelDTOs
{
    public class AdminPostDTO
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; } = null!;
        public string Title { get; set; } = null!;
        public string Category { get; set; } = null!;
        public string Status { get; set; } = null!;
        public DateTime Date { get; set; }
    }
}
