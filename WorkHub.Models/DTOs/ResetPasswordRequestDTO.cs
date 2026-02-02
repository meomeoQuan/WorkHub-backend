using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkHub.Models.DTOs
{
    public class ResetPasswordRequestDTO
    {
       
        public string Email { get; set; } = null!;

        public string NewPassword { get; set; } = null!;
    }
}
