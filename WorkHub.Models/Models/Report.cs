using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WorkHub.Models.Models
{
    public class Report
    {
        [Key]
        public int Id { get; set; }

        public int ReporterId { get; set; }
        [ForeignKey("ReporterId")]
        [InverseProperty("ReportsGiven")]
        public virtual User Reporter { get; set; }

        public int ReportedUserId { get; set; }
        [ForeignKey("ReportedUserId")]
        [InverseProperty("ReportsReceived")]
        public virtual User ReportedUser { get; set; }

        public string Reason { get; set; }

        public string Description { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// e.g. Pending, Reviewed, Dismissed, action_taken
        /// </summary>
        public string Status { get; set; } = "Pending";
    }
}
