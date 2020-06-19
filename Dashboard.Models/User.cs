using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Dashboard.Models
{
    public class User
    {
        public int UserId { get; set; }

        [Required]
        public string Email { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }

        [JsonIgnore]
        [Required]
        public string Password { get; set; }
        public bool IsActive { get; set; }
        public bool isPermanent { get; set; }
        public string[] Claims { get; set; }
        public IList<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
    }
}
