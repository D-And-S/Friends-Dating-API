using Friends_Date_API.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Friends_Date_API.Data
{
    public class Seed
    {
        // since we have used microsoft identity we need use UserManager in seeding
        public static async Task SeedUsers(UserManager<User> userManager, RoleManager<Role> roleManger)
        {
            if (await userManager.Users.AnyAsync()) return;

            //Read the file Data
            var userData = await System.IO.File.ReadAllTextAsync("Data/UserSeedData.json");

            // deserialize user data 
            var users = JsonSerializer.Deserialize<List<User>>(userData);


            var roles = new List<Role>()
            {
                new Role{Name="Member"},
                new Role{Name= "Admin"},
                new Role{Name= "Moderator"},
            };

            foreach (var role in roles)
            {
                await roleManger.CreateAsync(role);
            }

            foreach (var user in users)
            {
                user.UserName = user.UserName.ToLower();

                //we do not need to use since we have used microsoft identity
                /*using var hmac = new HMACSHA512();
                user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes("123456"));
                user.PasswordSalt = hmac.Key;*/

                //context.Users.AddAsync(user);
                await userManager.CreateAsync(user, "123456");
                await userManager.AddToRoleAsync(user, "Member");

            }

            // we don't need saveChangeAsynch because UserManager will take care of this
            //await context.SaveChangesAsync();

            var admin = new User
            {
                UserName = "admin"
            };
            await userManager.CreateAsync(admin, "123456");
            await userManager.AddToRolesAsync(admin, new[] { "Admin", "Moderator" });
        }
    }
}
