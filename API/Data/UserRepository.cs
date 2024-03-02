namespace API.Data
{
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
    using API.DTOs;
    using API.Entities;
 using API.Services.Inerfaces;
    using AutoMapper;
    using AutoMapper.QueryableExtensions;
    using Microsoft.EntityFrameworkCore;

    public class UserRepository : IUserRepository
    {
        private readonly DataContext _dataContext;
        private readonly IMapper _mapper;

        public UserRepository(DataContext dataContext,IMapper mapper)
        {
            _mapper = mapper;
            _dataContext = dataContext;
        }

        public async Task<IEnumerable<MemberDTO>> GetMemberAsync()
        {
            return  await _dataContext.Users.ProjectTo<MemberDTO>(_mapper.ConfigurationProvider)
                          .ToListAsync();
        }

        public async Task<MemberDTO> GetMemberAsync(string username)
        {
            return await _dataContext.Users.Where(x=>x.UserName==username)
                         .ProjectTo<MemberDTO>(_mapper.ConfigurationProvider)
                         .FirstOrDefaultAsync();
        }

        public async Task<AppUser> GetUserByIdAsync(int id)
        {
            var user= await _dataContext.Users.FindAsync(id);

            return user;
        }

        public async Task<AppUser> GetUserBynameAsync(string userName)
        {
           return await _dataContext.Users.Include(c=>c.Photos).FirstOrDefaultAsync(c=>c.UserName.Contains(userName));
        }

        public async Task<IEnumerable<AppUser>> GetUsersAsync()
        {
            return await _dataContext.Users.Include(c=>c.Photos).ToListAsync();
        }

        public async Task<bool> SaveAllAsync()
        {
          return await _dataContext.SaveChangesAsync() >0;
        }

        public async void Update(AppUser user)
        {
           _dataContext.Entry(user).State=EntityState.Modified;
        }
    }
}