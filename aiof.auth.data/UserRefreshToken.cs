using System;

namespace aiof.auth.data
{
    public class UserRefreshToken : IUserRefreshToken,
        IPublicKeyId
    {
        public int Id { get; set; }
        public Guid PublicKey { get; set; } = Guid.NewGuid();
        public string Token { get; set; } = Utils.GenerateApiKey<User>(64);
        public int UserId { get; set; }
        public DateTime Created { get; set; } = DateTime.UtcNow;
        public DateTime Expires { get; set; }
        public DateTime? Revoked { get; set; }
    }
}
