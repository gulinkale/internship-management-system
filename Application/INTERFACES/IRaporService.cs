using Application.DTOs;
using StajTakipUygulaması.Application.DTOs;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace StajTakipUygulaması.Application.Interfaces
{
    public interface IRaporService
    {
        Task<List<RaporStajyerDto>> AraAsync(string arama, int belgeTipId, CancellationToken ct = default);
    }
}
