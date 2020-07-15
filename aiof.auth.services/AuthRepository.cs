using System;
using System.Collections.Generic;
using System.Net;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

using AutoMapper;
using FluentValidation;

using aiof.auth.data;

namespace aiof.auth.services
{
    public class AuthRepository : IAuthRepository
    {
        private readonly ILogger<AuthRepository> _logger;
        private readonly IEnvConfiguration _envConfig;
        private readonly IUserRepository _userRepo;

        public AuthRepository(
            ILogger<AuthRepository> logger,
            IEnvConfiguration envConfig,
            IUserRepository userRepo)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _envConfig = envConfig ?? throw new ArgumentNullException(nameof(envConfig));
            _userRepo = userRepo ?? throw new ArgumentNullException(nameof(userRepo));
        }

        public async Task<ITokenResponse> GetUserTokenAsync(string apiKey)
        {
            var user = await _userRepo.GetUserAsync(apiKey);

            return GenerateJwtToken(user);
        }

        public ITokenResponse GenerateJwtToken(IUser user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_envConfig.JwtSecret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(AiofClaims.PublicKey, user.PublicKey.ToString()),
                    new Claim(AiofClaims.GivenName, user.FirstName),
                    new Claim(AiofClaims.FamilyName, user.LastName),
                    new Claim(AiofClaims.Email, user.Email)
                }),
                Expires = DateTime.UtcNow.AddSeconds(_envConfig.JwtExpires),
                Issuer = _envConfig.JwtIssuer,
                Audience = _envConfig.JwtAudience,
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return new TokenResponse
            {
                ExpiresIn = _envConfig.JwtExpires,
                AccessToken = tokenHandler.WriteToken(token)
            };
        }
    }
}
