using System;

namespace aiof.auth.data
{
    public interface IClientRefreshToken
    {
        int Id { get; set; }
        Guid PublicKey { get; set; }
        string Token { get; set; }
        int ClientId { get; set; }
        Client Client { get; set; }
        DateTime Created { get; set; }
        DateTime Expires { get; set; }
        bool IsExpired { get; set; }
        DateTime? Revoked { get; set; }
        bool IsActive { get; set; }
    }
}