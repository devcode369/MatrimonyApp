using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Services.Inerfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [Authorize]
    public class UsersController : BaseController
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public UsersController(IUserRepository userRepository,IMapper mapper )
        {

            _userRepository = userRepository;
            _mapper = mapper;
        }


 
        [HttpGet]
       public async Task<ActionResult<IEnumerable<MemberDTO>>> GetUsers()
       {
        var user=await _userRepository.GetMemberAsync();


         return new  JsonResult(user);
       }

        [HttpGet("{userName}")]
       public async Task<ActionResult<MemberDTO>> GetUsers(string userName)
       {
         
          return  await  _userRepository.GetMemberAsync(userName);

       }

       [HttpPut]
       public async Task<ActionResult> UpdateUser(MemberUpdateDto memberUpdateDto )
       {
          var userName=User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
          var user=await _userRepository.GetUserBynameAsync(userName);
          if(user==null) return NotFound();

          _mapper.Map(memberUpdateDto,user);

          if(await _userRepository.SaveAllAsync()) return NoContent();

           return BadRequest("Failed to update user ");
       }
    }
}