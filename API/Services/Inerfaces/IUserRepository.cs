namespace API.Services.Inerfaces
{
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
    using API.DTOs;
    using API.Entities;
    using API.Helpers;

    public interface IUserRepository
    {
        void Update(AppUser user);
        Task<bool> SaveAllAsync();
        Task<IEnumerable<AppUser>>GetUsersAsync();
        Task<AppUser>GetUserByIdAsync(int id);
        Task<AppUser>GetUserBynameAsync(string userName);

        Task<PagedList<MemberDTO>> GetMemberAsync(UserParams userParams);

        Task<MemberDTO> GetMemberAsync(string username);
    }
}