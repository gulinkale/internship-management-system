using Microsoft.AspNetCore.Mvc;
using StajTakipUygulaması.Application.Interfaces;
using StajTakipUygulaması.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StajTakipUygulaması.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StajTurleriController : ControllerBase
    {
        private readonly IStajyerService _stajyerService;

        public StajTurleriController(IStajyerService stajyerService)
        {
            _stajyerService = stajyerService;
        }

        [HttpGet]
        public async Task<ActionResult<List<StajTuru>>> GetAll()
        {
            var turler = await _stajyerService.GetStajTurleriAsync();
            if (turler == null || turler.Count == 0)
                return NotFound("Staj türleri bulunamadı.");

            return Ok(turler);
        }
    }
}
