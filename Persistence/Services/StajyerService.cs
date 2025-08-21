using Microsoft.EntityFrameworkCore;
using StajTakipUygulaması.Application.DTOs;
using StajTakipUygulaması.Application.Interfaces;
using StajTakipUygulaması.Data;
using StajTakipUygulaması.Domain.Entities;
using StajTakipUygulaması.Models;

namespace StajTakipUygulaması.Infrastructure.Services
{
    public class StajyerService : IStajyerService
    {
        private readonly StajContext _context;
        public StajyerService(StajContext context) => _context = context;

        public async Task<IReadOnlyList<StajyerDto>> GetAllAsync()
        {
            return await _context.Stajyerler
                .Select(x => new StajyerDto
                {
                    ID = x.ID,
                    Universite = x.Universite,
                    OgrenciNo = x.OgrenciNo,
                    Bolum = x.Bolum,
                    Fakulte = x.Fakulte,
                    BaslamaYili = x.BaslamaYili,
                    Sinif = x.Sinif,
                    PAU_ogrencisi_mi = x.PAU_ogrencisi_mi,

                    Ad = x.Ad,
                    Soyad = x.Soyad,
                    TCKimlikNo = x.TCKimlikNo,
                    DogumTarihi = x.DogumTarihi,
                    Cinsiyet = x.Cinsiyet,
                    TelNo = x.TelNo,
                    Email = x.Email,
                    Adres = x.Adres
                })
                .ToListAsync();
        }

        public async Task<StajyerDto?> GetByIdAsync(int id)
        {
            return await _context.Stajyerler
                .Where(x => x.ID == id)
                .Select(x => new StajyerDto
                {
                    ID = x.ID,
                    Universite = x.Universite,
                    OgrenciNo = x.OgrenciNo,
                    Bolum = x.Bolum,
                    Fakulte = x.Fakulte,
                    BaslamaYili = x.BaslamaYili,
                    Sinif = x.Sinif,
                    PAU_ogrencisi_mi = x.PAU_ogrencisi_mi,

                    Ad = x.Ad,
                    Soyad = x.Soyad,
                    TCKimlikNo = x.TCKimlikNo,
                    DogumTarihi = x.DogumTarihi,
                    Cinsiyet = x.Cinsiyet,
                    TelNo = x.TelNo,
                    Email = x.Email,
                    Adres = x.Adres
                })
                .FirstOrDefaultAsync();
        }

        public async Task<int> AddAsync(StajyerCreateDto dto)
        {
            var ent = new Stajyer
            {
                Universite = dto.Universite,
                OgrenciNo = dto.OgrenciNo,
                Bolum = dto.Bolum,
                Fakulte = dto.Fakulte,
                BaslamaYili = dto.BaslamaYili,
                Sinif = dto.Sinif,
                PAU_ogrencisi_mi = dto.PAU_ogrencisi_mi,

                Ad = dto.Ad,
                Soyad = dto.Soyad,
                TCKimlikNo = dto.TCKimlikNo,
                DogumTarihi = dto.DogumTarihi,
                Cinsiyet = dto.Cinsiyet,
                TelNo = dto.TelNo,
                Email = dto.Email,
                Adres = dto.Adres
            };

            _context.Stajyerler.Add(ent);
            await _context.SaveChangesAsync();
            return ent.ID;
        }

        public async Task UpdateAsync(StajyerUpdateDto dto)
        {
            var ent = await _context.Stajyerler.FirstOrDefaultAsync(x => x.ID == dto.ID);
            if (ent is null) throw new KeyNotFoundException("Stajyer bulunamadı.");

            // Sadece gönderilen alanları güncelle
            if (dto.Universite != null) ent.Universite = dto.Universite;
            if (dto.OgrenciNo != null) ent.OgrenciNo = dto.OgrenciNo;
            if (dto.Bolum != null) ent.Bolum = dto.Bolum;
            if (dto.Fakulte != null) ent.Fakulte = dto.Fakulte;
            if (dto.BaslamaYili.HasValue) ent.BaslamaYili = dto.BaslamaYili.Value;
            if (dto.Sinif != null) ent.Sinif = dto.Sinif;
            if (dto.PAU_ogrencisi_mi.HasValue) ent.PAU_ogrencisi_mi = dto.PAU_ogrencisi_mi.Value;

            if (dto.Ad != null) ent.Ad = dto.Ad;
            if (dto.Soyad != null) ent.Soyad = dto.Soyad;
            if (dto.TCKimlikNo != null) ent.TCKimlikNo = dto.TCKimlikNo;
            if (dto.DogumTarihi.HasValue) ent.DogumTarihi = dto.DogumTarihi.Value;
            if (dto.Cinsiyet != null) ent.Cinsiyet = dto.Cinsiyet;
            if (dto.TelNo != null) ent.TelNo = dto.TelNo;
            if (dto.Email != null) ent.Email = dto.Email;
            if (dto.Adres != null) ent.Adres = dto.Adres;

            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var ent = await _context.Stajyerler.FindAsync(id);
            if (ent is null) return;

            _context.Stajyerler.Remove(ent);
            await _context.SaveChangesAsync();
        }
    }
}
