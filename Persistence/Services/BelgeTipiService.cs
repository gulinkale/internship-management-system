using Microsoft.EntityFrameworkCore;
using StajTakipUygulaması.Domain.Entities;
using StajTakipUygulaması.Application.DTOs;
using StajTakipUygulaması.Application.Interfaces;
using StajTakipUygulaması.Data;

namespace StajTakipUygulaması.Infrastructure.Services
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

