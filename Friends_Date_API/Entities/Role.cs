using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace Friends_Date_API.Entities
{
    // we will derived form the identity role class
    // since role and user has many to many relationship thats why we need UserRole entity
    public class Role : IdentityRole<int> 
    {
        public ICollection<UserRole> UserRoles { get; set; }
    }
}
