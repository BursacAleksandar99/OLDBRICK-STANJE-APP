using OLDBRICK_STANJE_ARTIKALA_APP.DTOs.Auth;

namespace OLDBRICK_STANJE_ARTIKALA_APP.Services.Auth
{
    public interface IAuthService
    {
        Task<LoginResponseDto> LoginAsync(LoginRequestDto request);
    }
}
