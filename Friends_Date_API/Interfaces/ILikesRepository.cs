using Friends_Date_API.DTO;
using Friends_Date_API.Entities;
using Friends_Date_API.Helpers;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Friends_Date_API.Interfaces
{
    public interface ILikesRepository
    {
        Task<UserLike> GetUserLike(int sourceId, int likedUserId);
        Task<User> GetUserWithLikes(int userId);
        Task<PageList<LikeDto>> GetUserLikes(LikesParams likesParams);

    }
}
