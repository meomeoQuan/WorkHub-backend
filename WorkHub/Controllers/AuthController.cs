using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using WorkHub.Business.Service.IService;
using WorkHub.Models.DTOs;
using WorkHub.Models.DTOs.AuthDTOs;
using WorkHub.Models.Models;

namespace WorkHub.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IGoogleAuthService _googleAuthService;
        private readonly IEmailService _emailService;

        public AuthController(IAuthService authService, IGoogleAuthService googleAuthService, IEmailService emailService)
        {
            _authService = authService;
            _googleAuthService = googleAuthService;
            _emailService = emailService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDTO registerRequest)
        {
            try
            {

                if (registerRequest == null)
                {
                    return BadRequest(ApiResponse<object>.BadRequest("Registeration data is required"));
                }

                var user = await _authService.RegisterAsync(registerRequest);

                if (user == null)
                {
                    return BadRequest(ApiResponse<object>.BadRequest("Registration failed !"));
                }

                var response = ApiResponse<object>.CreatedAt(user, "User registered successfully");

                return CreatedAtAction(nameof(Register), response);
            }
            catch (Exception ex)
            {
                var errorResponse = ApiResponse<object>.Error(500, "An error occurred during registration", ex.Message);
                return StatusCode(500, errorResponse);

            }

        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDTO loginRequestDTO)
        {
            try
            {
                if (loginRequestDTO == null)
                {
                    return BadRequest(ApiResponse<object>.BadRequest("login data is required"));
                }

                var (loginData, refreshToken) = await _authService.LoginWithRefreshAsync(loginRequestDTO);

                if (loginData == null)
                {
                    return BadRequest(ApiResponse<object>.BadRequest("Login failed !"));
                }

                SetRefreshTokenCookie(refreshToken);

                var response = ApiResponse<LoginResponseDTO>.Ok(loginData, "Login successful");
                return Ok(response);
            }
            catch (Exception ex)
            {
                var errorResponse = ApiResponse<object>.Error(500, "An error occurred during login", ex.Message);
                return StatusCode(500, errorResponse);
            }
        }

      

        [HttpPost("google")]
        public async Task<IActionResult> GoogleLogin([FromBody] string authCode)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(authCode))
                {
                    return BadRequest(ApiResponse<object>.BadRequest("Authorization code is required"));
                }

                var (loginData, refreshToken) = await _authService.GoogleLoginWithRefreshAsync(authCode);

                if (loginData == null)
                {
                    return BadRequest(ApiResponse<object>.BadRequest("Google login failed"));
                }

                SetRefreshTokenCookie(refreshToken);

                var response = ApiResponse<LoginResponseDTO>.CreatedAt(loginData, "Login successful");
                return Ok(response);
            }
            catch (Exception ex)
            {
                var errorResponse = ApiResponse<object>.Error(500, "An error occurred during Google login", ex.Message);
                return StatusCode(500, errorResponse);
            }
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] TokenRequestDTO request)
        {
            try
            {
                var refreshToken = Request.Cookies["refreshToken"];
                if (string.IsNullOrEmpty(refreshToken))
                {
                    return Unauthorized(ApiResponse<object>.Error(401, "Refresh token is missing. Please log in again."));
                }

                var (loginData, newRefreshToken) = await _authService.RefreshTokenAsync(request.AccessToken, refreshToken);

                SetRefreshTokenCookie(newRefreshToken);

                return Ok(ApiResponse<LoginResponseDTO>.Ok(loginData, "Token refreshed successfully"));
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ApiResponse<object>.Error(401, ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.Error(500, "Token refresh failed", ex.Message));
            }
        }

        private void SetRefreshTokenCookie(string refreshToken)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true, 
                SameSite = SameSiteMode.None,
                Expires = DateTime.UtcNow.AddDays(7)
            };
            Response.Cookies.Append("refreshToken", refreshToken, cookieOptions);
        }

        [HttpPost("verify-email")]
        public async Task<IActionResult> EmailVerify([FromBody] VerifyEmailRequestDTO request)
        {
            if (string.IsNullOrWhiteSpace(request.Token))
            {
                return BadRequest(
                    ApiResponse<object>.BadRequest("Token is required")
                );
            }

          

            await _emailService.VerifyEmailAsync(request);

            return Ok(
                ApiResponse<object>.Ok(null, "Email verified successfully")
            );
        }


        [HttpPost("resend-email")]
        public async Task<IActionResult> ResendEmailConfirmation([FromBody] EmailResendConfirmationDTO request)
        {
            if (string.IsNullOrWhiteSpace(request.Email))
            {
                return BadRequest(
                    ApiResponse<object>.BadRequest("Email is required")
                );
            }

            await _authService.ResendEmailConfirmationAsync(request);

            return Ok(
                ApiResponse<object>.Ok(null, "Email sent successfully")
            );
        }


        [HttpPost("forgot-password")]
        public async Task<IActionResult> SendPasswordEmailChanging([FromBody] EmailResendConfirmationDTO request)
        {
            if (string.IsNullOrWhiteSpace(request.Email))
            {
                return BadRequest(
                    ApiResponse<object>.BadRequest("Email is required")
                );
            }

            await _authService.SendEmailPasswordChaningRequestAsync(request);

            return Ok(
                ApiResponse<object>.Ok(null, "Email sent successfully")
            );
        }

        [HttpPost("password-reset")]
        public async Task<IActionResult> PasswordChangingRequest([FromBody] ResetPasswordRequestDTO request)
        {
            if (string.IsNullOrWhiteSpace(request.Email))
            {
                return BadRequest(
                    ApiResponse<object>.BadRequest("Email is required")
                );
            }


            if (string.IsNullOrWhiteSpace(request.NewPassword))
            {
                return BadRequest(
                    ApiResponse<object>.BadRequest("NewPassword is required")
                );
            }

            await _authService.ResetPasswordAsync(request);

            return Ok(
                ApiResponse<object>.Ok(null, "Password reset successfully")
            );
        }


        [HttpPost("validate-reset-token")]
        public async Task<IActionResult> ValidateResetToken(
     [FromBody] ValidateResetTokenRequestDTO request)
        {
            if (string.IsNullOrWhiteSpace(request.Token))
            {
                return BadRequest(
                    ApiResponse<object>.BadRequest("TOKEN_INVALID")
                );
            }

            var isExpired = await _authService.IsTokenExpired(request.Token);

            if (isExpired)
            {
                return BadRequest(
                    ApiResponse<object>.BadRequest("TOKEN_EXPIRED")
                );
            }

            return Ok(
                ApiResponse<object>.Ok(null, "valid")
            );
        }




        //[Authorize]
        //[HttpGet("me")]
        //public IActionResult Me()
        //{
        //    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        //    if (userId == null)
        //        return Unauthorized();

        //    var user = _unitOfWork.UserRepository.GetById(int.Parse(userId));

        //    if (user == null)
        //        return NotFound();

        //    return Ok(ApiResponse<UserDTO>.Ok(_mapper.Map<UserDTO>(user)));
        //}

    }
}
