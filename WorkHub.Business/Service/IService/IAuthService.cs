using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkHub.Models.DTOs;

namespace WorkHub.Business.Service.IService
{
    public interface IAuthService
    {
        Task<UserDTO?> RegisterAsync(RegisterRequestDTO RegisterRequest);
        Task<LoginResponseDTO?> LoginAsync(LoginRequestDTO loginRequest);

        Task<LoginResponseDTO?> GoogleLoginAsync(string authCode);
    }
}
