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
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class AccountController : BaseController
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly ITokenServices _tokenServices;
        private readonly IMapper _mapper;
        public AccountController(UserManager<AppUser> userManager, ITokenServices tokenServices, IMapper mapper)
        {
            _mapper = mapper;
            _userManager = userManager;
            _tokenServices = tokenServices;


        }

        [HttpPost("register")]
        public async Task<ActionResult<UserDTO>> Register(RegisterDTO registerDTO)
        {
            if (await UserExists(registerDTO.UserName))
            {
                return BadRequest("UserName Already taken...");
            }

            var user = _mapper.Map<AppUser>(registerDTO);


            user.UserName = registerDTO.UserName.ToLower();
            var result = await _userManager.CreateAsync(user, registerDTO.Password);

            if (!result.Succeeded) return BadRequest(result.Errors);
     var roleResult=await _userManager.AddToRoleAsync(user,"Member");

     if(!roleResult.Succeeded) return BadRequest(roleResult.Errors);
     
            return new UserDTO
            {
                UserName = user.UserName,
                Token = await _tokenServices.GenerateToken(user),
                KnownAs = user.KnownAs,
                Gender = user.Gender
            };

        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDTO>> Login(LoginDTO loginDTO)
        {
            var user = await _userManager.Users
                                      .Include(p => p.Photos)
                                        .FirstOrDefaultAsync(x => x.UserName.ToLower() == loginDTO.UserName.ToLower());

            if (user is not { })
            {
                return Unauthorized("Invalid Username");
            }

            var result = await _userManager.CheckPasswordAsync(user, loginDTO.Password);

            if (!result) return Unauthorized("Invalid password");

            return new UserDTO
            {
                UserName = user.UserName,
                Token = await _tokenServices.GenerateToken(user),
                PhotoUrl = user.Photos.FirstOrDefault(p => p.IsMain)?.Url,
                KnownAs = user.KnownAs,
                Gender = user.Gender

            };
            ;

        }

        private async Task<bool> UserExists(string userName)
        {
            return await _userManager.Users.AnyAsync(x => x.UserName == userName.ToLower().ToLower());
        }

    }
}