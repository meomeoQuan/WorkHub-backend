using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkHub.Models.DTOs.ModelDTOs.ApplicationDTOs
{
    public class ApplicationFilterDTO
    {
        public string? SearchTerm { get; set; }
        public int? JobId { get; set; }
        public string? Status { get; set; }
    }
}
