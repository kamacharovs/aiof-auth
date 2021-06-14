using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;

namespace aiof.auth.data
{
    public interface IUser
    {
        int Id { get; set; }
        Guid PublicKey { get; set; }
        string FirstName { get; set; }
        string LastName { get; set; }

        [Required]
        string Email { get; set; }

        [JsonIgnore]
        string Password { get; set; }
        
        [JsonIgnore]
        string PrimaryApiKey { get; set; }

        [JsonIgnore]
        string SecondaryApiKey { get; set; }
        
        [JsonIgnore]
        [Required]
        int RoleId { get; set; }
        
        Role Role { get; set; }
        DateTime Created { get; set; }

        [JsonIgnore]
        bool IsDeleted { get; set; }

        [JsonIgnore]
        ICollection<UserRefreshToken> RefreshTokens { get; set; }
    }
}