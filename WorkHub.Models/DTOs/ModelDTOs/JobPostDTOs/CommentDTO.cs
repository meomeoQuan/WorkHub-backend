using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkHub.Models.DTOs.ModelDTOs.JobPostDTOs
{
    public class CommentDTO
    {
        public int Id { get; set; }

        public int PostId { get; set; }

        public int UserId { get; set; }
        public string UserUrl { get; set; }
        public string UserName { get; set; }

        public int? ParentCommentId { get; set; }

        public string? Content { get; set; }

        public DateTime? CreatedAt { get; set; }
    }

}
