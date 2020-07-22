using System;

namespace aiof.auth.data
{
    public class ClientRefreshToken : IClientRefreshToken,
        IPublicKeyId
    {
        public int Id { get; set; }
        public Guid PublicKey { get; set; } = Guid.NewGuid();
        public int ClientId { get; set; }
        public Client Client { get; set; }
        public DateTime GeneratedOn { get; set; } = DateTime.UtcNow;
        public string RefreshToken { get; set; } = Utils.GenerateApiKey(64);
    }

    public class ClientRefreshTokenDto
    {
        public int ClientId { get; set; }
    }
}