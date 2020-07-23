using System;
using System.Text.Json.Serialization;

namespace aiof.auth.data
{
    public class ClientRefreshToken : IClientRefreshToken,
        IPublicKeyId
    {
        public int Id { get; set; }
        public Guid PublicKey { get; set; } = Guid.NewGuid();
        public string Token { get; set; } = Utils.GenerateApiKey(64);
        public int ClientId { get; set; }

        [JsonIgnore]
        public Client Client { get; set; }
        
        public DateTime Created { get; set; } = DateTime.UtcNow;
        public DateTime Expires { get; set; }
        public DateTime? Revoked { get; set; }
    }
}