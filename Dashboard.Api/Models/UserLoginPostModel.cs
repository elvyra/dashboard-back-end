using System.ComponentModel.DataAnnotations;

namespace Dashboard.Api.Models
{
    public class UserLoginPostModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [StringLength(20, MinimumLength = 6)]

       // [RegularExpression(@"^[a-zA-Z0-9!@#$%^&]+$", ErrorMessage = "Only latin letters, numbers and symbols !@#$%^&")]
       
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{6,20}$", ErrorMessage = "Password policy: Minimum six and maximum 20 characters, only latin letters, at least one uppercase letter, one lowercase letter, one number and one special character")]
        public string Password { get; set; }
    }
}
