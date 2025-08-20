using Application.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IBasvuruService
    {
        Task<List<BasvuruListDto>> TumBasvurulariGetirAsync();
        Task<BasvuruDto?> BasvuruDetayAsync(int id);
        Task<int> BasvuruOlusturAsync(BasvuruCreateDto dto);
        Task<bool> BasvuruGuncelleAsync(BasvuruUpdateDto dto);
        Task<bool> BasvuruOnaylaAsync(int id);
        Task<bool> BasvuruReddetAsync(BasvuruReddetDto dto);
    }
}
