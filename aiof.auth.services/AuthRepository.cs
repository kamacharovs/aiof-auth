using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using System.Security.Claims;
using System.Text;
using System.Collections.Generic;

using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

using aiof.auth.data;

namespace aiof.auth.services
{
    public class AuthRepository : IAuthRepository
    {
        private readonly ILogger<AuthRepository> _logger;
        private readonly IEnvConfiguration _envConfig;

        private readonly string _key;
        private readonly string _issuer;
        private readonly string _audience;


        public AuthRepository(
            ILogger<AuthRepository> logger,
            IEnvConfiguration envConfig)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _envConfig = envConfig ?? throw new ArgumentNullException(nameof(envConfig));

            _key = _envConfig.JwtSecret ?? throw new ArgumentNullException(nameof(_envConfig.JwtSecret));
            _issuer = _envConfig.JwtIssuer ?? throw new ArgumentNullException(nameof(_envConfig.JwtIssuer));
            _audience = _envConfig.JwtAudience ?? throw new ArgumentNullException(nameof(_envConfig.JwtAudience));
        }

        public ITokenResponse GenerateJwtToken(IUser user)
        {
            return GenerateJwtToken(new Claim[]
            {
                new Claim(AiofClaims.PublicKey, user.PublicKey.ToString()),
                new Claim(AiofClaims.GivenName, user.FirstName),
                new Claim(AiofClaims.FamilyName, user.LastName),
                new Claim(AiofClaims.Email, user.Email)
            },
            user as IPublicKeyId);
        }

        public ITokenResponse GenerateJwtToken(IClient client)
        {
            return GenerateJwtToken(new Claim[]
            {
                new Claim(AiofClaims.PublicKey, client.PublicKey.ToString()),
                new Claim(AiofClaims.Name, client.Name),
                new Claim(AiofClaims.Slug, client.Slug)
            },
            client as IPublicKeyId);
        }

        public ITokenResponse GenerateJwtToken(IEnumerable<Claim> claims, IPublicKeyId entity = null)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_key);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddSeconds(_envConfig.JwtExpires),
                Issuer = _issuer,
                Audience = _audience,
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);

            if (entity != null)
                _logger.LogInformation($"Created JWT for {entity.GetType().Name} with Id='{entity.Id}' and PublicKey='{entity.PublicKey}'");

            return new TokenResponse
            {
                ExpiresIn = _envConfig.JwtExpires,
                AccessToken = tokenHandler.WriteToken(token)
            };
        }

        public ITokenResult ValidateToken(string token)
        {
            try
            {
                var key = Encoding.ASCII.GetBytes(_key);
                var tokenParams = new TokenValidationParameters
                {
                    RequireSignedTokens = true,
                    ValidAudience = _audience,
                    ValidateAudience = true,
                    ValidIssuer = _issuer,
                    ValidateIssuer = true,
                    ValidateIssuerSigningKey = true,
                    ValidateLifetime = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key)
                };

                var handler = new JwtSecurityTokenHandler();
                var result = handler.ValidateToken(token, tokenParams, out var securityToken);

                return new TokenResult
                {
                    Principal = result,
                    Status = TokenResultStatus.Valid
                };
            }
            catch (SecurityTokenExpiredException)
            {
                return new TokenResult
                {
                    Status = TokenResultStatus.Expired
                };
            }
        }
        public bool IsAuthenticated(string token)
        {
            return ValidateToken(token)
                .Principal
                .Identity
                .IsAuthenticated;
        }

        public string GenerateApiKey()
        {
            var key = new byte[32];
            using (var generator = RandomNumberGenerator.Create())
                generator.GetBytes(key);
            return Convert.ToBase64String(key);
        }
    }
}
