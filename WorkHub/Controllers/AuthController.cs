using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using WorkHub.Business.Service.IService;
using WorkHub.Models.DTOs;
using WorkHub.Models.Models;

namespace WorkHub.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IGoogleAuthService _googleAuthService;

        public AuthController(IAuthService authService, IGoogleAuthService googleAuthService)
        {
            _authService = authService;
            _googleAuthService = googleAuthService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDTO registerRequest)
        {
            try
            {

                await _authService.RegisterAsync(registerRequest);

                var response = ApiResponse<object>.CreatedAt(null, "User registered successfully");

                return StatusCode(response.StatusCode, response);
            }
            catch (Exception ex)
            {
                var response = ApiResponse<object>.NoContent(ex.Message);

                return StatusCode(response.StatusCode, response);

            }

        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDTO loginRequest)
        {
            try
            {
                var token = await _authService.LoginAsync(loginRequest);


                var response = ApiResponse<object>.Ok(token, "Login successful");
                return Ok(response);
            }
            catch (UnauthorizedAccessException ex)
            {
                var response = ApiResponse<object>.BadRequest(ex.Message);
                return BadRequest(response);
            }

        }

        [HttpPost("google")]
        public async Task<IActionResult> GoogleLogin([FromBody] string idToken)
        {
            try
            {
                var token = await _authService.GoogleLoginAsync(idToken);

                var response = ApiResponse<object>.Ok(token, "Google login successful");

                return Ok(response);
            }
            catch (Exception ex)
            {
                var response = ApiResponse<object>.BadRequest(ex.Message);
                return BadRequest(response);
            }

        }
    }
}
