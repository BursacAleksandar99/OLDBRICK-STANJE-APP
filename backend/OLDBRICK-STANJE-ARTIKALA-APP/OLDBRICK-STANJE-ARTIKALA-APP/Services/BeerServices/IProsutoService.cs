using OLDBRICK_STANJE_ARTIKALA_APP.DTOs.DailyReports;
using OLDBRICK_STANJE_ARTIKALA_APP.Entities;

namespace OLDBRICK_STANJE_ARTIKALA_APP.Services.BeerServices
{
    public interface IProsutoService
    {
        Task<ProsutoResultDto> CalculateAndSaveAsync(int idNaloga);

        Task<ProsutoResultDto> GetAllStatesByIdNaloga(int idNaloga);

        Task UpdateProsutoKantaAsync(int idNaloga, float prosutoKanta);

        Task<float> CalculateAndSaveProsutoRazlikaAsync(int idNaloga);

        Task<List<DailyBeerState>> CalculateAndUpdateProsutoForReportAsync(int idNaloga);
    }
}
