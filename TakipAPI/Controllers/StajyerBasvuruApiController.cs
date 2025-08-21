using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StajTakipUygulaması.Data;
using StajTakipUygulaması.Models;
using StajTakipUygulaması.Application.Interfaces;

namespace StajTakipUygulaması.Api.Controllers.v1
{
    [ApiController]
    [Route("api/v1/[controller]")] // /api/v1/stajyerbasvurular
    public class StajyerBasvurularController : ControllerBase
    {
        private readonly StajContext _context;
        private readonly IEmailSender _email;

        public StajyerBasvurularController(StajContext context, IEmailSender email)
        {
            _context = context;
            _email = email;
        }

        // GET: /api/v1/stajyerbasvurular
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var list = await _context.Basvurular
                .AsNoTracking()
                .Include(b => b.StajTuru)
                .Include(b => b.BasvuruBelgeleri)
                .OrderByDescending(b => b.ID)
                .ToListAsync();

            return Ok(list);
        }

        // GET: /api/v1/stajyerbasvurular/{id}
        [HttpGet("{id:int}")]
        public async Task<IActionResult> Get(int id)
        {
            var b = await _context.Basvurular
                .AsNoTracking()
                .Include(x => x.StajTuru)
                .Include(x => x.BasvuruBelgeleri)
                .FirstOrDefaultAsync(x => x.ID == id);

            return b is null ? NotFound() : Ok(b);
        }

        // POST: /api/v1/stajyerbasvurular (multipart/form-data)
        [HttpPost]
        [RequestSizeLimit(100_000_000)]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Create([FromForm] BasvuruCreateRequest req, CancellationToken ct)
        {
            var tc = req?.TCKimlikNo?.Trim();
            if (string.IsNullOrWhiteSpace(tc))
                return BadRequest(new { message = "TCKimlikNo zorunludur." });

            var varMiBasvuru = await _context.Basvurular.AnyAsync(x => x.TCKimlikNo == tc, ct);
            var varMiStajyer = await _context.Stajyerler.AnyAsync(x => x.TCKimlikNo == tc, ct);
            if (varMiBasvuru || varMiStajyer)
                return Conflict(new { message = "Bu TC ile mevcut başvuru/staj kaydı var." });

            var basvuru = new Basvuru
            {
                Ad = req.Ad,
                Soyad = req.Soyad,
                TCKimlikNo = req.TCKimlikNo,
                Cinsiyet = req.Cinsiyet,
                TelNo = req.TelNo,
                Email = req.Email,
                Adres = req.Adres,
                Universite = req.Universite,
                Fakulte = req.Fakulte,
                Bolum = req.Bolum,
                Sinif = req.Sinif,
                BaslamaYili = (DateTime)req.BaslamaYili,
                OgrenciNo = req.OgrenciNo,
                Departman = req.Departman,
                SorumluID = req.SorumluID,
                Yetkiler = req.Yetkiler,
                BaslamaTarihi = req.BaslamaTarihi,
                BitisTarihi = req.BitisTarihi,
                StajTuruID = req.StajTuruID,
                BasvuruTarihi = DateTime.Now
            };

            var uploads = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "belgeler");
            Directory.CreateDirectory(uploads);

            void AddFile(IFormFile? file, string ad, int tip)
            {
                if (file == null || file.Length == 0) return;
                var unique = $"{Guid.NewGuid()}_{file.FileName}";
                var full = Path.Combine(uploads, unique);
                using var fs = new FileStream(full, FileMode.Create);
                file.CopyTo(fs);

                basvuru.BasvuruBelgeleri ??= new List<BasvuruBelge>();
                basvuru.BasvuruBelgeleri.Add(new BasvuruBelge
                {
                    BelgeAdı = ad,
                    BelgeTipiID = tip,
                    Yolu = "/belgeler/" + unique,
                    Açıklama = ""
                });
            }

            AddFile(req.OgrenciBelgesi, "Öğrenci Belgesi", 1);
            AddFile(req.Transkript, "Transkript", 2);
            AddFile(req.BasvuruFormu, "Başvuru Formu", 3);
            AddFile(req.Taahutname, "Taahhütname", 4);
            AddFile(req.Referans, "Referans Mektubu", 5);

            if (req.Fotograf != null && req.Fotograf.Length > 0)
            {
                var fotoTipId = await _context.BelgeTipleri
                    .Where(t => t.Ad == "Fotoğraf")
                    .Select(t => t.ID)
                    .FirstOrDefaultAsync(ct);

                if (fotoTipId > 0)
                {
                    var fileName = $"{Guid.NewGuid()}{Path.GetExtension(req.Fotograf.FileName)}";
                    var fullPath = Path.Combine(uploads, fileName);
                    using (var s = new FileStream(fullPath, FileMode.Create))
                        await req.Fotograf.CopyToAsync(s, ct);

                    basvuru.BasvuruBelgeleri ??= new List<BasvuruBelge>();
                    basvuru.BasvuruBelgeleri.Add(new BasvuruBelge
                    {
                        BelgeTipiID = fotoTipId,
                        BelgeAdı = "Fotoğraf",
                        Yolu = "/belgeler/" + fileName,
                        Açıklama = ""
                    });
                }
            }

            _context.Basvurular.Add(basvuru);
            await _context.SaveChangesAsync(ct);

            if (!string.IsNullOrWhiteSpace(basvuru.Email))
            {
                var subject = "Staj Başvurunuz Alındı";
                var body = $@"Merhaba <b>{basvuru.Ad} {basvuru.Soyad}</b>, başvurunuz alınmıştır. No: {basvuru.ID}";
                try { await _email.SendAsync(basvuru.Email, subject, body, ct); } catch { }
            }

            return CreatedAtAction(nameof(Get), new { id = basvuru.ID }, new { id = basvuru.ID });
        }
    }

    // multipart/form-data binding modeli
    public class BasvuruCreateRequest
    {
        // Kişisel
        public string Ad { get; set; } = "";
        public string Soyad { get; set; } = "";
        public string TCKimlikNo { get; set; } = "";
        public string? Cinsiyet { get; set; }
        public string? TelNo { get; set; }
        public string? Email { get; set; }
        public string? Adres { get; set; }

        // Eğitim
        public string? Universite { get; set; }
        public string? Fakulte { get; set; }
        public string? Bolum { get; set; }
        public string? Sinif { get; set; }
        public DateTime? BaslamaYili { get; set; }
        public string? OgrenciNo { get; set; }

        // Staj
        public string? Departman { get; set; }
        public string? SorumluID { get; set; }
        public string? Yetkiler { get; set; }
        public DateTime? BaslamaTarihi { get; set; }
        public DateTime? BitisTarihi { get; set; }
        public int StajTuruID { get; set; }

        // Dosyalar
        public IFormFile? OgrenciBelgesi { get; set; }
        public IFormFile? Transkript { get; set; }
        public IFormFile? BasvuruFormu { get; set; }
        public IFormFile? Taahutname { get; set; }
        public IFormFile? Referans { get; set; }
        public IFormFile? Fotograf { get; set; }
    }
}
