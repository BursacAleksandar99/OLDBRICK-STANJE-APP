using OLDBRICK_STANJE_ARTIKALA_APP.Entities;

namespace OLDBRICK_STANJE_ARTIKALA_APP.Services.Auth
{
    public interface ITokenService
    {
        string CreateToken(Users user);
        
    }
}
