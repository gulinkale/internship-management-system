
using Microsoft.AspNetCore.Mvc;
using System.IO;
using StajTakipUygulaması.Models;
using StajTakipUygulaması.Infrastructure.Services;

[ApiController]
[Route("api/[controller]")]
public class BelgeController : ControllerBase
{
    private readonly IBelgeService _belgeService;
    private readonly IBelgeTipiService _belgeTipiService;
    private readonly IWebHostEnvironment _env;

    public BelgeController(
        IBelgeService belgeService,
        IBelgeTipiService belgeTipiService,
        IWebHostEnvironment env)
    {
        _belgeService = belgeService;
        _belgeTipiService = belgeTipiService;
        _env = env;
    }

    // GET api/belge?stajId=5  → ilgili stajın belgeleri
    [HttpGet]
    public async Task<ActionResult<IEnumerable<object>>> GetByStaj([FromQuery] int stajId)
    {
        var list = await _belgeService.GetByStajIdAsync(stajId); // ← imzayı buna göre varsaydım
        var tipler = await _belgeTipiService.GetAllAsync();
        var data = list.Select(b => new {
            b.ID,
            b.StajID,
            b.BelgeTipiID,
            BelgeTipiAd = tipler.FirstOrDefault(t => t.ID == b.BelgeTipiID)?.Ad ?? "",
            b.Yolu,
            b.BelgeAdı,
            b.Açıklama
        });
        return Ok(data);
    }

    // GET api/belge/12  → tek belge
    [HttpGet("{id:int}")]
    public async Task<ActionResult<object>> Get(int id)
    {
        var b = await _belgeService.GetByIdAsync(id);
        if (b is null) return NotFound();

        var tip = await _belgeTipiService.GetByIdAsync(b.BelgeTipiID);
        return Ok(new
        {
            b.ID,
            b.StajID,
            b.BelgeTipiID,
            BelgeTipiAd = tip?.Ad ?? "",
            b.Yolu,
            b.BelgeAdı,
            b.Açıklama
        });
    }

    // POST api/belge/upload  (multipart/form-data: dosya, stajId, belgeTipId, aciklama)
    [HttpPost("upload")]
    [RequestSizeLimit(100_000_000)]
    public async Task<ActionResult<object>> Upload(
        [FromForm] IFormFile dosya,
        [FromForm] int stajId,
        [FromForm] int belgeTipId,
        [FromForm] string? aciklama)
    {
        if (dosya == null || dosya.Length == 0) return BadRequest("Dosya seçilmedi.");
        var tip = await _belgeTipiService.GetByIdAsync(belgeTipId);
        if (tip is null) return BadRequest("Geçersiz belge tipi.");

        var root = _env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
        var uploadDir = Path.Combine(root, "belgeler");
        Directory.CreateDirectory(uploadDir);

        var unique = $"{Guid.NewGuid()}_{dosya.FileName}";
        var fullPath = Path.Combine(uploadDir, unique);
        await using (var fs = new FileStream(fullPath, FileMode.Create))
            await dosya.CopyToAsync(fs);

        var webPath = $"/belgeler/{unique}"; // Yolu alanına bu yazılır :contentReference[oaicite:3]{index=3}

        var entity = new Belge
        {
            StajID = stajId,                // :contentReference[oaicite:4]{index=4}
            BelgeTipiID = belgeTipId,       // :contentReference[oaicite:5]{index=5}
            Yolu = webPath,
            BelgeAdı = tip.Ad,              // :contentReference[oaicite:6]{index=6}
            Açıklama = string.IsNullOrWhiteSpace(aciklama) ? "Yüklendi" : aciklama!.Trim()
        };

        var created = await _belgeService.CreateAsync(entity);
        return CreatedAtAction(nameof(Get), new { id = created.ID }, new { created.ID, created.Yolu });
    }

    // PUT api/belge/{id}/file  (multipart/form-data: yeniDosya)
    [HttpPut("{id:int}/file")]
    public async Task<ActionResult<object>> UpdateFile(int id, [FromForm] IFormFile yeniDosya)
    {
        var belge = await _belgeService.GetByIdAsync(id);
        if (belge is null) return NotFound();
        if (yeniDosya == null || yeniDosya.Length == 0) return BadRequest("Dosya boş.");

        var root = _env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
        var uploadDir = Path.Combine(root, "belgeler");
        Directory.CreateDirectory(uploadDir);

        var unique = $"{Guid.NewGuid()}_{yeniDosya.FileName}";
        var fullPath = Path.Combine(uploadDir, unique);
        await using (var fs = new FileStream(fullPath, FileMode.Create))
            await yeniDosya.CopyToAsync(fs);

        belge.Yolu = $"/belgeler/{unique}";
        belge.Açıklama = "Güncellendi";
        await _belgeService.UpdateAsync(belge);

        return Ok(new { belge.ID, belge.Yolu });
    }
}
