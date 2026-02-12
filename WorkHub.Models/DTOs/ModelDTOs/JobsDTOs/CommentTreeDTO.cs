using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkHub.Models.DTOs.ModelDTOs.JobsDTOs
{
    public class CommentTreeDTO
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string? Content { get; set; }
        public DateTime? CreatedAt { get; set; }

        public List<CommentTreeDTO> Replies { get; set; } = new();
    }
}
