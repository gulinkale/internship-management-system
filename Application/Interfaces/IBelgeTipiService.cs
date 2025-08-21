using StajTakipUygulaması.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace StajTakipUygulaması.Application.Interfaces
{
    public interface IBelgeTipiService
    {
        Task<List<BelgeTipi>> GetAllAsync();
    }
}
