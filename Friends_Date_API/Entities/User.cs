using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Friends_Date_API.Entities
{
    //by default IdentityUser is string it primary key is string and it has some common field
    // Id  UserName Password. it is overriding from Identity User so don't need to use it 
    // manually and since we need Id primary key as integer that's why we make IDentityUser integer
    // by using generic
    public class User : IdentityUser<int>
    {
        //public int Id { get; set; }

        //[Required]
        //[StringLength(255)]
        //public string UserName { get; set; }
        //public byte[] PasswordHash { get; set; }
        //public byte[] PasswordSalt { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string KnownAs { get; set; }
        public DateTime Created { get; set; } = DateTime.Now;
        public DateTime LastActive { get; set; } = DateTime.Now;
        public string Gender { get; set; }
        public string Introduction { get; set; }
        public string LookingFor { get; set; }
        public string Interests { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public ICollection<Photo> Photos { get; set; }
        public ICollection<UserLike> LikedByUsers { get; set; } // list of user that like currently logged in user (amake koto jon like korse)
        public ICollection<UserLike> LikedUser { get; set; } // list of user liked by currently logged in user (ami koto jon ke like korechi)
        public ICollection<Message> MessageSent { get; set; }
        public ICollection<Message> MessageReceived { get; set; }
        public ICollection<UserRole> UserRoles { get; set; }
    }

}
