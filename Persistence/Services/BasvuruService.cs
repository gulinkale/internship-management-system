using Microsoft.EntityFrameworkCore;
using StajTakipUygulaması.Application.Interfaces;
using StajTakipUygulaması.Application.DTOs;
using StajTakipUygulaması.Data;    // StajContext
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using StajTakipUygulaması.Domain.Entities;
using StajTakipUygulamasi.Application.DTOs.BasvuruBelge;

namespace StajTakipUygulaması.Infrastructure.Services
{
    public class BasvuruService : IBasvuruService
    {
        private readonly StajContext _ctx;

        public BasvuruService(StajContext ctx)
        {
            _ctx = ctx;
        }

        // LISTE
        public async Task<List<BasvuruListDto>> TumBasvurulariGetirAsync()
        {
            return await _ctx.Basvurular
                .AsNoTracking()
                .Include(b => b.StajTuru)
                .Include(b => b.BasvuruBelgeleri)
                .OrderByDescending(b => b.BasvuruTarihi)
                .Select(b => new BasvuruListDto
                {
                    ID = b.ID,
                    AdSoyad = (b.Ad ?? "") + " " + (b.Soyad ?? ""),
                    TCKimlikNo = b.TCKimlikNo,
                    Durum = b.Durum.ToString(),
                    StajTuruAdi = b.StajTuru != null ? b.StajTuru.Ad : null,
                    BasvuruTarihi = b.BasvuruTarihi,
                    BelgeSayisi = b.BasvuruBelgeleri != null ? b.BasvuruBelgeleri.Count : 0
                })
                .ToListAsync();
        }

        // DETAY
        public async Task<BasvuruDto?> BasvuruDetayAsync(int id)
        {
            var b = await _ctx.Basvurular
                .Include(x => x.StajTuru)
                .Include(x => x.BasvuruBelgeleri)
                    .ThenInclude(bb => bb.BelgeTipi)
                .FirstOrDefaultAsync(x => x.ID == id);

            if (b == null) return null;

            return new BasvuruDto
            {
                ID = b.ID,
                // Kişisel
                Ad = b.Ad ?? string.Empty,
                Soyad = b.Soyad ?? string.Empty,
                TCKimlikNo = b.TCKimlikNo ?? string.Empty,
                DogumTarihi = b.DogumTarihi,
                Cinsiyet = b.Cinsiyet ?? string.Empty,
                TelNo = b.TelNo ?? string.Empty,
                Email = b.Email ?? string.Empty,
                Adres = b.Adres ?? string.Empty,
                // Eğitim
                Universite = b.Universite ?? string.Empty,
                Fakulte = b.Fakulte ?? string.Empty,
                Bolum = b.Bolum ?? string.Empty,
                Sinif = b.Sinif ?? string.Empty,
                BaslamaYili = b.BaslamaYili,
                OgrenciNo = b.OgrenciNo ?? string.Empty,
                // Staj
                Departman = b.Departman,
                SorumluID = b.SorumluID,
                Yetkiler = b.Yetkiler,
                BaslamaTarihi = b.BaslamaTarihi,
                BitisTarihi = b.BitisTarihi,
                // Tür
                StajTuruID = b.StajTuruID,
                StajTuruAdi = b.StajTuru?.Ad,
                // Başvuru meta
                BasvuruTarihi = b.BasvuruTarihi,
                Durum = b.Durum.ToString(),
                RedNedeni = b.RedNedeni,
                RedTarihi = b.RedTarihi,
                // Belgeler
                Belgeler = (b.BasvuruBelgeleri ?? new List<BasvuruBelge>())
                    .Select(bb => new BasvuruBelgeListDto
                    {
                        ID = bb.ID,
                        BelgeAdi = bb.BelgeAdı ?? "",
                        Aciklama = bb.Açıklama,
                        DosyaYolu = bb.Yolu,
                        BelgeTipiID = bb.BelgeTipiID,  // ← düzeltildi
                        BelgeTipiAdi = bb.BelgeTipi?.Ad
                    })
                    .ToList()
            };
        }

        // OLUSTUR
        public async Task<int> BasvuruOlusturAsync(BasvuruCreateDto dto)
        {
            var stajTuruVar = await _ctx.StajTurleri.AnyAsync(t => t.ID == dto.StajTuruID);
            if (!stajTuruVar)
                throw new InvalidOperationException("Geçersiz StajTuruID.");

            var entity = new Basvuru
            {
                // Kişisel
                Ad = dto.Ad,
                Soyad = dto.Soyad,
                TCKimlikNo = dto.TCKimlikNo,
                DogumTarihi = dto.DogumTarihi,
                Cinsiyet = dto.Cinsiyet,
                TelNo = dto.TelNo,
                Email = dto.Email,
                Adres = dto.Adres,
                // Eğitim
                Universite = dto.Universite,
                Fakulte = dto.Fakulte,
                Bolum = dto.Bolum,
                Sinif = dto.Sinif,
                BaslamaYili = dto.BaslamaYili,
                OgrenciNo = dto.OgrenciNo,
                // Staj
                Departman = string.IsNullOrWhiteSpace(dto.Departman) ? null : dto.Departman.Trim(),
                SorumluID = string.IsNullOrWhiteSpace(dto.SorumluID) ? null : dto.SorumluID.Trim(),
                Yetkiler = dto.Yetkiler,
                BaslamaTarihi = dto.BaslamaTarihi,
                BitisTarihi = dto.BitisTarihi,
                // Tür
                StajTuruID = dto.StajTuruID,
                // Meta
                BasvuruTarihi = DateTime.Now,
                Durum = BasvuruDurumu.Beklemede
            };

            _ctx.Basvurular.Add(entity);
            await _ctx.SaveChangesAsync();
            return entity.ID;
        }

        // GUNCELLE
        public async Task<bool> BasvuruGuncelleAsync(BasvuruUpdateDto dto)
        {
            var idProp = dto.GetType().GetProperty("ID") ?? dto.GetType().GetProperty("Id");
            if (idProp == null) throw new InvalidOperationException("BasvuruUpdateDto.ID alanı bulunamadı.");
            var id = (int)(idProp.GetValue(dto) ?? 0);
            if (id <= 0) return false;

            var b = await _ctx.Basvurular.FirstOrDefaultAsync(x => x.ID == id);
            if (b == null) return false;

            // DTO’da gelen null/boş olmayan değerleri uygula
            CopyIfPresent(dto, b, nameof(Basvuru.Ad));
            CopyIfPresent(dto, b, nameof(Basvuru.Soyad));
            CopyIfPresent(dto, b, nameof(Basvuru.TCKimlikNo));
            CopyIfPresent(dto, b, nameof(Basvuru.DogumTarihi));
            CopyIfPresent(dto, b, nameof(Basvuru.Cinsiyet));
            CopyIfPresent(dto, b, nameof(Basvuru.TelNo));
            CopyIfPresent(dto, b, nameof(Basvuru.Email));
            CopyIfPresent(dto, b, nameof(Basvuru.Adres));

            CopyIfPresent(dto, b, nameof(Basvuru.Universite));
            CopyIfPresent(dto, b, nameof(Basvuru.Fakulte));
            CopyIfPresent(dto, b, nameof(Basvuru.Bolum));
            CopyIfPresent(dto, b, nameof(Basvuru.Sinif));
            CopyIfPresent(dto, b, nameof(Basvuru.BaslamaYili));
            CopyIfPresent(dto, b, nameof(Basvuru.OgrenciNo));

            CopyIfPresent(dto, b, nameof(Basvuru.Departman));
            CopyIfPresent(dto, b, nameof(Basvuru.SorumluID));
            CopyIfPresent(dto, b, nameof(Basvuru.Yetkiler));
            CopyIfPresent(dto, b, nameof(Basvuru.BaslamaTarihi));
            CopyIfPresent(dto, b, nameof(Basvuru.BitisTarihi));
            CopyIfPresent(dto, b, nameof(Basvuru.StajTuruID));

            await _ctx.SaveChangesAsync();
            return true;
        }

        // ONAYLA
        public async Task<bool> BasvuruOnaylaAsync(int id)
        {
            var b = await _ctx.Basvurular.FirstOrDefaultAsync(x => x.ID == id);
            if (b == null) return false;

            if (b.Durum == BasvuruDurumu.Onaylandi) return true;

            if (b.BaslamaTarihi == null) b.BaslamaTarihi = DateTime.Today;
            if (b.BitisTarihi == null) b.BitisTarihi = DateTime.Today.AddMonths(1);

            var valid = b.StajTuruID > 0 && await _ctx.StajTurleri.AnyAsync(t => t.ID == b.StajTuruID);
            if (!valid) throw new InvalidOperationException("Geçerli bir staj türü seçilmemiş.");

            b.Durum = BasvuruDurumu.Onaylandi;
            await _ctx.SaveChangesAsync();
            return true;
        }

        // REDDET
        public async Task<bool> BasvuruReddetAsync(BasvuruReddetDto dto)
        {
            var b = await _ctx.Basvurular.FirstOrDefaultAsync(x => x.ID == dto.ID);
            if (b == null) return false;

            b.Durum = BasvuruDurumu.Reddedildi;
            b.RedNedeni = string.IsNullOrWhiteSpace(dto.RedNedeni) ? null : dto.RedNedeni.Trim();
            b.RedTarihi = DateTime.Now;

            await _ctx.SaveChangesAsync();
            return true;
        }

        // BEKLEMEYE AL
        public async Task<bool> BeklemeyeAlAsync(int id)
        {
            var b = await _ctx.Basvurular.FirstOrDefaultAsync(x => x.ID == id);
            if (b == null) return false;

            b.Durum = BasvuruDurumu.Beklemede;
            b.RedNedeni = null;
            b.RedTarihi = null;

            await _ctx.SaveChangesAsync();
            return true;
        }

        // Helper: DTO’da gelen null/boş olmayan değeri entity’ye kopyala
        private static void CopyIfPresent(object sourceDto, object targetEntity, string propertyName)
        {
            var dtoProp = sourceDto.GetType().GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
            var entProp = targetEntity.GetType().GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
            if (dtoProp == null || entProp == null || !entProp.CanWrite) return;

            var value = dtoProp.GetValue(sourceDto);
            if (value == null) return;

            if (value is string s && string.IsNullOrWhiteSpace(s)) return;
            entProp.SetValue(targetEntity, value);
        }
    }
}
