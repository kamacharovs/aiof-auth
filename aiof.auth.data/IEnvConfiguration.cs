using System;

namespace aiof.auth.data
{
    public interface IEnvConfiguration
    {
        int JwtExpires { get; }
        string JwtIssuer { get; }
        string JwtAudience { get; }
        string JwtSecret { get; }
    }
}