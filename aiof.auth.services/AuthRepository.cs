using System;

using Microsoft.Extensions.Logging;

namespace aiof.auth.services
{
    public class AuthRepository : IAuthRepository
    {
        private readonly ILogger<AuthRepository> _logger;

        public AuthRepository(ILogger<AuthRepository> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
    }
}
