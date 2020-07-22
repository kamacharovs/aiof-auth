using System;

namespace aiof.auth.data
{
    public interface IClientRefreshToken
    {
        int Id { get; set; }
        Guid PublicKey { get; set; }
        int ClientId { get; set; }
        Client Client { get; set; }
        DateTime GeneratedOn { get; set; }
        string RefreshToken { get; set; }
    }
}