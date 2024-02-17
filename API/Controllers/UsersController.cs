using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class UsersController : BaseController
    {
        private readonly DataContext _dataContext;

        public UsersController(DataContext dataContext)
        {
           
            _dataContext = dataContext;
        }


        [HttpGet]
       public async Task<ActionResult<IEnumerable<AppUser>>> GetUsers()
       {
         var users=await _dataContext.Users.ToListAsync();
         return new  JsonResult(users);
       }

        [HttpGet("{id}")]
       public async Task<ActionResult<AppUser>> GetUsers(int id)
       {
          return await  _dataContext.Users.FindAsync(id);
       }
    }
}