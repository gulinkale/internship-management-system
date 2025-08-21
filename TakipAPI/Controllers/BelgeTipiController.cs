// Proje: StajTakipUygulaması.Api
// Dosya: Controllers/BelgeTipiController.cs
using Microsoft.AspNetCore.Mvc;
using StajTakipUygulaması.Application.Interfaces;

namespace StajTakipUygulaması.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BelgeTipiController : ControllerBase
    {
        private readonly IBelgeTipiService _svc;
        public BelgeTipiController(IBelgeTipiService svc) => _svc = svc;

        // GET api/belgetipi
        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetAll()
        {
            var list = await _svc.GetAllAsync();
            return Ok(list.Select(x => new { id = x.ID, ad = x.Ad }));
        }
    }
}
