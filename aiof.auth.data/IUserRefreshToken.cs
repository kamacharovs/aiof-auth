using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;

namespace aiof.auth.data
{
    public interface IUserRefreshToken
    {
        [JsonIgnore]
        [Required]
        int Id { get; set; }

        [JsonIgnore]
        [Required]
        Guid PublicKey { get; set; }

        [Required]
        string Token { get; set; }

        [JsonIgnore]
        [Required]
        int UserId { get; set; }

        [Required]
        DateTime Created { get; set; }

        [Required]
        DateTime Expires { get; set; }

        DateTime? Revoked { get; set; }
    }
}
