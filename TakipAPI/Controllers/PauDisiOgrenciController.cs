using StajTakipUygulaması.Domain.Entities;
// Proje: TakipAPI (StajTakipUygulaması.Api)
// Dosya: Controllers/PauDisiOgrenciController.cs

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO; // <-- MemoryStream için

// Alias'lar: doğru katmanları netleştiriyoruz
using AppDtos = StajTakipUygulaması.Application.DTOs;        // StajyerCreateDto, BelgeUploadRequest
using AppSvcs = StajTakipUygulaması.Application.Interfaces;  // IStajyerService, IStajService, IBelgeService

// DİKKAT: Entity'lerin gerçek namespace'ine göre EN AZ BİRİNİ aktif bırak
// using Domain = StajTakipUygulaması.Models;   // Türkçe "ı" ile olan proje ise bu satır
//using Domain = StajTakipUygulaması.Models; // ASCII ile olan proje ise bu satır


namespace StajTakipUygulaması.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Consumes("multipart/form-data")]
    public class PauDisiOgrenciController : ControllerBase
    {
        private readonly AppSvcs.IStajyerService _stajyerService;
        private readonly AppSvcs.IStajService _stajService;
        private readonly AppSvcs.IBelgeService _belgeService;

        public PauDisiOgrenciController(
            AppSvcs.IStajyerService stajyerService,
            AppSvcs.IStajService stajService,
            AppSvcs.IBelgeService belgeService)
        {
            _stajyerService = stajyerService;
            _stajService = stajService;
            _belgeService = belgeService;
        }

        [HttpPost("create")]
        [RequestSizeLimit(100_000_000)]
        public async Task<IActionResult> Create(
            // --- Stajyer ---
            [FromForm] string Universite,
            [FromForm] string OgrenciNo,
            [FromForm] string Bolum,
            [FromForm] string Fakulte,
            [FromForm] DateTime BaslamaYili,  // DTO ile uyumlu: DateTime
            [FromForm] string Sinif,          // DTO ile uyumlu: string
            [FromForm] string Ad,
            [FromForm] string Soyad,
            [FromForm] string TCKimlikNo,
            [FromForm] DateTime DogumTarihi,
            [FromForm] string Cinsiyet,
            [FromForm] string TelNo,
            [FromForm] string Email,
            [FromForm] string Adres,

            // --- Staj ---
            [FromForm] int StajTuruID,
            [FromForm] DateTime BaslamaTarihi,
            [FromForm] DateTime BitisTarihi,
            [FromForm] string? Departman,
            [FromForm] string? SorumluID,
            [FromForm] string? Yetkiler,

            // --- Belgeler (opsiyonel) ---
            [FromForm] IFormFile? OgrenciBelgesi,
            [FromForm] IFormFile? Transkript,
            [FromForm] IFormFile? BasvuruFormu,
            [FromForm] IFormFile? Taahutname,
            [FromForm] IFormFile? Referans
        )
        {
            // 1) Stajyer → StajyerCreateDto ile ekle (service ID döndürür)
            var stajyerCreate = new AppDtos.StajyerCreateDto
            {
                Universite = Universite,
                OgrenciNo = OgrenciNo,
                Bolum = Bolum,
                Fakulte = Fakulte,
                BaslamaYili = BaslamaYili,
                Sinif = Sinif,
                PAU_ogrencisi_mi = false,
                Ad = Ad,
                Soyad = Soyad,
                TCKimlikNo = TCKimlikNo,
                DogumTarihi = DogumTarihi,
                Cinsiyet = Cinsiyet,
                TelNo = TelNo,
                Email = Email,
                Adres = Adres
            };
            var stajyerId = await _stajyerService.AddAsync(stajyerCreate);

            // 2) Staj → IStajService.AddAsync(Staj) entity bekliyor
            var staj = new Staj
            {
                StajyerID = stajyerId,
                StajTuruID = StajTuruID,
                BaslamaTarihi = BaslamaTarihi,
                BitisTarihi = BitisTarihi,
                Departman = Departman,
                SorumluID = SorumluID,
                Yetkiler = Yetkiler
            };
            await _stajService.AddAsync(staj);
            var stajId = staj.ID; // SaveChanges sonrası dolu

            // 3) Belgeler → IBelgeService.UploadAndSaveAsync(Stream...)
            var belgeIds = new List<int>();

            async Task TryUploadAsync(IFormFile? file, int belgeTipiId)
            {
                if (file is null) return;
                using var ms = new MemoryStream();
                await file.CopyToAsync(ms);
                ms.Position = 0; // Stream başına al

                var req = new AppDtos.BelgeUploadRequest
                {
                    Content = ms,
                    OriginalFileName = file.FileName,
                    BelgeTipiID = belgeTipiId,
                    StajID = stajId
                };
                var id = await _belgeService.UploadAndSaveAsync(req);
                belgeIds.Add(id);
            }

            await TryUploadAsync(OgrenciBelgesi, 1);
            await TryUploadAsync(Transkript, 2);
            await TryUploadAsync(BasvuruFormu, 3);
            await TryUploadAsync(Taahutname, 4);
            await TryUploadAsync(Referans, 5);

            return Created(string.Empty, new { stajyerId, stajId, belgeler = belgeIds });
        }
    }
}
