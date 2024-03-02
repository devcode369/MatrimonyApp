using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using API.Entities;
using API.Services.Inerfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [Authorize]
    public class UsersController : BaseController
    {
        private readonly IUserRepository _userRepository;

        public UsersController(IUserRepository userRepository)
        {

            _userRepository = userRepository;
        }


 
        [HttpGet]
       public async Task<ActionResult<IEnumerable<AppUser>>> GetUsers()
       {

         return new  JsonResult(await _userRepository.GetUsersAsync());
       }

        [HttpGet("{userName}")]
       public async Task<ActionResult<AppUser>> GetUsers(string userName)
       {
          return await  _userRepository.GetUserBynameAsync(userName);
       }
    }
}