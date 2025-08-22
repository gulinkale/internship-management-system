using StajTakipUygulaması.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using StajTakipUygulaması.Application.DTOs;

namespace StajTakipUygulaması.Api.Controllers.v1
{
    [ApiController]
    [Route("api/v1/[controller]")] // => api/v1/basvurular
    public class BasvuruDegerlendirApiController : ControllerBase
    {
        private readonly IBasvuruService _service;

        public BasvuruDegerlendirApiController(IBasvuruService service)
        {
            _service = service;
        }

        /// <summary> Tüm başvurular (liste) </summary>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var list = await _service.TumBasvurulariGetirAsync(); // List<BasvuruListDto>
            return Ok(list);
        }

        /// <summary> Başvuru detayı </summary>
        [HttpGet("{id:int}")]
        public async Task<IActionResult> Get(int id)
        {
            var dto = await _service.BasvuruDetayAsync(id); // BasvuruDto?
            return dto is null ? NotFound() : Ok(dto);
        }

        /// <summary> Başvuru oluştur </summary>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] BasvuruCreateDto dto)
        {
            if (!ModelState.IsValid) return ValidationProblem(ModelState);
            var newId = await _service.BasvuruOlusturAsync(dto); // int
            // Detay endpointine location header ile dönelim
            return CreatedAtAction(nameof(Get), new { id = newId }, new { id = newId });
        }

        /// <summary> Başvuru güncelle </summary>
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] BasvuruUpdateDto dto)
        {
            if (!ModelState.IsValid) return ValidationProblem(ModelState);

            // DTO içinde Id alanı varsa route ile tutarlı hale getir
            var dtoIdProp = typeof(BasvuruUpdateDto).GetProperty("Id");
            if (dtoIdProp != null) dtoIdProp.SetValue(dto, id);

            var ok = await _service.BasvuruGuncelleAsync(dto); // bool
            return ok ? NoContent() : NotFound();
        }

        /// <summary> Başvuruyu onayla </summary>
        [HttpPost("{id:int}/onayla")]
        public async Task<IActionResult> Approve(int id)
        {
            var ok = await _service.BasvuruOnaylaAsync(id); // bool
            return ok ? Ok(new { message = "Başvuru onaylandı." }) : NotFound();
        }

        /// <summary> Başvuruyu reddet (DTO içinde Id ve Neden beklenir) </summary>
        [HttpPost("reddet")]
        public async Task<IActionResult> Reject([FromBody] BasvuruReddetDto dto)
        {
            if (!ModelState.IsValid) return ValidationProblem(ModelState);
            var ok = await _service.BasvuruReddetAsync(dto); // bool
            return ok ? Ok(new { message = "Başvuru reddedildi." }) : NotFound();
        }

        /// <summary> Reddedilmiş başvuruyu tekrar Beklemede durumuna al </summary>
        [HttpPost("{id:int}/beklemeye-al")]
        public async Task<IActionResult> MoveToPending(int id)
        {
            var ok = await _service.BeklemeyeAlAsync(id); // bool
            return ok ? Ok(new { message = "Başvuru Beklemede durumuna alındı." }) : BadRequest();
        }
    }
}
