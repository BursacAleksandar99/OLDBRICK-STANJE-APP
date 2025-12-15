using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OLDBRICK_STANJE_ARTIKALA_APP.DTOs.Auth;
using OLDBRICK_STANJE_ARTIKALA_APP.Services.Auth;

namespace OLDBRICK_STANJE_ARTIKALA_APP.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {

        private readonly IAuthService _auth;

        public AuthController(IAuthService auth)
        {
            _auth = auth;

        }

        [HttpPost("login")]
        public async Task<ActionResult<LoginResponseDto>> Login([FromBody] LoginRequestDto request)
        {
            try
            {
                var result = await _auth.LoginAsync(request);
                return Ok(result);

            }catch(ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch(UnauthorizedAccessException ex)
            {
                return Unauthorized(new {message = ex.Message});    
            }
        }

        [HttpGet("hash")]
        public IActionResult Hash([FromServices] IPasswordService passwords, [FromQuery] string password)
        {
            var hash = passwords.HashPassword(password);
            return Ok(new { hash });
        }
    }
}
