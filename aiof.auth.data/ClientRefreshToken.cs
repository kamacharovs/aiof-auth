using System;

namespace aiof.auth.data
{
    public class ClientRefreshToken : IClientRefreshToken,
        IPublicKeyId
    {
        public ClientRefreshToken()
        {
            IsExpired = Created >= Expires;
            IsActive = Revoked == null && !IsExpired;
        }

        public int Id { get; set; }
        public Guid PublicKey { get; set; } = Guid.NewGuid();
        public string Token { get; set; } = Utils.GenerateApiKey(64);
        public int ClientId { get; set; }
        public Client Client { get; set; }
        public DateTime Created { get; set; } = DateTime.UtcNow;
        public DateTime Expires { get; set; } = DateTime.UtcNow.AddDays(1);
        public bool IsExpired { get; set; }
        public DateTime? Revoked { get; set; }
        public bool IsActive { get; set; }
    }

    public class ClientRefreshTokenDto
    {
        public int ClientId { get; set; }
    }
}