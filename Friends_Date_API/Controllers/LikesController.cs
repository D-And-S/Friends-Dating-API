using Friends_Date_API.DTO;
using Friends_Date_API.Entities;
using Friends_Date_API.Extension;
using Friends_Date_API.Helpers;
using Friends_Date_API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Friends_Date_API.Controllers
{
    [Authorize]
    public class LikesController : BaseAPIController
    {
        private readonly IUnitOfWork _unitOfWork;
        public LikesController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpPost("{username}")]
        public async Task<IActionResult> AddLikes(string username)
        {
            var sourceUserId = User.GetUserId();
            var likedUser = await _unitOfWork.UserRepository.GetUserByUsernameAsync(username);
            var sourceUser = await _unitOfWork.LikesRepository.GetUserWithLikes(sourceUserId);

            if (likedUser == null) return NotFound();

            if (sourceUser == null) return BadRequest("You cannot like yourself");

            var userLike = await _unitOfWork.LikesRepository.GetUserLike(sourceUserId, likedUser.Id);

            if (userLike != null) return BadRequest("You already like this user");

            userLike = new UserLike
            {
                SourceUser = sourceUser,
                Likeduser = likedUser,
            };

            sourceUser.LikedUser.Add(userLike);

            if (await _unitOfWork.Complete()) return Ok();

            return BadRequest("Faild to like user");
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<LikeDto>>> GetUserLikes([FromQuery]LikesParams likesParams)
        {  
            likesParams.UserId = User.GetUserId();
            var users = await _unitOfWork.LikesRepository.GetUserLikes(likesParams);

            Response.AddPaginationHeader(users.CurrentPage, users.PageSize, 
                users.TotalCount, users.TotalPages);

            return Ok(users);
        }
    }
}
