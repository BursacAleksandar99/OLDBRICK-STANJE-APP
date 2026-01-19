using OLDBRICK_STANJE_ARTIKALA_APP.DTOs.Beers;
using OLDBRICK_STANJE_ARTIKALA_APP.Entities;

namespace OLDBRICK_STANJE_ARTIKALA_APP.Services.BeerServices
{
    public interface IBeerService
    {
        Task<Beer> CreateAsync(CreateBeerRequest request);

        Task<Beer> GetByIdAsync(int id);
        Task<List<Beer>> GetAllBeersAsync();

        Task SaveDailyBeerShortageAsync(int idNaloga);
        Task<List<BeerShortageSumDto>> GetBeerShortageTotalsSinceLastInventoryAsync(int idNaloga);
    }
}
