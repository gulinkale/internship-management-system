// Proje: StajTakipUygulaması.Api
// Dosya: Controllers/StajController.cs
using Microsoft.AspNetCore.Mvc;
using StajTakipUygulaması.Application.Interfaces; // IStajService
using StajTakipUygulaması.Models;                // Staj entity

namespace StajTakipUygulaması.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StajController : ControllerBase
    {
        private readonly IStajService _svc;
        public StajController(IStajService svc) => _svc = svc;

        // GET api/staj
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Staj>>> GetAll()
            => Ok(await _svc.GetAllAsync());

        // GET api/staj/aktif
        [HttpGet("aktif")]
        public async Task<ActionResult<IEnumerable<Staj>>> GetAktif()
            => Ok(await _svc.GetAktifStajlarAsync());

        // GET api/staj/tamamlanmis
        [HttpGet("tamamlanmis")]
        public async Task<ActionResult<IEnumerable<Staj>>> GetTamamlanmis()
            => Ok(await _svc.GetTamamlanmisStajlarAsync());

        // GET api/staj/5
        [HttpGet("{id:int}")]
        public async Task<ActionResult<Staj>> Get(int id)
        {
            var s = await _svc.GetByIdAsync(id);
            return s is null ? NotFound() : Ok(s);
        }

        // POST api/staj   (body: Staj)
        [HttpPost]
        public async Task<ActionResult<object>> Create([FromBody] Staj staj)
        {
            if (staj is null) return BadRequest("Geçersiz istek.");
            await _svc.AddAsync(staj);
            return CreatedAtAction(nameof(Get), new { id = staj.ID }, new { id = staj.ID });
        }

        // PUT api/staj/5   (body: Staj)
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] Staj staj)
        {
            if (staj is null || staj.ID != id) return BadRequest("Geçersiz istek.");
            await _svc.UpdateAsync(staj);
            return NoContent();
        }

        // DELETE api/staj/5
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _svc.DeleteAsync(id);
            return NoContent();
        }
    }
}
