using System;
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

        public string DatabaseConString => _config.GetConnectionString(Keys.Database);

        public int JwtExpires => int.Parse(_config[$"{Keys.Jwt}:{Keys.Expires}"]);
        public int JwtRefreshExpires => int.Parse(_config[$"{Keys.Jwt}:{Keys.RefreshExpires}"]);
        public string JwtType => _config[$"{Keys.Jwt}:{Keys.Type}"];
        public string JwtIssuer => _config[$"{Keys.Jwt}:{Keys.Issuer}"];
        public string JwtAudience => _config[$"{Keys.Jwt}:{Keys.Audience}"];
        public string JwtSecret => _config[$"{Keys.Jwt}:{Keys.Secret}"];

        public int HashIterations => int.Parse(_config[$"{Keys.Hash}:{Keys.Iterations}"]);
        public int HashSaltSize => int.Parse(_config[$"{Keys.Hash}:{Keys.SaltSize}"]);
        public int HashKeySize => int.Parse(_config[$"{Keys.Hash}:{Keys.KeySize}"]);

        public async Task<bool> IsEnabledAsync(FeatureFlags featureFlag)
        {
            return await _featureManager.IsEnabledAsync(nameof(featureFlag));
        }
    }

    public enum FeatureFlags
    {
        RefreshToken
    }
}