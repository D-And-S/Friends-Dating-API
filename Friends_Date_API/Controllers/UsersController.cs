using Friends_Date_API.Data;
using Friends_Date_API.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Friends_Date_API.Controllers
{
    public class UsersController : BaseAPIController
    {
        private readonly DataContext _context;

        public UsersController(DataContext context)
        {
            _context = context;
        }

        // Return List of AppUser
        
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<AppUser>>> GetAllUsers()
        {        
            return await _context.Users.ToListAsync();
        }

        //since we return the single user that's why our return type is AppUser
        
        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<AppUser>> GetUser(int id)
        {
            return await _context.Users.FindAsync(id);
        }
        
    }


}
