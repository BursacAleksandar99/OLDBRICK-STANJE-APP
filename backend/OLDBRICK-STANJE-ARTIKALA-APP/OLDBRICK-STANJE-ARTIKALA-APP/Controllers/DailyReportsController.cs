using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OLDBRICK_STANJE_ARTIKALA_APP.DTOs.DailyReports;
using OLDBRICK_STANJE_ARTIKALA_APP.Services.DailyReports;

namespace OLDBRICK_STANJE_ARTIKALA_APP.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DailyReportsController : ControllerBase
    {

        private readonly IDailyReportService _dailyReport;

        public DailyReportsController(IDailyReportService dailyReport)
        {
            _dailyReport = dailyReport;
        }

        [HttpPost("for-date")]
        public async Task<IActionResult> GetorCreateForDate([FromBody] GetOrCreateDailyReportDto dto)
        {
            var result = await _dailyReport.GetorCreateByDateAsync(dto.Datum);
            return Ok(result);
        }
    }
}
