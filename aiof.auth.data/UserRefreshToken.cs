using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;

namespace aiof.auth.data
{
    public class UserRefreshToken : IUserRefreshToken,
        IPublicKeyId
    {
        [JsonIgnore]
        public int Id { get; set; }

        [JsonIgnore]
        public Guid PublicKey { get; set; } = Guid.NewGuid();

        public string Token { get; set; } = Utils.GenerateApiKey<User>(64);

        [JsonIgnore]
        public int UserId { get; set; }

        public DateTime Created { get; set; } = DateTime.UtcNow;
        public DateTime Expires { get; set; }
        public DateTime? Revoked { get; set; }
    }
}
