using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkHub.Models.DTOs.ModelDTOs
{
    public class CompanyDetailDTO
    {
        public int Id { get; set; }
        public string CompanyName { get; set; } = null!;
        public string? CompanyWebUrl { get; set; }
        public string? CompanyLogoUrl { get; set; }
        public string CompanyIndustry { get; set; } = null!;
        public int CompanySize { get; set; }
        public string Location { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? Info { get; set; }
    }

}
