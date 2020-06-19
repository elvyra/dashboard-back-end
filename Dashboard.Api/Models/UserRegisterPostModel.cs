using System.ComponentModel.DataAnnotations;

namespace Dashboard.Api.Models
{
    public class UserRegisterPostModel : UserLoginPostModel
    {
        [Required]
        [StringLength(60, MinimumLength = 2)]
        [RegularExpression(@"^[a-zA-Z]+ ?[a-zA-Z]+$", ErrorMessage = "Only latin letters")]
        public string Name { get; set; }
        [Required]
        [StringLength(60, MinimumLength = 2)]
        [RegularExpression(@"^[a-zA-Z]+ ?[a-zA-Z]+$", ErrorMessage = "Only latin letters, up to 2 surnames")]
        public string Surname { get; set; }
        public bool IsActive { get; set; }
    }
}
