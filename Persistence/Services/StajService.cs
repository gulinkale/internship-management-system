using Microsoft.EntityFrameworkCore;
using StajTakipUygulaması.Application.Interfaces;
using StajTakipUygulaması.Data;
using StajTakipUygulaması.Domain.Entities;

namespace StajTakipUygulaması.Infrastructure.Services
{
    public class StajService : IStajService
    {
        private readonly StajContext _context;

        public StajService(StajContext context) => _context = context;

        public async Task<List<Staj>> GetAllAsync()
        {
            return await _context.Stajlar
                .Include(s => s.Stajyer)
                .Include(s => s.StajTuru)
                .Include(s => s.Belgeler).ThenInclude(b => b.BelgeTipi)
                .ToListAsync();
        }

        public async Task<List<StajTuru>> GetStajTurleriAsync()
        {
            return await _context.StajTurleri.ToListAsync();
        }


        public async Task<Staj?> GetByIdAsync(int id)
        {
            return await _context.Stajlar
                .Include(s => s.Stajyer)
                .Include(s => s.StajTuru)
                .Include(s => s.Belgeler).ThenInclude(b => b.BelgeTipi)
                .FirstOrDefaultAsync(s => s.ID == id);
        }

        public async Task AddAsync(Staj staj)
        {
            _context.Stajlar.Add(staj);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Staj staj)
        {
            _context.Stajlar.Update(staj);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var staj = await _context.Stajlar.FindAsync(id);
            if (staj is null) return;

            _context.Stajlar.Remove(staj);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Staj>> GetAktifStajlarAsync()
        {
            var now = DateTime.Now.Date;
            return await _context.Stajlar
                .Where(s => s.BitisTarihi >= now)
                .Include(s => s.Stajyer)
                .Include(s => s.StajTuru)
                .ToListAsync();
        }

        public async Task<List<Staj>> GetTamamlanmisStajlarAsync()
        {
            var now = DateTime.Now.Date;
            return await _context.Stajlar
                .Where(s => s.BitisTarihi < now)
                .Include(s => s.Stajyer)
                .Include(s => s.StajTuru)
                .ToListAsync();
        }
    }
}
