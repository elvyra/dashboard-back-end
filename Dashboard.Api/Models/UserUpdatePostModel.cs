using System.ComponentModel.DataAnnotations;

namespace Dashboard.Api.Models
{
    public class UserUpdatePostModel
    {
        [Required]
        public int UserId { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [StringLength(60, MinimumLength = 2)]
        [RegularExpression(@"^[a-zA-Z]+ ?[a-zA-Z]+$", ErrorMessage = "Only latin letters")]
        public string Name { get; set; }
        [Required]
        [StringLength(60, MinimumLength = 2)]
        [RegularExpression(@"^[a-zA-Z]+ ?[a-zA-Z]+$", ErrorMessage = "Only latin letters, up to 2 surnames")]
        public string Surname { get; set; }
#nullable enable
        [StringLength(20, MinimumLength = 6)]

        // [RegularExpression(@"^[a-zA-Z0-9!@#$%^&]+$", ErrorMessage = "Only latin letters, numbers and symbols !@#$%^&")]

        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{6,20}$", ErrorMessage = "Password policy: Minimum six and maximum 20 characters, only latin letters, at least one uppercase letter, one lowercase letter, one number and one special character")]
        public string? Password { get; set; }
#nullable disable
        public bool IsActive { get; set; }
    }
}
