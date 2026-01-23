using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WorkHub.Business.Service.IService;
using WorkHub.Models.DTOs;

namespace WorkHub.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(IAuthService authService) : ControllerBase
    {
        private readonly IAuthService _authService = authService;

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest registerRequest)
        {
            try {

                await _authService.RegisterAsync(registerRequest);

                var response = ApiResponse<object>.CreatedAt(null, "User registered successfully");

                return StatusCode(response.StatusCode, response);
            }
            catch(Exception ex) 
            {
                var response = ApiResponse<object>.NoContent(ex.Message);

                return StatusCode(response.StatusCode, response);
            
            }
           
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
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
    }
}
