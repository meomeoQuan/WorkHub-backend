using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkHub.Models.DTOs.ModelDTOs.JobPostDTOs
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
        public List<JobDTO> Jobs { get; set; }


        // Engagement
        public int LikeCount { get; set; }
        public int CommentCount { get; set; }
        public bool IsLiked { get; set; }
    }

}
