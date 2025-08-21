using StajTakipUygulaması.Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using StajTakipUygulaması.Application.DTOs;
using StajTakipUygulaması.Data;

namespace StajTakipUygulamasi.Infrastructure.Services
{
    public class RaporService : IRaporService
    {
        private readonly StajContext _ctx;
        public RaporService(StajContext ctx) => _ctx = ctx;

        public async Task<List<RaporStajyerDto>> AraAsync(string arama, int belgeTipId, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(arama)) return new();

            arama = arama.Trim();

            return await _ctx.Stajyerler
                .AsNoTracking()
                .Include(s => s.Stajlar)
                    .ThenInclude(st => st.Belgeler)
                        .ThenInclude(b => b.BelgeTipi)
                .Where(s =>
                       s.TCKimlikNo == arama
                    || ((s.Ad ?? "") + " " + (s.Soyad ?? "")).Contains(arama)
                    || s.OgrenciNo == arama)
                .Select(s => new RaporStajyerDto
                {
                    Id = s.ID,
                    AdSoyad = (s.Ad ?? "") + " " + (s.Soyad ?? ""),
                    TCKimlikNo = s.TCKimlikNo,
                    OgrenciNo = s.OgrenciNo,
                    Stajlar = s.Stajlar.Select(st => new RaporStajDto
                    {
                        Id = st.ID,
                        BaslamaTarihi = st.BaslamaTarihi,
                        BitisTarihi = st.BitisTarihi,
                        Belgeler = st.Belgeler
                            .Where(b => belgeTipId == 0 || b.BelgeTipiID == belgeTipId)
                            .Select(b => new RaporBelgeDto
                            {
                                Id = b.ID,
                                BelgeAdi = b.BelgeAdı ?? "",
                                Yol = b.Yolu,
                                BelgeTipiId = b.BelgeTipiID,
                                BelgeTipiAdi = b.BelgeTipi!.Ad
                            }).ToList()
                    }).ToList()
                })
                .OrderByDescending(x => x.Id)
                .ToListAsync(ct);
        }
    }
}
