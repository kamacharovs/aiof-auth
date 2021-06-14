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
        int Id { get; set; }

        [JsonIgnore]
        Guid PublicKey { get; set; }

        string Token { get; set; }

        [JsonIgnore]
        int UserId { get; set; }

        DateTime Created { get; set; }
        DateTime Expires { get; set; }
        DateTime? Revoked { get; set; }
    }
}
