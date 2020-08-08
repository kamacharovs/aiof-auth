using System;
using System.Threading.Tasks;

namespace aiof.auth.data
{
    public interface IEnvConfiguration
    {
        string PostgreSQLConString { get; }

        int JwtExpires { get; }
        int JwtRefreshExpires { get; }
        string JwtType { get; }
        string JwtIssuer { get; }
        string JwtAudience { get; }
        string JwtSecret { get; }

        int HashIterations { get; }
        int HashSaltSize { get; }
        int HashKeySize { get; }

        Task<bool> IsEnabledAsync(FeatureFlags featureFlag);
    }
}