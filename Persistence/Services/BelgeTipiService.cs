using Microsoft.EntityFrameworkCore;
using StajTakipUygulaması.Domain.Entities;
using StajTakipUygulaması.Application.Interfaces;
using StajTakipUygulaması.Data;

namespace StajTakipUygulaması.Infrastructure.Services
{
    public class BelgeTipiService : IBelgeTipiService
    {
        private readonly StajContext _context;

        public BelgeTipiService(StajContext context)
        {
            _context = context;
        }

        // Tüm belge tiplerini veritabanından getirir
        public async Task<List<BelgeTipi>> GetAllAsync()
        {
            return await _context.BelgeTipleri.ToListAsync();
        }
    }
}
