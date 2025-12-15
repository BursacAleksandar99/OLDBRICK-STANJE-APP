using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OLDBRICK_STANJE_ARTIKALA_APP.DTOs.Users;
using OLDBRICK_STANJE_ARTIKALA_APP.Services.User;

namespace OLDBRICK_STANJE_ARTIKALA_APP.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _user;

        public UsersController(IUserService user)
        {
            _user = user;
        }

        [Authorize(Roles = "ADMIN")]
        [HttpPost]
        public async Task<ActionResult<CreateUserResponseDto>> Create([FromBody] CreateUserRequestDto request)
        {
            try
            {
                var result = await _user.CreateUserAsync(request);
                return Ok(result);
            }catch(ArgumentException ex)
            {
                return BadRequest(new { messsage = ex.Message });
            }catch(InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }
    }
}
