namespace API.Services.Inerfaces
{
    using API.DTOs;
    using API.Entities;
    using API.Helpers;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IUserRepository
    {
        void Update(AppUser user);
        Task<bool> SaveAllAsync();
        Task<IEnumerable<AppUser>> GetUsersAsync();
        Task<AppUser> GetUserByIdAsync(int id);
        Task<AppUser> GetUserBynameAsync(string userName);

        Task<PagedList<MemberDTO>> GetMemberAsync(UserParams userParams);

        Task<MemberDTO> GetMemberAsync(string username);
    }
}