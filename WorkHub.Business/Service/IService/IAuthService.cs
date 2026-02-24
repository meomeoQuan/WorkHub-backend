using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkHub.Models.DTOs.AuthDTOs;
using WorkHub.Models.DTOs.ModelDTOs;

namespace WorkHub.Business.Service.IService
{
    public interface IAuthService
    {
        Task<UserDTO?> RegisterAsync(RegisterRequestDTO RegisterRequest);
        Task<LoginResponseDTO?> LoginAsync(LoginRequestDTO loginRequest);
        Task<(LoginResponseDTO loginData, string refreshToken)> LoginWithRefreshAsync(LoginRequestDTO request);
        Task<(LoginResponseDTO loginData, string refreshToken)> GoogleLoginWithRefreshAsync(string authCode);
        Task<(LoginResponseDTO loginData, string refreshToken)> RefreshTokenAsync(string expiredAccessToken, string refreshToken);

        Task ResendEmailConfirmationAsync(EmailResendConfirmationDTO email);

        Task SendEmailPasswordChaningRequestAsync(EmailResendConfirmationDTO email);

        Task ResetPasswordAsync(ResetPasswordRequestDTO resetPasswordRequestDTO);

        Task<bool> IsTokenExpired(string token);
    }
}
