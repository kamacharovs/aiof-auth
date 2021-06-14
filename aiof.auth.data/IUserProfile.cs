using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;

namespace aiof.auth.data
{
    public interface IUserProfile
    {
        [JsonIgnore]
        int Id { get; set; }

        [JsonIgnore]
        Guid PublicKey { get; set; }

        [JsonIgnore]
        int UserId { get; set; }
    }
}