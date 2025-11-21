using Microsoft.AspNetCore.Mvc;

namespace CafeteriaApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HealthController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetHealth()
        {
            return Ok(new { status = "API is running", timestamp = DateTime.UtcNow });
        }
    }
}
