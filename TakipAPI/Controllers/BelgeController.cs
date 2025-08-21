// Proje: StajTakipUygulaması.Api
// Dosya: Controllers/BelgeController.cs
using Microsoft.AspNetCore.Mvc;
using StajTakipUygulaması.Application.DTOs;
using StajTakipUygulaması.Application.Interfaces; // IBelgeService, IFileStorage, IBelgeTipiService vs.

namespace StajTakipUygulaması.Api.Controllers
{
    public record BelgeResponseDto(
        int ID,
        int StajID,
        int BelgeTipiID,
        string? BelgeTipiAd,
        string Yolu,
        string BelgeAdı,
        string? Açıklama
    );

    [ApiController]
    [Route("api/[controller]")]
    public class BelgeController : ControllerBase
    {
        private readonly IBelgeService _belgeService;
        private readonly IBelgeTipiService _belgeTipiService;
        private readonly IFileStorage _storage;

        public BelgeController(
            IBelgeService belgeService,
            IBelgeTipiService belgeTipiService,
            IFileStorage storage)
        {
            _belgeService = belgeService;
            _belgeTipiService = belgeTipiService;
            _storage = storage;
        }

        /// <summary> Tüm belgeleri (opsiyonel stajId filtresi ile) döner. </summary>
        /// <param name="stajId">Opsiyonel filtre</param>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BelgeResponseDto>>> GetAll([FromQuery] int? stajId)
        {
            var list = await _belgeService.GetAllAsync(); // IEnumerable<BelgeDto>
            var filtered = stajId.HasValue
                ? list.Where(b => b.StajID == stajId.Value)
                : list;

            // Tip adını zaten BelgeDto içinde veriyorsun (BelgeTipiAd). Yine de uniform DTO dönelim.
            var data = filtered.Select(b => new BelgeResponseDto(
                b.ID, b.StajID, b.BelgeTipiID, b.BelgeTipiAd, b.Yolu, b.BelgeAdı, b.Açıklama
            ));

            return Ok(data);
        }

        /// <summary> Tek belge detay. </summary>
        [HttpGet("{id:int}")]
        public async Task<ActionResult<BelgeResponseDto>> Get(int id)
        {
            var b = await _belgeService.GetByIdAsync(id); // BelgeDto?
            if (b is null) return NotFound();

            var dto = new BelgeResponseDto(
                b.ID, b.StajID, b.BelgeTipiID, b.BelgeTipiAd, b.Yolu, b.BelgeAdı, b.Açıklama
            );
            return Ok(dto);
        }

        /// <summary>
        /// Dosya yükler ve yeni belge kaydı oluşturur.
        /// multipart/form-data: dosya, stajId, belgeTipId, aciklama
        /// </summary>
        [HttpPost("upload")]
        [RequestSizeLimit(100_000_000)] // 100 MB örnek
        public async Task<ActionResult<object>> Upload(
            [FromForm] IFormFile dosya,
            [FromForm] int stajId,
            [FromForm] int belgeTipId,
            [FromForm] string? aciklama)
        {
            if (dosya == null || dosya.Length == 0)
                return BadRequest("Dosya seçilmedi.");

            if (stajId <= 0 || belgeTipId <= 0)
                return BadRequest("stajId ve belgeTipId zorunludur.");

            // Servisin beklediği: BelgeUploadRequest (stream + orijinal dosya adı + tip & staj)
            using var stream = dosya.OpenReadStream();
            var req = new BelgeUploadRequest
            {
                Content = stream,
                OriginalFileName = dosya.FileName,
                BelgeTipiID = belgeTipId,
                StajID = stajId
            };

            // Dosyayı kaydeder + meta kaydı yapar, geriye yeni belge ID'si döner
            var newId = await _belgeService.UploadAndSaveAsync(req);

            // İsteğe bağlı: açıklama güncelle (serviste UploadAndSave içinde açıklama set etmiyorsan)
            if (!string.IsNullOrWhiteSpace(aciklama))
            {
                await _belgeService.UpdateAsync(new BelgeUpdateDto
                {
                    ID = newId,
                    Açıklama = aciklama.Trim()
                });
            }

            // Cevap
            var created = await _belgeService.GetByIdAsync(newId);
            return CreatedAtAction(nameof(Get), new { id = newId }, new
            {
                id = newId,
                created?.StajID,
                created?.BelgeTipiID,
                created?.Yolu
            });
        }

        /// <summary>
        /// Var olan belgenin dosyasını yenisiyle değiştirir (yolu günceller).
        /// multipart/form-data: yeniDosya
        /// </summary>
        [HttpPut("{id:int}/file")]
        [RequestSizeLimit(100_000_000)]
        public async Task<ActionResult<object>> UpdateFile(int id, [FromForm] IFormFile yeniDosya)
        {
            var mevcut = await _belgeService.GetByIdAsync(id);
            if (mevcut is null) return NotFound();

            if (yeniDosya == null || yeniDosya.Length == 0)
                return BadRequest("Dosya boş.");

            // 1) Yeni dosyayı kaydet → relativePath al
            using var stream = yeniDosya.OpenReadStream();
            // BelgeService UploadAndSaveAsync yeni kayıt açıyor; biz sadece dosya yolunu değiştirmek istiyoruz.
            // Bu nedenle IFileStorage'ı doğrudan burada kullanıyoruz.
            var relativePath = await _storage.SaveAsync(
                stream,
                yeniDosya.FileName,
                subFolder: "Belgeler"           // appsettings: Uploads:BelgeFolder ile eşleşmeli (service’de "Belgeler" default)
            );

            // 2) Yolu güncelle
            await _belgeService.UpdateAsync(new BelgeUpdateDto
            {
                ID = id,
                Yolu = relativePath,
                Açıklama = "Güncellendi"
            });

            // 3) Cevap
            var guncel = await _belgeService.GetByIdAsync(id);
            return Ok(new { guncel!.ID, guncel.Yolu });
        }

        /// <summary>
        /// Meta bilgilerinde (Açıklama/Yolu) değişiklik yapar.
        /// </summary>
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateMeta(int id, [FromBody] BelgeUpdateDto dto)
        {
            if (dto is null || dto.ID != id)
                return BadRequest("Geçersiz istek.");

            await _belgeService.UpdateAsync(dto);
            return NoContent();
        }
    }
}
