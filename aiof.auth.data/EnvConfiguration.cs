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
            _featureManager = featureManager ?? throw new ArgumentNullException(nameof(featureManager));
        }

        public int MemCacheTtl => int.Parse(_config[$"{Keys.MemCache}:{Keys.Ttl}"] ?? throw new KeyNotFoundException());

        public string PostgreSQLConString => _config[Keys.PostgreSQL];

        public int JwtExpires => int.Parse(_config[$"{Keys.Jwt}:{Keys.Expires}"] ?? throw new KeyNotFoundException());
        public int JwtRefreshExpires => int.Parse(_config[$"{Keys.Jwt}:{Keys.RefreshExpires}"] ?? throw new KeyNotFoundException());
        public string JwtType => _config[$"{Keys.Jwt}:{Keys.Type}"] ?? throw new KeyNotFoundException();
        public string JwtIssuer => _config[$"{Keys.Jwt}:{Keys.Issuer}"] ?? throw new KeyNotFoundException();
        public string JwtAudience => _config[$"{Keys.Jwt}:{Keys.Audience}"] ?? throw new KeyNotFoundException();
        public string JwtSecret => _config[$"{Keys.Jwt}:{Keys.Secret}"] ?? throw new KeyNotFoundException();

        public int HashIterations => int.Parse(_config[$"{Keys.Hash}:{Keys.Iterations}"] ?? throw new KeyNotFoundException());
        public int HashSaltSize => int.Parse(_config[$"{Keys.Hash}:{Keys.SaltSize}"] ?? throw new KeyNotFoundException());
        public int HashKeySize => int.Parse(_config[$"{Keys.Hash}:{Keys.KeySize}"] ?? throw new KeyNotFoundException());

        public async Task<bool> IsEnabledAsync(FeatureFlags featureFlag)
        {
            return await _featureManager.IsEnabledAsync(featureFlag.ToString());
        }
    }

    public enum FeatureFlags
    {
        RefreshToken,
        OpenId,
        MemCache
    }
}