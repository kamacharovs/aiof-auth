using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;

namespace aiof.auth.data
{
    public class User : IUser, 
        IPublicKeyId, IApiKey, IIsDeleted
    {
        public int Id { get; set; }
        public Guid PublicKey { get; set; } = Guid.NewGuid();
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }

        [JsonIgnore]
        public string Password { get; set; }

        [JsonIgnore]
        public string PrimaryApiKey { get; set; }

        [JsonIgnore]
        public string SecondaryApiKey { get; set; }

        [JsonIgnore]
        public int RoleId { get; set; }

        public Role Role { get; set; }
        public DateTime Created { get; set; } = DateTime.UtcNow;

        [JsonIgnore]
        public bool IsDeleted { get; set; } = false;

        [JsonIgnore]
        public ICollection<UserRefreshToken> RefreshTokens { get; set; } = new List<UserRefreshToken>();
    }

    public class UserDto
    {
        [Required]
        [MaxLength(200)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(200)]
        public string LastName { get; set; }

        [Required]
        [MaxLength(200)]
        public string Email { get; set; }

        [Required]
        [MaxLength(100)]
        public string Password { get; set; }
    }
}
