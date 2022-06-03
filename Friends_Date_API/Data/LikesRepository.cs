using Friends_Date_API.DTO;
using Friends_Date_API.Entities;
using Friends_Date_API.Extension;
using Friends_Date_API.Helpers;
using Friends_Date_API.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Friends_Date_API.Data
{
    public class LikesRepository : ILikesRepository
    {
        private DataContext _context;

        public LikesRepository(DataContext context)
        {
            _context = context;
        }
        public async Task<UserLike> GetUserLike(int sourceId, int likedUserId)
        {
            return await _context.Likes.FindAsync(sourceId, likedUserId);
        }

        public async Task<PageList<LikeDto>> GetUserLikes(LikesParams likesParams)
        {
            var users = _context.Users.OrderBy(u => u.UserName).AsQueryable();
            var likes = _context.Likes.AsQueryable();

            //currently logged in user liked other user
            if (likesParams.Predicate == "liked")
            {
                likes = likes.Where(like => like.SourceUserId == likesParams.UserId);
                users = likes.Select(like => like.Likeduser); // user who get many like from other user
            }

            //logged in user get many like from other user
            if (likesParams.Predicate == "likedBy")
            {
                likes = likes.Where(like => like.LikeduserId == likesParams.UserId);
                users = likes.Select(like => like.SourceUser);
            }

            var query = users.Select(user => new LikeDto
            {
                UserName = user.UserName,
                KnownAs = user.KnownAs,
                Age = user.DateOfBirth.CalculateAge(),
                PhotoUrl = user.Photos.FirstOrDefault(p => p.IsMain).Url,
                City = user.City,
                Id = user.Id,
            }).AsQueryable();

            return await PageList<LikeDto>.CreateAsync(query.AsNoTracking(),
                likesParams.PageNumber, likesParams.PageSize);
        }

        public async Task<User> GetUserWithLikes(int userId)


        {
            return await _context.Users
                    .Include(x => x.LikedUser)
                    .FirstOrDefaultAsync(x => x.Id == userId);

        }
    }
}
