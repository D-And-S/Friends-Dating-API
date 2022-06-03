using Microsoft.AspNetCore.Identity;

namespace Friends_Date_API.Entities
{
    public class UserRole : IdentityUserRole<int>
    {
        // we define the join entities that we need for
        public User User { get; set; }
        public Role Role { get; set; }

    }
}
