using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;

namespace aiof.auth.data
{
    public class UserProfile : IUserProfile,
        IPublicKeyId
    {
        [JsonIgnore]
        public int Id { get; set; }

        [JsonIgnore]
        public Guid PublicKey { get; set; } = Guid.NewGuid();

        [JsonIgnore]
        public int UserId { get; set; }
    }
}