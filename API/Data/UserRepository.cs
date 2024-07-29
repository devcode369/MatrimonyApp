namespace API.Data
{
    using API.DTOs;
    using API.Entities;
    using API.Helpers;
    using API.Services.Inerfaces;
    using AutoMapper;
    using AutoMapper.QueryableExtensions;
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class UserRepository : IUserRepository
    {
        private readonly DataContext _dataContext;
        private readonly IMapper _mapper;

        public UserRepository(DataContext dataContext, IMapper mapper)
        {
            _mapper = mapper;
            _dataContext = dataContext;
        }

        public async Task<PagedList<MemberDTO>> GetMemberAsync(UserParams userParams)
        {


            var query = _dataContext.Users.AsQueryable();
            query = query.Where(u => u.UserName != userParams.CurrentUserName);
            query = query.Where(u => u.Gender == userParams.Gender);
            var minDob = DateOnly.FromDateTime(DateTime.Today.AddYears(-userParams.MaxAge - 1));
            var maxDob = DateOnly.FromDateTime(DateTime.Today.AddYears(-userParams.MinAge));
            query = query.Where(u => u.DateOfBirth >= minDob && u.DateOfBirth <= maxDob);
            query = userParams.OrderBy switch
            {
                "created" => query.OrderByDescending(u => u.Created),
                _ => query.OrderByDescending(u => u.LastActive)
            };

            //  return await PagedList<MemberDTO>.CreateAsync(query,userParams.PageNumber,userParams.PageSize,)

            //     var query = _dataContext.Users.ProjectTo<MemberDTO>(_mapper.ConfigurationProvider)
            //                                    .AsNoTracking();

            return await PagedList<MemberDTO>.CreateAsync(
             query.AsNoTracking().ProjectTo<MemberDTO>(_mapper.ConfigurationProvider),
             userParams.PageNumber,
             userParams.PageSize
             );

        }

        public async Task<MemberDTO> GetMemberAsync(string username)
        {
            return await _dataContext.Users.Where(x => x.UserName == username)
                         .ProjectTo<MemberDTO>(_mapper.ConfigurationProvider)
                         .FirstOrDefaultAsync();
        }

        public async Task<AppUser> GetUserByIdAsync(int id)
        {
            var user = await _dataContext.Users.FindAsync(id);

            return user;
        }

        public async Task<AppUser> GetUserBynameAsync(string userName)
        {
            return await _dataContext.Users.Include(c => c.Photos).FirstOrDefaultAsync(c => c.UserName.Contains(userName));
        }

        public async Task<IEnumerable<AppUser>> GetUsersAsync()
        {
            return await _dataContext.Users.Include(c => c.Photos).ToListAsync();
        }

        public async Task<bool> SaveAllAsync()
        {
            return await _dataContext.SaveChangesAsync() > 0;
        }

        public async void Update(AppUser user)
        {
            _dataContext.Entry(user).State = EntityState.Modified;
        }
    }
}