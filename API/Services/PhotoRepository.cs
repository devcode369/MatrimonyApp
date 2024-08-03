using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Services.Inerfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Services
{
    public class PhotoRepository : IPhotoRepository
    {
        private readonly DataContext _dataContext;

        public PhotoRepository(DataContext dataContext)
        {
            _dataContext = dataContext;
        }
        public async Task<Photo> GetPhotoById(int id)
        {
            return await _dataContext.Photos
                 .IgnoreQueryFilters()
                .SingleOrDefaultAsync(x => x.Id == id);
        }

        public async Task<IEnumerable<PhotoForApprovalDto>> GetUnapprovedPhotos()
        {
            return await  _dataContext.Photos
                 .IgnoreQueryFilters()
                 .Where(p => p.IsApproved == false)
                 .Select(u => new PhotoForApprovalDto
                    {
                    Id = u.Id,
                    UserName = u.AppUser.UserName,
                    Url = u.Url,
                    IsApproved = u.IsApproved
                     }).ToListAsync();
        }

        public  void RemovePhoto(Photo photo)
        {
            _dataContext.Photos.Remove(photo);
        }
    }
}