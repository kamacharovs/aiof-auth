using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;

namespace aiof.auth.data
{
    public class UserRefreshToken : 
        IPublicKeyId
    {
        [JsonIgnore]
        [Required]
        public int Id { get; set; }

        [JsonIgnore]
        [Required]
        public Guid PublicKey { get; set; } = Guid.NewGuid();

        [Required]
        public string Token { get; set; } = Utils.GenerateApiKey<User>(64);

        [Required]
        public int UserId { get; set; }

        [Required]
        public DateTime Created { get; set; } = DateTime.UtcNow;

        [Required]
        public DateTime Expires { get; set; }

        public DateTime? Revoked { get; set; }
    }
}
