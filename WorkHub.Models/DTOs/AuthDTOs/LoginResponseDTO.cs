using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkHub.Models.DTOs.ModelDTOs;

namespace WorkHub.Models.DTOs.AuthDTOs
{
    public class LoginResponseDTO
    {
        public string ? Token { get; set; }
        public  UserDTO ? UserDTO { get; set; }
    }
}
