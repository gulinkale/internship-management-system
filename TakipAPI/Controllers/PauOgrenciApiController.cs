using Microsoft.AspNetCore.Mvc;
using StajTakipUygulaması.Application.DTOs;
using StajTakipUygulaması.Application.Interfaces;

namespace StajTakipUygulamasi.Api.Controllers.v1
{
    [ApiController]
    [Route("api/v1/[controller]")] // => /api/v1/stajyerler
    public class StajyerlerController : ControllerBase
    {
        private readonly IStajyerService _service;

        public StajyerlerController(IStajyerService service)
        {
            _service = service;
        }

        // GET /api/v1/stajyerler
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var list = await _service.GetAllAsync();
            return Ok(list);
        }

        // GET /api/v1/stajyerler/{id}
        [HttpGet("{id:int}")]
        public async Task<IActionResult> Get(int id)
        {
            var dto = await _service.GetByIdAsync(id);
            return dto is null ? NotFound() : Ok(dto);
        }

        // POST /api/v1/stajyerler
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] StajyerCreateDto dto)
        {
            if (!ModelState.IsValid) return ValidationProblem(ModelState);
            var newId = await _service.AddAsync(dto);
            return CreatedAtAction(nameof(Get), new { id = newId }, new { id = newId });
        }

        // PUT /api/v1/stajyerler/{id}
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] StajyerUpdateDto dto)
        {
            if (!ModelState.IsValid) return ValidationProblem(ModelState);

            // DTO’da ID varsa route ile eşitle
            var idProp = typeof(StajyerUpdateDto).GetProperty("ID") ?? typeof(StajyerUpdateDto).GetProperty("Id");
            if (idProp != null) idProp.SetValue(dto, id);

            await _service.UpdateAsync(dto);
            return NoContent();
        }

        // DELETE /api/v1/stajyerler/{id}
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _service.DeleteAsync(id);
            return NoContent();
        }
    }
}
