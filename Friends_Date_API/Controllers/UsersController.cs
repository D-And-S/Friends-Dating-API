using AutoMapper;
using Friends_Date_API.Data;
using Friends_Date_API.DTO;
using Friends_Date_API.Entities;
using Friends_Date_API.Extension;
using Friends_Date_API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Friends_Date_API.Controllers
{
    public class UsersController : BaseAPIController
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly IPhotoService _photoService;

        public UsersController(IUserRepository userRepository, 
                               IMapper mapper,
                               IPhotoService photoService)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _photoService = photoService;
        }
        
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<MemberDto>>> GetAllUsers()
        {
            return Ok(await _userRepository.GetAllMembersAsync());
        }
    
        [HttpGet("{id}")]
        public async Task<ActionResult<MemberDto>> GetUser(int id)
        {
            var user = await _userRepository.GetUserByIdAsync(id);
            return _mapper.Map<MemberDto>(user);
        }

        [HttpGet("GetUserByUsername/{username}", Name = "GetUserByUsername")]
        public async Task<ActionResult<MemberDto>> GetUser(string username)
        {
            return await _userRepository.GetMemberByUserNameAsync(username);
        }

        [HttpPut("UpdateUser")]
        public async Task<ActionResult> UpdateUser(MemberUpdateDto memberUpdateDto)
        {
            //User.GetUsername() gives the user name from toaken that API uses to authenticate of the user
            var user = await _userRepository.GetUserByUsernameAsync(User.GetUsername());
            
            _mapper.Map(memberUpdateDto,user);

            _userRepository.Update(user);

            if (await _userRepository.SaveAllsync()) return NoContent();

            return BadRequest("Faild to update user");
        }

        [HttpPost("Add-Photo")]
        public async Task<ActionResult<PhotoDto>> AddPhoto(IFormFile file)
        {
            var user = await _userRepository.GetUserByUsernameAsync(User.GetUsername());

            var result = await _photoService.AddPhotoAsynch(file);

            if (result.Error != null) return BadRequest(result.Error.Message);

            var photo = new Photo
            {
                Url = result.SecureUrl.AbsoluteUri,
                PublicId = result.PublicId,
            };

            if (user.Photos.Count == 0)
            {
                photo.IsMain = true;
            }

            user.Photos.Add(photo);

            if (await _userRepository.SaveAllsync())
            {
                //return _mapper.Map<PhotoDto>(photo);
                return CreatedAtRoute("GetUserByUsername", new { username = user.UserName }, _mapper.Map<PhotoDto>(photo));

            }

            return BadRequest("Problem Adding Photo");
        }

        [HttpPut("set-main-photo/{photoId}")]
        public async Task<ActionResult> SetMainPhoto(int photoId)
        {
            var user = await _userRepository.GetUserByUsernameAsync(User.GetUsername());

            var photo = user.Photos.FirstOrDefault(p => p.Id == photoId);

            if (photo.IsMain) return BadRequest("This is already your main photo");

            //get the current main photo and make it false
            var currntMainPhoto = user.Photos.FirstOrDefault(x=>x.IsMain);
            if (currntMainPhoto != null) currntMainPhoto.IsMain = false;
            photo.IsMain = true;

            if (await _userRepository.SaveAllsync()) return NoContent();

            // in case if any thing wrong happen
            return BadRequest("Failed to set main photo");
        }

        [HttpDelete("delete-photo/{photoId}")]
        public async Task<ActionResult> DeletePhoto(int photoId)
        {
            var user = await _userRepository.GetUserByUsernameAsync(User.GetUsername());
            
            var photo = user.Photos.FirstOrDefault(x=>x.Id == photoId);

            if (photo == null) return NotFound();

            if (photo.IsMain) return BadRequest("You cannot delete your main photo");

            if(photo.PublicId != null)
            {
                var result = await _photoService.DeletePhotoAsync(photo.PublicId);

                //if we have any problem to delete with photo 
                if (result.Error != null) return BadRequest(result.Error.Message);
            }

            user.Photos.Remove(photo);

            if (await _userRepository.SaveAllsync()) return Ok();

            return BadRequest("Failed to Delete This Photo");

        }
    }
}
