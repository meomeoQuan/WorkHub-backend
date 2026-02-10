using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkHub.Models.DTOs.AuthDTOs
{
    public class ValidateResetTokenRequestDTO
    {
        public string Token { get; set; } = string.Empty;
    }
}
