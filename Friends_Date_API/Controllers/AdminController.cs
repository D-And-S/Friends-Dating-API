using Friends_Date_API.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace Friends_Date_API.Controllers
{
    public class AdminController : BaseAPIController
    {
        private readonly UserManager<User> _userManager;

        public AdminController(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        [Authorize(Policy = "RequireAdminRole")]
        [HttpGet("users-with-roles")]

        public async Task<ActionResult> GetUsersWithRoles()
        {
            var users = await _userManager.Users
                .Include(x => x.UserRoles)
                .ThenInclude(x => x.Role)
                .OrderBy(u => u.UserName)
                .Select(u => new
                {
                    u.Id,
                    Username = u.UserName,
                    roles = u.UserRoles.Select(r => r.Role.Name).ToList()
                })
                .ToListAsync();

            // we can add pagination here if we want
            return Ok(users);
        }

        [HttpPost("edit-roles/{username}")]
        public async Task<ActionResult> EditRoles(string username, [FromQuery] string roles)
        {
            var selectedRoles = roles.Split(",").ToArray();

            // find user by username
            var user = await _userManager.FindByNameAsync(username);

            if (user == null) return NotFound("Could not find user");

            // provide roles of particular user
            var userRoles = await _userManager.GetRolesAsync(user);

            // it will select the role expcept the role user currently in
            var result = await _userManager.AddToRolesAsync(user, selectedRoles.Except(userRoles));

            if (!result.Succeeded) return BadRequest("Failed to add to roles");

            // now our role is selected. and user currently je role e ache seta bad diye
            // je role select kora hoyeche seta establish korbe
            result = await _userManager.RemoveFromRolesAsync(user, userRoles.Except(selectedRoles));

            if (!result.Succeeded) return BadRequest("Failed to remove from roles");

            // selected role return kora 
            return Ok(await _userManager.GetRolesAsync(user));
        }

        [Authorize(Policy = "ModeratorPhotoRole")]
        [HttpGet("photos-to-moderate")]
        public ActionResult GetPhotosForModeration()
        {
            return Ok("Admin And moderator can see this");
        }


    }
}
