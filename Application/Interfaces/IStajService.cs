using StajTakipUygulaması.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StajTakipUygulaması.Application.Interfaces
{
    public interface IStajService
    {
        Task<List<Staj>> GetAllAsync();
        Task<Staj> GetByIdAsync(int id);
        Task AddAsync(Staj staj);
        Task UpdateAsync(Staj staj);
        Task DeleteAsync(int id);
        Task<List<Staj>> GetAktifStajlarAsync();
        Task<List<Staj>> GetTamamlanmisStajlarAsync();

        Task<List<StajTuru>> GetStajTurleriAsync();

    }
}
