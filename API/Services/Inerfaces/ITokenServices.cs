using API.Entities;

namespace API.Services.Inerfaces
{
    public interface ITokenServices
    {
        Task<string> GenerateToken(AppUser appUser);
    }
}