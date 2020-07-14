using System;

using Microsoft.Extensions.Configuration;

namespace aiof.auth.data
{
    public class EnvConfiguration : IEnvConfiguration
    {
        public readonly IConfiguration _config;

        public EnvConfiguration(IConfiguration config)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
        }

        public int JwtExpires => int.Parse(_config[$"{Keys.Jwt}:{Keys.Expires}"]);
        public string JwtIssuer => _config[$"{Keys.Jwt}:{Keys.Issuer}"];
        public string JwtAudience => _config[$"{Keys.Jwt}:{Keys.Audience}"];
        public string JwtSecret => _config[$"{Keys.Jwt}:{Keys.Secret}"];
    }
}