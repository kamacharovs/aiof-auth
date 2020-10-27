using System;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;

namespace aiof.auth.data
{
    public interface IClientRefreshToken
    {
        [JsonIgnore]
        [Required]
        int Id { get; set; }

        [JsonIgnore]
        [Required]
        Guid PublicKey { get; set; }

        [Required]
        [MaxLength(100)]
        string Token { get; set; }

        [JsonIgnore]
        [Required]
        int ClientId { get; set; }

        [JsonIgnore]
        [Required]
        Client Client { get; set; }

        [Required]
        DateTime Created { get; set; }

        [Required]
        DateTime Expires { get; set; }
        
        DateTime? Revoked { get; set; }
    }
}