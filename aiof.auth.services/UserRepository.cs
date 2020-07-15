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
    public class UserRepository : IUserRepository
    {
        private readonly ILogger<AuthRepository> _logger;
        private readonly IEnvConfiguration _envConfig;
        private readonly IMapper _mapper;
        private readonly AuthContext _context;
        private readonly AbstractValidator<UserDto> _userDtoValidator;
        private readonly AbstractValidator<User> _userValidator;

        public UserRepository(
            ILogger<AuthRepository> logger,
            IEnvConfiguration envConfig,
            IMapper mapper,
            AuthContext context,
            AbstractValidator<UserDto> userDtoValidator,
            AbstractValidator<User> userValidator)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _envConfig = envConfig ?? throw new ArgumentNullException(nameof(envConfig));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _userDtoValidator = userDtoValidator ?? throw new ArgumentNullException(nameof(userDtoValidator));
            _userValidator = userValidator ?? throw new ArgumentNullException(nameof(userValidator));
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

        public async Task<IUser> AddUserAsync(UserDto userDto)
        {
            var validation = _userDtoValidator.Validate(userDto);

            if (!validation.IsValid)
                throw new AuthValidationException(validation.Errors);

            var user = _mapper.Map<User>(userDto);

            user.PrimaryApiKey = GenerateApiKey();
            user.SecondaryApiKey = GenerateApiKey();
            user.Password = Hash(userDto.Password);

            await _context.Users
                .AddAsync(user);

            await _context.SaveChangesAsync();

            _logger.LogInformation($"Created User with Id='{user.Id}' and PublicKey='{user.PublicKey}'");

            return user;
        }

        public string GenerateApiKey()
        {
            var key = new byte[32];
            using (var generator = RandomNumberGenerator.Create())
                generator.GetBytes(key);
            return Convert.ToBase64String(key);
        }

        public string Hash(string password)
        {
            using (var algorithm = new Rfc2898DeriveBytes(
                password,
                _envConfig.HashSaltSize,
                _envConfig.HashIterations,
                HashAlgorithmName.SHA256))
            {
                var key = Convert.ToBase64String(algorithm.GetBytes(_envConfig.HashKeySize));
                var salt = Convert.ToBase64String(algorithm.Salt);

                return $"{_envConfig.HashIterations}.{salt}.{key}";
            }
        }

        public (bool Verified, bool NeedsUpgrade) Check(string hash, string password)
        {
            var parts = hash.Split('.', 3);

            if (parts.Length != 3)
                throw new FormatException("Unexpected hash format. " +
                  "Should be formatted as `{iterations}.{salt}.{hash}`");

            var iterations = Convert.ToInt32(parts[0]);
            var salt = Convert.FromBase64String(parts[1]);
            var key = Convert.FromBase64String(parts[2]);

            var needsUpgrade = iterations != _envConfig.HashIterations;

            using (var algorithm = new Rfc2898DeriveBytes(
              password,
              salt,
              iterations,
              HashAlgorithmName.SHA256))
            {
                var keyToCheck = algorithm.GetBytes(_envConfig.HashKeySize);

                var verified = keyToCheck.SequenceEqual(key);

                return (verified, needsUpgrade);
            }
        }
    }
}