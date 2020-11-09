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
        [Required]
        public int Id { get; set; }

        [JsonIgnore]
        [Required]
        public Guid PublicKey { get; set; } = Guid.NewGuid();

        [JsonIgnore]
        [Required]
        public int UserId { get; set; }
    }
}