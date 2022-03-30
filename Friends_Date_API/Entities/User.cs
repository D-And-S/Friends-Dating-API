using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Friends_Date_API.Extension;

namespace Friends_Date_API.Entities
{
    public class User
    {
        public int Id { get; set; }

        [Required]
        [StringLength(255)]
        public string UserName { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string KnownAs { get; set; }
        public DateTime Created { get; set; } = DateTime.Now;
        public DateTime LastActive { get; set; } = DateTime.Now;
        public string Gender { get; set; }
        public string Introduction { get; set; }
        public string Interests { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public ICollection<Photo> Photos { get; set; }

        //public int GetAge()
        //{
        //    return DateOfBirth.CalculateAge();
        //}
    }
}
