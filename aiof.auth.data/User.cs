using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;

namespace aiof.auth.data
{
    public class User : IUser, 
        IPublicKeyId, IApiKey, IIsDeleted
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public Guid PublicKey { get; set; } = Guid.NewGuid();

        [Required]
        [MaxLength(200)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(200)]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        [MaxLength(200)]
        public string Email { get; set; }

        [Required]
        [MaxLength(200)]
        public string Username { get; set; }

        [JsonIgnore]
        [Required]
        [MaxLength(100)]
        public string Password { get; set; }

        [JsonIgnore]
        [MaxLength(100)]
        public string PrimaryApiKey { get; set; }

        [JsonIgnore]
        [MaxLength(100)]
        public string SecondaryApiKey { get; set; }

        [JsonIgnore]
        [Required]
        public int RoleId { get; set; }
        
        [Required]
        public Role Role { get; set; }

        [Required]
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
        [MaxLength(200)]
        public string Username { get; set; }

        [Required]
        [MaxLength(100)]
        public string Password { get; set; }
    }
}
