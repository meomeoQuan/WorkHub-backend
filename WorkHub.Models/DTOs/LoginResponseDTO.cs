using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkHub.Models.DTOs
{
    public class LoginResponseDTO
    {
        public string ? Token { get; set; }
        public  UserDTO ? UserDTO { get; set; }
    }
}
