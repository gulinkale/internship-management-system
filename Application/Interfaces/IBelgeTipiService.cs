using StajTakipUygulaması.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

public interface IBelgeTipiService
{
    Task<List<BelgeTipi>> GetAllAsync();
}
