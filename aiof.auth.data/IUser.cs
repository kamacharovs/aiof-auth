using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;

namespace aiof.auth.data
{
    public interface IUser
    {
        [Required]
        int Id { get; set; }

        [Required]
        Guid PublicKey { get; set; }

        [Required]
        [MaxLength(200)]
        string FirstName { get; set; }

        [Required]
        [MaxLength(200)]
        string LastName { get; set; }

        [Required]
        [EmailAddress]
        [MaxLength(200)]
        string Email { get; set; }

        [Required]
        [MaxLength(200)]
        string Username { get; set; }

        [JsonIgnore]
        [Required]
        [MaxLength(100)]
        string Password { get; set; }
        
        [JsonIgnore]
        [Required]
        [MaxLength(100)]
        string PrimaryApiKey { get; set; }

        [JsonIgnore]
        [Required]
        [MaxLength(100)]
        string SecondaryApiKey { get; set; }

        [Required]
        DateTime Created { get; set; }

        [JsonIgnore]
        ICollection<UserRefreshToken> RefreshTokens { get; set; }
    }
}