using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.Data;
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
                var loginData = await _authService.LoginAsync(loginRequestDTO);

                if(loginData == null)
                {
                    return BadRequest(ApiResponse<object>.BadRequest("Login failed !"));
                }

                var response = ApiResponse<object>.Ok(loginData, "Login successful");
                return Ok(response);

            }
            catch (Exception ex)
            {
                var errorResponse = ApiResponse<object>.Error(500, "An error occurred during login", ex.Message);
                return StatusCode(500, errorResponse);
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
