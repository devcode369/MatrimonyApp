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
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class AccountController : BaseController
    {
        private readonly DataContext _dataContext;
        private readonly ITokenServices _tokenServices;
        public AccountController(DataContext dataContext,ITokenServices tokenServices)
        {
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

            using var hmac=new HMACSHA512();

            var user=new AppUser{
              UserName=registerDTO.UserName.ToLower(),
              PasswordHash=hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDTO.Password)),
              PasswordSalt=hmac.Key
            };

            _dataContext.Users.Add(user);
            await _dataContext.SaveChangesAsync();
            return  new UserDTO{
                UserName=user.UserName,
                Token=_tokenServices.GenerateToken(user)
            };

        }
         
         [HttpPost("login")]
          public async Task<ActionResult<UserDTO>>Login(LoginDTO loginDTO)
          {
              var user =await _dataContext.Users.FirstOrDefaultAsync(x=>x.UserName.ToLower()==loginDTO.UserName.ToLower());

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
                Token=_tokenServices.GenerateToken(user)
            };
;

          }

        private async Task<bool> UserExists(string userName)
        {
            return await _dataContext.Users.AnyAsync(x=>x.UserName == userName.ToLower().ToLower());
        }
        
    }
}