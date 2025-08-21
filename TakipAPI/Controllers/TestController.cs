using Microsoft.AspNetCore.Mvc;

namespace TakipAPI.Controllers   // <- proje adın farklıysa bu namespace'i proje adına göre düzelt
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestController : ControllerBase
    {
        // GET: /api/test
        [HttpGet]
        public IActionResult Get() => Ok(new { ok = true, msg = "Swagger çalışıyor" });
    }
}
