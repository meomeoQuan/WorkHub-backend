using System;

namespace WorkHub.Models.DTOs.ModelDTOs
{
    public class AdminOrderDTO
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; } = null!;
        public string Plan { get; set; } = null!;
        public long Amount { get; set; }
        public string Status { get; set; } = null!;
        public DateTime Date { get; set; }
    }
}
