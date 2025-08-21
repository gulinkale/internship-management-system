using StajTakipUygulaması.Application.DTOs;

namespace StajTakipUygulaması.Application.Interfaces
{
    public interface IStajyerService
    {
        Task<IReadOnlyList<StajyerDto>> GetAllAsync();
        Task<StajyerDto?> GetByIdAsync(int id);
        Task<int> AddAsync(StajyerCreateDto dto);
        Task UpdateAsync(StajyerUpdateDto dto);
        Task DeleteAsync(int id);
    }
}

