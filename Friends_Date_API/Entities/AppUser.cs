using System.ComponentModel.DataAnnotations;

namespace Friends_Date_API.Entities
{
    public class AppUser
    {
        public int Id { get; set; }

        [Required]
        [StringLength(255)]
        public string UserName { get; set; }
    }
}
