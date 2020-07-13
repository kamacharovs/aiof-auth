using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

using aiof.auth.data;

namespace aiof.auth.services
{
    public class AuthRepository : IAuthRepository
    {
        private readonly ILogger<AuthRepository> _logger;
        private readonly IEnvConfiguration _envConfig;
        private readonly AuthContext _context;

        public AuthRepository(
            ILogger<AuthRepository> logger,
            IEnvConfiguration envConfig,
            AuthContext context)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _envConfig = envConfig ?? throw new ArgumentNullException(nameof(envConfig));
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        private IQueryable<User> GetUsersQuery()
        {
            return _context.Users
                .AsNoTracking()
                .AsQueryable();
        }

        public async Task<IUser> GetUserAsync(int id)
        {
            return await GetUsersQuery()
                .FirstOrDefaultAsync(x => x.Id == id)
                ?? throw new AuthNotFoundException();
        }
        public async Task<IUser> GetUserAsync(string apiKey)
        {
            return await GetUsersQuery()
                .FirstOrDefaultAsync(x => x.ApiKey == apiKey)
                ?? throw new AuthNotFoundException();
        }

        public async Task<string> GetUserTokenAsync(int id)
        {
            var user = await GetUsersQuery()
                .FirstOrDefaultAsync(x => x.Id == id);

            return GenerateJwtToken(user);
        }

        private string GenerateJwtToken(IUser user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            // generate token that is valid for 7 days
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_envConfig.TokenSecret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[] 
                {
                    new Claim(ClaimTypes.Name, user.Id.ToString()),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.UserData, user.Username)
                }),
                Expires = DateTime.UtcNow.AddMinutes(_envConfig.TokenTtl),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
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
