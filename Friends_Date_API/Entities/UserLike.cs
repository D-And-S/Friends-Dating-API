using System.ComponentModel.DataAnnotations.Schema;

namespace Friends_Date_API.Entities
{
    public class UserLike
    {
        public User SourceUser { get; set; } // user who will like other users 
        public int SourceUserId { get; set; }
        public User Likeduser { get; set; } // user who get many like from other user
        public int LikeduserId { get; set; }
    }
}
