using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkHub.Models.DTOs.ModelDTOs.JobPostDTOs
{
    public class UpdatePostDTO
    {
        public string? Content { get; set; }

        public string? PostImageUrl { get; set; }

        public List<int>? RecruitmentIds { get; set; }
    }
}
