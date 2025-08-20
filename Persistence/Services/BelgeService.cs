using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using StajTakipUygulamasi.Application.DTOs;
using StajTakipUygulamasi.Application.Interfaces;
using StajTakipUygulaması.Data;            // StajContext
using StajTakipUygulaması.Models;          // Belge, BelgeTipi entity'leri

namespace StajTakipUygulamasi.Infrastructure.Services
{
    public class BelgeService : IBelgeService
    {
        private readonly StajContext _ctx;
        private readonly IFileStorage _storage;
        private readonly string _subFolder; // appsettings: Uploads:BelgeFolder (varsayılan: "Belgeler")

        public BelgeService(StajContext ctx, IFileStorage storage, IConfiguration config)
        {
            _ctx = ctx;
            _storage = storage;
            _subFolder = config.GetSection("Uploads")["BelgeFolder"] ?? "Belgeler";
        }

        // Tüm belgeleri DTO olarak getir
        public async Task<IEnumerable<BelgeDto>> GetAllAsync()
        {
            return await _ctx.Belgeler
                .Include(b => b.BelgeTipi)
                .Select(b => new BelgeDto
                {
                    ID = b.ID,
                    BelgeAdı = b.BelgeAdı,
                    Açıklama = b.Açıklama,
                    Yolu = b.Yolu,
                    BelgeTipiID = b.BelgeTipiID,
                    BelgeTipiAd = b.BelgeTipi != null ? b.BelgeTipi.Ad : null,
                    StajID = b.StajID
                })
                .ToListAsync();
        }

        // Id'ye göre belge getir
        public async Task<BelgeDto?> GetByIdAsync(int id)
        {
            return await _ctx.Belgeler
                .Include(b => b.BelgeTipi)
                .Where(b => b.ID == id)
                .Select(b => new BelgeDto
                {
                    ID = b.ID,
                    BelgeAdı = b.BelgeAdı,
                    Açıklama = b.Açıklama,
                    Yolu = b.Yolu,
                    BelgeTipiID = b.BelgeTipiID,
                    BelgeTipiAd = b.BelgeTipi != null ? b.BelgeTipi.Ad : null,
                    StajID = b.StajID
                })
                .FirstOrDefaultAsync();
        }

        // Sadece meta ekleme (dosya yok)
        public async Task<int> AddAsync(BelgeCreateDto dto)
        {
            var ent = new Belge
            {
                BelgeAdı = dto.BelgeAdı,
                Açıklama = dto.Açıklama ?? "",
                Yolu = dto.Yolu,
                BelgeTipiID = dto.BelgeTipiID,
                StajID = dto.StajID
            };

            _ctx.Belgeler.Add(ent);
            await _ctx.SaveChangesAsync();
            return ent.ID;
        }

        // Dosya + meta kaydetme
        public async Task<int> UploadAndSaveAsync(BelgeUploadRequest req)
        {
            if (req.Content == Stream.Null || string.IsNullOrWhiteSpace(req.OriginalFileName))
                throw new ArgumentException("Geçersiz dosya.");

            // 1) Dosyayı kaydet, web path al
            var relativePath = await _storage.SaveAsync(req.Content, req.OriginalFileName, _subFolder);

            // 2) Meta (DB) kaydı
            var createDto = new BelgeCreateDto
            {
                BelgeAdı = req.OriginalFileName,
                Yolu = relativePath,
                BelgeTipiID = req.BelgeTipiID,
                StajID = req.StajID
            };

            return await AddAsync(createDto);
        }

        // Güncelleme (yalnızca açıklama ve/veya yol)
        public async Task UpdateAsync(BelgeUpdateDto dto)
        {
            var ent = await _ctx.Belgeler.FirstOrDefaultAsync(x => x.ID == dto.ID);
            if (ent == null)
                throw new KeyNotFoundException("Belge bulunamadı.");

            if (dto.Açıklama is not null)
                ent.Açıklama = dto.Açıklama;

            if (dto.Yolu is not null)
                ent.Yolu = dto.Yolu;

            await _ctx.SaveChangesAsync();
        }
    }
}
