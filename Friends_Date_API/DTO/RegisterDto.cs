using System.ComponentModel.DataAnnotations;

namespace Friends_Date_API.DTO
{
    public class RegisterDto
    {
        [Required]
        public string UserName { get; set; }

        [Required] 
        [StringLength(15, MinimumLength =4)]
        public string Password { get; set; }
    }
}
