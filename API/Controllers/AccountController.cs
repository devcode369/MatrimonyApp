using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Services.Inerfaces;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class AccountController : BaseController
    {
        private readonly DataContext _dataContext;
        private readonly ITokenServices _tokenServices;
        private readonly IMapper _mapper;
        public AccountController(DataContext dataContext,ITokenServices tokenServices,IMapper mapper)
        {
            _mapper = mapper;
            _tokenServices = tokenServices;
            _dataContext = dataContext;
            
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserDTO>> Register(RegisterDTO registerDTO)
        {
            if(await UserExists(registerDTO.UserName))
            { 
            return BadRequest("UserName Already taken...");
            }

            var user=_mapper.Map<AppUser>(registerDTO);

            using var hmac=new HMACSHA512();

          
              user.UserName=registerDTO.UserName.ToLower();
              user.PasswordHash=hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDTO.Password));
              user.PasswordSalt=hmac.Key;
           

            _dataContext.Users.Add(user);
            await _dataContext.SaveChangesAsync();
            return  new UserDTO{
                UserName=user.UserName,
                Token=_tokenServices.GenerateToken(user),
                KnownAs=user.KnownAs
            };

        }
         
         [HttpPost("login")]
          public async Task<ActionResult<UserDTO>>Login(LoginDTO loginDTO)
          {
              var user =await _dataContext.Users
                                        .Include(p=>p.Photos)
                                          .FirstOrDefaultAsync(x=>x.UserName.ToLower()==loginDTO.UserName.ToLower());

              if(user is not {})
              {
                return Unauthorized("Invalid Username");
              }
              using var hmac=new HMACSHA512(user.PasswordSalt);
              var computedhash=hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDTO.Password));
             for(int i=0;i<computedhash.Length;i++)
             {
                if(computedhash[i]!=user.PasswordHash[i])
                {
                    return Unauthorized("Invalid Password");
                }
             } 

             return  new UserDTO{
                UserName=user.UserName,
                Token=_tokenServices.GenerateToken(user),
                PhotoUrl=user.Photos.FirstOrDefault(p=>p.IsMain)?.Url
            };
;

          }

        private async Task<bool> UserExists(string userName)
        {
            return await _dataContext.Users.AnyAsync(x=>x.UserName == userName.ToLower().ToLower());
        }
        
    }
}