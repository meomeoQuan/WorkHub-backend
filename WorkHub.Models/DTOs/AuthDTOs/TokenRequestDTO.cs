using System.ComponentModel.DataAnnotations;

namespace WorkHub.Models.DTOs.AuthDTOs
{
    public class TokenRequestDTO
    {
        [Required]
        public string AccessToken { get; set; }
        // Note: RefreshToken is retrieved from HttpOnly cookie
    }
}
