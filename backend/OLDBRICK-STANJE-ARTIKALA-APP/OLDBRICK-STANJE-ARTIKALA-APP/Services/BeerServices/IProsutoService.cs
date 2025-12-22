using OLDBRICK_STANJE_ARTIKALA_APP.DTOs.DailyReports;

namespace OLDBRICK_STANJE_ARTIKALA_APP.Services.BeerServices
{
    public interface IProsutoService
    {
        Task<ProsutoResultDto> CalculateAndSaveAsync(int idNaloga);

        Task<ProsutoResultDto> GetAllStatesByIdNaloga(int idNaloga);

        Task UpdateProsutoKantaAsync(int idNaloga, float prosutoKanta);

        Task<float> CalculateAndSaveProsutoRazlikaAsync(int idNaloga);
    }
}
