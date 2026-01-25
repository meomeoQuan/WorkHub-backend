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
        Task RegisterAsync(RegisterRequestDTO RegisterRequest);
        Task<string> LoginAsync(LoginRequestDTO loginRequest);

        Task<string> GoogleLoginAsync(string idToken);
    }
}
