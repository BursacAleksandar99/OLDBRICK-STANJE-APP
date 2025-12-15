using OLDBRICK_STANJE_ARTIKALA_APP.DTOs.Users;

namespace OLDBRICK_STANJE_ARTIKALA_APP.Services.User
{
    public interface IUserService
    {
        Task<CreateUserResponseDto> CreateUserAsync(CreateUserRequestDto request);
    }
}
