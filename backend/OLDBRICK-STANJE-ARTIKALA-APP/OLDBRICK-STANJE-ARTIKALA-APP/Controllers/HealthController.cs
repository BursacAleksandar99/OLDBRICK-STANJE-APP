using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OLDBRICK_STANJE_ARTIKALA_APP.Data;

namespace YourApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HealthController : ControllerBase
    {
        private readonly AppDbContext _db;

        public HealthController(AppDbContext db)
        {
            _db = db;
        }

        [HttpGet("db")]
        public async Task<IActionResult> Db()
        {
            try
            {
                // Ovo radi realnu konekciju do baze
                var canConnect = await _db.Database.CanConnectAsync();

                // Bonus: proveri ko si “u bazi”
                var who = await _db.Database
                    .SqlQueryRaw<string>("select current_user")
                    .ToListAsync();

                return Ok(new
                {
                    canConnect,
                    currentUser = who.FirstOrDefault()
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    error = ex.GetType().FullName,
                    message = ex.Message,
                    inner = ex.InnerException?.Message
                });
            }
        }
    }
}
