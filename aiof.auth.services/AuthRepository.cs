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
        private readonly IMapper _mapper;
        private readonly AuthContext _context;
        private readonly AbstractValidator<UserDto> _userDtoValidator;

        public AuthRepository(
            ILogger<AuthRepository> logger,
            IEnvConfiguration envConfig,
            IMapper mapper,
            AuthContext context,
            AbstractValidator<UserDto> userDtoValidator)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _envConfig = envConfig ?? throw new ArgumentNullException(nameof(envConfig));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _userDtoValidator = userDtoValidator ?? throw new ArgumentNullException(nameof(userDtoValidator));
        }

        private IQueryable<User> GetUsersQuery()
        {
            return _context.Users
                .AsNoTracking()
                .AsQueryable();
        }

        private IQueryable<T> GetEntityQuery<T>()
            where T : class, IPublicKeyId
        {
            return _context.Set<T>()
                .AsNoTracking()
                .AsQueryable();
        }

        public async Task<IUser> GetUserAsync(int id)
        {
            return await GetUsersQuery()
                .FirstOrDefaultAsync(x => x.Id == id)
                ?? throw new AuthNotFoundException();
        }
        public async Task<IUser> GetUserAsync(Guid publicKey)
        {
            return await GetUsersQuery()
                .FirstOrDefaultAsync(x => x.PublicKey == publicKey)
                ?? throw new AuthNotFoundException();
        }
        public async Task<IUser> GetUserAsync(string apiKey)
        {
            return await GetUsersQuery()
                .FirstOrDefaultAsync(x => x.PrimaryApiKey == apiKey || x.SecondaryApiKey == apiKey)
                ?? throw new AuthNotFoundException();
        }
        public async Task<IUser> GetUserAsync(int id, string apiKey)
        {
            return await GetUsersQuery()
                .FirstOrDefaultAsync(x => x.Id == id
                    && x.PrimaryApiKey == apiKey || x.SecondaryApiKey == apiKey)
                ?? throw new AuthNotFoundException();
        }
        public async Task<IPublicKeyId> GetUserAsPublicKeyId(string apiKey)
        {
            return await GetUsersQuery()
                .FirstOrDefaultAsync(x => x.PrimaryApiKey == apiKey || x.SecondaryApiKey == apiKey)
                ?? throw new AuthNotFoundException();
        }

        public async Task<IPublicKeyId> GetEntityAsync<T>(int id)
            where T : class, IPublicKeyId
        {
            return await GetEntityQuery<T>()
                .FirstOrDefaultAsync(x => x.Id == id)
                ?? throw new AuthNotFoundException();
        }

        public async Task<ITokenResponse> GetUserTokenAsync(string apiKey)
        {
            var user = await GetUserAsync(apiKey);

            return GenerateJwtToken(user);
        }

        public async Task<IUser> AddUserAsync(UserDto userDto)
        {
            var validation = _userDtoValidator.Validate(userDto);

            if (!validation.IsValid)
                throw new AuthFriendlyException(HttpStatusCode.BadRequest,
                    validation.Errors);

            var user = _mapper.Map<User>(userDto);

            user.PublicKey = Guid.NewGuid();
            user.PrimaryApiKey = GenerateApiKey();
            user.SecondaryApiKey = GenerateApiKey();

            await _context.Users
                .AddAsync(user);

            await _context.SaveChangesAsync();

            _logger.LogInformation($"Created User with Id='{user.Id}' and PublicKey='{user.PublicKey}'");

            return user;
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

        public string GenerateApiKey()
        {
            var key = new byte[32];
            using (var generator = RandomNumberGenerator.Create())
                generator.GetBytes(key);
            return Convert.ToBase64String(key);
        }
    }
}
