using AutoMapper;
using Friends_Date_API.Data;
using Friends_Date_API.DTO;
using Friends_Date_API.Entities;
using Friends_Date_API.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Friends_Date_API.Controllers
{
    public class AccountController : BaseAPIController
    {
        private readonly DataContext _context;
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public AccountController(DataContext context,
                                 ITokenService tokenService,
                                 IMapper mapper,
                                 UserManager<User> userManager,
                                 SignInManager<User> signInManager)
        {
            _context = context;
            _tokenService = tokenService;
            _mapper = mapper;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpPost("Register")]
        public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
        {
            //check user name is exist or not
            if (await UserExists(registerDto.UserName))
                return BadRequest("Username is taken");

            var user = _mapper.Map<User>(registerDto);

            //set value to Users Object

            user.UserName = registerDto.UserName.ToLower();

            // --> We do not nedd to use this because we have used microsoft identity
            /*
                //use security class to make password hast
                using var hmac = new HMACSHA512();
                user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password));
                user.PasswordSalt = hmac.Key;

                _context.Users.Add(user);
                await _context.SaveChangesAsync();
            */

            var result = await _userManager.CreateAsync(user, registerDto.Password);

            if (!result.Succeeded) BadRequest(result.Errors);

            var roleResult = await _userManager.AddToRoleAsync(user, "Member");

            if (!roleResult.Succeeded) return BadRequest(roleResult.Errors);

            return new UserDto
            {
                Username = registerDto.UserName,
                Token = await _tokenService.CreateToken(user),
                KnownAs = registerDto.KnownAs,
                Gender = registerDto.Gender
            };
        }

        private async Task<bool> UserExists(string username)
        {
            return await _userManager.Users.AnyAsync(x => x.UserName == username.ToLower());
        }


        [HttpPost("Login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
        {
            // we will use userManager instead of _context
            var user = await _userManager.Users
                .Include(p => p.Photos)
                .SingleOrDefaultAsync(x => x.UserName == loginDto.UserName.ToLower());

            if (user == null)
                return BadRequest("Invalid Username");

            // we do not need this because we have used microsoft identity
            /*
                //if the user name match then we convert selected user passwordSalt which give us password hash value in byte string
                using var hmac = new HMACSHA512(user.PasswordSalt);

                // we convert user entered password to byte string 
                var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));

                // check weather databash hash and user entered password hash matched
                for (int i = 0; i < computedHash.Length; i++)
                {
                    if (computedHash[i] != user.PasswordHash[i])
                        return Unauthorized("Invalid Password");
                }
            */

            // we use sign in manager to sign in
            var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);

            if (!result.Succeeded) return Unauthorized();

            return new UserDto
            {
                Username = loginDto.UserName,
                Token = await _tokenService.CreateToken(user),
                PhotoUrl = user.Photos.FirstOrDefault(x => x.IsMain)?.Url,
                KnownAs = user.KnownAs,
                Gender = user.Gender
            };
        }
    }
}
