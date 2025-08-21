using Application.DTOs;
using Microsoft.AspNetCore.Mvc;
using StajTakipUygulaması.Application.DTOs;
using StajTakipUygulaması.Application.Interfaces;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace StajTakipUygulaması.Api.Controllers.v1
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class RaporController : ControllerBase
    {
        private readonly IRaporService _raporService;

        public RaporController(IRaporService raporService)
        {
            _raporService = raporService;
        }

        // GET /api/v1/rapor?arama=...&belgeTipId=0
        [HttpGet]
        public async Task<ActionResult<List<RaporStajyerDto>>> Ara(
            [FromQuery] string arama,
            [FromQuery] int belgeTipId = 0,
            CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(arama))
                return BadRequest(new { message = "arama parametresi zorunludur." });

            var data = await _raporService.AraAsync(arama.Trim(), belgeTipId, ct);
            return Ok(data);
        }
    }
}
