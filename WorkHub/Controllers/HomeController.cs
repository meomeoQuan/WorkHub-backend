using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WorkHub.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        [HttpGet("public")]
        public IActionResult Public()
       => Ok("Anyone can see this");

        [Authorize]
        [HttpGet("private")]
        public IActionResult Private()
            => Ok("You are authenticated 🔥");
    }
}
