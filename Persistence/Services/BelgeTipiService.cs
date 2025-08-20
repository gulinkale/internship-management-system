using Microsoft.EntityFrameworkCore;
using StajTakipUygulaması.Data;
using StajTakipUygulamasi.Application.DTOs;
using StajTakipUygulaması.Models;

namespace StajTakipUygulamasi.Persistence.Services
{
    public class BelgeTipiService : IBelgeTipiService
    {
        private readonly StajContext _context;

        public BelgeTipiService(StajContext context) => _context = context;

        public async Task<IReadOnlyList<BelgeTipiDto>> GetAllAsync()
        {
            return await _context.BelgeTipleri
                .Select(x => new BelgeTipiDto { Id = x.ID, Ad = x.Ad })
                .ToListAsync();
        }

        Task<List<BelgeTipi>> IBelgeTipiService.GetAllAsync()
        {
            throw new NotImplementedException();
        }
    }
}

