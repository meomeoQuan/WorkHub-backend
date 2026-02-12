using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkHub.Models.DTOs.ModelDTOs.JobsDTOs
{
    public class JobPostDTO
    {
        // Post
        public int PostId { get; set; }
        public string? Header { get; set; }
        public string? Content { get; set; }

        public string? PostImage { get; set; }
        public DateTime? CreatedAt { get; set; }

        // User / Company
        public int UserId { get; set; }
        public string FullName { get; set; } = null!;
        public double? Rating { get; set; }
        public string? AvatarUrl { get; set; }

        // Job info

        public int JobId { get; set; }  
        public string? JobLocation { get; set; }
        public string? JobSalaryRange { get; set; }
        public string? JobType { get; set; }

        public string? JobName { get; set; }

        // Engagement
        public int LikeCount { get; set; }
        public int CommentCount { get; set; }
    }

}
