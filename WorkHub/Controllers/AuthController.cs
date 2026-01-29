using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
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
        public async Task<IActionResult> GoogleLogin([FromBody] string authCode)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(authCode))
                {
                    return BadRequest(
                        ApiResponse<object>.BadRequest("Authorization code is required")
                    );
                }

                var loginData = await _authService.GoogleLoginAsync(authCode);

                if (loginData == null)
                {
                    return BadRequest(
                        ApiResponse<object>.BadRequest("Google login failed")
                    );
                }

                var response =
                    ApiResponse<LoginResponseDTO>.CreatedAt(loginData, "Login successful");

                return Ok(response);
            }
            catch (Exception ex)
            {
                var errorResponse =
                    ApiResponse<object>.Error(
                        500,
                        "An error occurred during Google login",
                        ex.Message
                    );

                return StatusCode(500, errorResponse);
            }
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
