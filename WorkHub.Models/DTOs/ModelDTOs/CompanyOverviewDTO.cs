using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkHub.Models.DTOs.ModelDTOs
{
    public class CompanyOverviewDTO
    {
        public int Id { get; set; }
        public string CompanyName { get; set; } = null!;
        public string? CompanyLogoUrl { get; set; }
        public string CompanyIndustry { get; set; } = null!;
        public string Location { get; set; } = null!;
    }
}
