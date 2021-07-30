using System;

namespace aiof.auth.data
{
    public interface IUserRefreshToken
    {
        int Id { get; set; }
        Guid PublicKey { get; set; }
        string Token { get; set; }
        int UserId { get; set; }
        DateTime Created { get; set; }
        DateTime Expires { get; set; }
        DateTime? Revoked { get; set; }
    }
}
