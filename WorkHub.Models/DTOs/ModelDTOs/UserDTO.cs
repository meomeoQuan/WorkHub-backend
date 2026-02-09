using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkHub.Models.DTOs.ModelDTOs
{
    public class UserDTO
    {
      
        public int Id { get; set; }

        public string Email { get; set; } = null!;

        public string? Phone { get; set; }

        public string Password { get; set; } = null!;

        public int Role { get; set; }


        public double? Rating { get; set; }

        public string FullName { get; set; } = null!;

        public string? AvatarUrl { get; set; }

        public string? Location { get; set; }

        public int Age { get; set; }

        public string? Provider { get; set; }

    }
}
