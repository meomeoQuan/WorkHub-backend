using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkHub.Models.DTOs.ModelDTOs
{
    public class EmployerOverviewDTO
    {
        public int Id { get; set; }
        public string? Bio { get; set; }

        public string FullName { get; set; } = null!;
        public string Email { get; set; } = null!;
    }

}
