using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkHub.Models.DTOs.ModelDTOs.HomeDTOs
{
    public class UserFeatureDTO
    {
        public string FullName { get; set; } = null!;
        public double ? ratingCount { get; set; }

        public int ActiveJob { get; set; }

    }
}
