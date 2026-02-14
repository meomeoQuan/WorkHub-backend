using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkHub.Models.Models
{
    public class CommentLikes
    {
        public int UserId { get; set; }
        public int CommentId { get; set; }
        public DateTime ? CreatedAt { get; set; }


        public User User { get; set; } 
        public Comment Comment { get; set; }
    }
}
