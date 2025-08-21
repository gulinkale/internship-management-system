using StajTakipUygulaması.Application.DTOs;

namespace StajTakipUygulaması.Application.Interfaces
{
    public interface IBelgeService
    {
        Task<IEnumerable<BelgeDto>> GetAllAsync();
        Task<BelgeDto?> GetByIdAsync(int id);

        Task<int> AddAsync(BelgeCreateDto dto);                // sadece DB meta
        Task<int> UploadAndSaveAsync(BelgeUploadRequest req);  // dosya + DB meta

        Task UpdateAsync(BelgeUpdateDto dto);
    }
}
