using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.Extensions.Configuration;
using Microsoft.FeatureManagement;

namespace aiof.auth.data
{
    public class EnvConfiguration : IEnvConfiguration
    {
        public readonly IConfiguration _config;
        public readonly IFeatureManager _featureManager;

        public EnvConfiguration(
            IConfiguration config,
            IFeatureManager featureManager)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _featureManager = featureManager;
        }

        public string PostgreSQLConnection => _config[Keys.PostgreSQLConnectionEnv]
            ?? _config.GetConnectionString(Keys.PostgreSQLConnection)
            ?? throw new KeyNotFoundException();

        public string CorsPortal => _config[Keys.CorsPortal] == null ? "http://localhost:4100" : _config[Keys.CorsPortal];
        public int MemCacheTtl => int.Parse(_config[Keys.MemCacheTtl] ?? throw new KeyNotFoundException());

        public int JwtExpires => int.Parse(_config[Keys.JwtExpires] ?? throw new KeyNotFoundException());
        public int JwtRefreshExpires => int.Parse(_config[Keys.JwtRefreshExpires] ?? throw new KeyNotFoundException());
        public string JwtType => _config[Keys.JwtType] ?? throw new KeyNotFoundException();
        public string JwtIssuer => _config[Keys.JwtIssuer] ?? throw new KeyNotFoundException();
        public string JwtAudience => _config[Keys.JwtAudience] ?? throw new KeyNotFoundException();
        public string JwtPrivateKey => _config[Keys.JwtPrivateKey] ?? throw new KeyNotFoundException();
        public string JwtPublicKey => _config[Keys.JwtPublicKey] ?? throw new KeyNotFoundException();

        public int HashIterations => int.Parse(_config[Keys.HashIterations] ?? throw new KeyNotFoundException());
        public int HashSaltSize => int.Parse(_config[Keys.HashSaltSize] ?? throw new KeyNotFoundException());
        public int HashKeySize => int.Parse(_config[Keys.HashKeySize] ?? throw new KeyNotFoundException());

        public string OpenApiVersion => _config[Keys.OpenApiVersion] ?? throw new KeyNotFoundException();
        public string OpenApiTitle => _config[Keys.OpenApiTitle] ?? throw new KeyNotFoundException();
        public string OpenApiDescription => _config[Keys.OpenApiDescription] ?? throw new KeyNotFoundException();
        public string OpenApiContactName => _config[Keys.OpenApiContactName] ?? throw new KeyNotFoundException();
        public string OpenApiContactEmail => _config[Keys.OpenApiContactEmail] ?? throw new KeyNotFoundException();
        public string OpenApiContactUrl => _config[Keys.OpenApiContactUrl] ?? throw new KeyNotFoundException();
        public string OpenApiLicenseName => _config[Keys.OpenApiLicenseName] ?? throw new KeyNotFoundException();
        public string OpenApiLicenseUrl => _config[Keys.OpenApiLicenseUrl] ?? throw new KeyNotFoundException();


        public async Task<bool> IsEnabledAsync(FeatureFlags featureFlag)
        {
            return await _featureManager.IsEnabledAsync(featureFlag.ToString());
        }
    }

    public enum AlgType
    {
        RS256,
        HS256
    }

    public enum RsaKeyType
    {
        Private,
        Public
    }

    public enum FeatureFlags
    {
        RefreshToken,
        OpenId,
        MemCache
    }
}