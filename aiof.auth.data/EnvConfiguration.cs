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

        public int TokenTtl => int.Parse(_config[$"{Keys.Token}:{Keys.Ttl}"]);
        public string TokenSecret => _config[$"{Keys.Token}:{Keys.Secret}"];
    }
}