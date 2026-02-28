using Microsoft.AspNetCore.Mvc;
using StajTakipUygulaması.Application.Interfaces;

using StajTakipUygulaması.Domain.Entities;

namespace StajTakipUygulaması.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StajTurleriController : ControllerBase
    {
        private readonly IStajService _stajService;

        public StajTurleriController(IStajService stajService)
        {
            _stajService = stajService;
        }

        [HttpGet]
        public async Task<ActionResult<List<StajTuru>>> GetAll()
        {
            var turler = await _stajService.GetStajTurleriAsync();
            return Ok(turler);
        }
    }
}
