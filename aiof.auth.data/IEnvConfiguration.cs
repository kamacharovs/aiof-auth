using System;
using System.Threading.Tasks;

namespace aiof.auth.data
{
    public interface IEnvConfiguration
    {
        string DataPostgreSQL { get; }

        string CorsPortal { get; }

        int JwtExpires { get; }
        int JwtRefreshExpires { get; }
        string JwtType { get; }
        string JwtIssuer { get; }
        string JwtAudience { get; }
        string JwtPrivateKey { get; }
        string JwtPublicKey { get; }

        int HashIterations { get; }
        int HashSaltSize { get; }
        int HashKeySize { get; }

        string OpenApiTitle { get; }
        string OpenApiDescription { get; }
        string OpenApiContactName { get; }
        string OpenApiContactEmail { get; }
        string OpenApiContactUrl { get; }
        string OpenApiLicenseName { get; }
        string OpenApiLicenseUrl { get; }

        Task<bool> IsEnabledAsync(FeatureFlags featureFlag);
    }
}