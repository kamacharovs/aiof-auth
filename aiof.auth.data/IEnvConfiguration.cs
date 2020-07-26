using System;

namespace aiof.auth.data
{
    public interface IEnvConfiguration
    {
        int JwtExpires { get; }
        int JwtRefreshExpires { get; }
        string JwtType { get; }
        string JwtIssuer { get; }
        string JwtAudience { get; }
        string JwtSecret { get; }

        int HashIterations { get; }
        int HashSaltSize { get; }
        int HashKeySize { get; }
    }
}