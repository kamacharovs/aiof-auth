using System;
using System.Net;
using System.Linq;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Caching.Memory;

using AutoMapper;
using FluentValidation;

using aiof.auth.data;

namespace aiof.auth.services
{
    public class UserRepository : IUserRepository
    {
        private readonly ILogger<UserRepository> _logger;
        private readonly IEnvConfiguration _envConfig;
        private readonly IUtilRepository _utilRepo;
        private readonly IMapper _mapper;
        private readonly AuthContext _context;
        private readonly AbstractValidator<UserDto> _userDtoValidator;
        private readonly AbstractValidator<User> _userValidator;

        public UserRepository(
            ILogger<UserRepository> logger,
            IEnvConfiguration envConfig,
            IUtilRepository utilRepo,
            IMapper mapper,
            AuthContext context,
            AbstractValidator<UserDto> userDtoValidator,
            AbstractValidator<User> userValidator)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _envConfig = envConfig ?? throw new ArgumentNullException(nameof(envConfig));
            _utilRepo = utilRepo ?? throw new ArgumentNullException(nameof(utilRepo));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _userDtoValidator = userDtoValidator ?? throw new ArgumentNullException(nameof(userDtoValidator));
            _userValidator = userValidator ?? throw new ArgumentNullException(nameof(userValidator));
        }

        private IQueryable<User> GetQuery(bool asNoTracking = true)
        {
            return asNoTracking
                ? _context.Users
                    .Include(x => x.Role)
                    .AsNoTracking()
                    .AsQueryable()
                : _context.Users
                    .Include(x => x.Role)
                    .AsQueryable();
        }

        private IQueryable<UserRefreshToken> GetRefreshTokensQuery(bool asNoTracking = true)
        {
            return asNoTracking
                ? _context.UserRefreshTokens
                    .AsNoTracking()
                    .AsQueryable()
                : _context.UserRefreshTokens
                    .AsQueryable();
        }

        public async Task<IUser> GetAsync(
            int id,
            bool asNoTracking = true)
        {
            return await GetQuery(asNoTracking)
                .FirstOrDefaultAsync(x => x.Id == id)
                ?? throw new AuthNotFoundException($"{nameof(User)} with Id='{id}' was not found");
        }
        public async Task<IUser> GetAsync(Guid publicKey)
        {
            return await GetQuery()
                .FirstOrDefaultAsync(x => x.PublicKey == publicKey)
                ?? throw new AuthNotFoundException($"{nameof(User)} with publicKey='{publicKey}' was not found");
        }
        public async Task<IUser> GetAsync(string apiKey)
        {
            return await GetQuery()
                .FirstOrDefaultAsync(x => x.PrimaryApiKey == apiKey
                    || x.SecondaryApiKey == apiKey)
                ?? throw new AuthNotFoundException($"{nameof(User)} with ApiKey='{apiKey}' was not found");
        }
        public async Task<IUser> GetByUsernameAsync(
            string username, 
            bool asNoTracking = true)
        {
            return await GetQuery(asNoTracking)
                .FirstOrDefaultAsync(x => x.Username == username)
                ?? throw new AuthNotFoundException($"User with Username='{username}' was not found");
        }
        public async Task<IUser> GetAsync(
            string username, 
            string password)
        {
            var user = await GetByUsernameAsync(username);

            if (!Check(user.Password, password))
                throw new AuthFriendlyException(HttpStatusCode.BadRequest,
                    $"Incorrect password for User with Username='{username}'");

            return user;
        }
        public async Task<IUser> GetAsync(
            string firstName,
            string lastName,
            string email,
            string username)
        {
            return await GetQuery()
                .FirstOrDefaultAsync(
                    x => x.FirstName == firstName
                    && x.LastName == lastName
                    && x.Email == email
                    && x.Username == username);
        }
        public async Task<IUser> GetAsync(UserDto userDto)
        {
            return await GetAsync(
                userDto.FirstName,
                userDto.LastName,
                userDto.Email,
                userDto.Username);
        }

        public async Task<IUser> GetByRefreshTokenAsync(string refreshToken)
        {
            return await _context.Users
                .Include(x => x.Role)
                .Include(x => x.RefreshTokens)
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.RefreshTokens.Any(x => x.Token == refreshToken))
                ?? throw new AuthNotFoundException($"{nameof(User)} with RefreshToken='{refreshToken}' was not found");
        }
        public async Task<IUserRefreshToken> GetRefreshTokenAsync(int userId)
        {
            return await GetRefreshTokensQuery()
                .Where(x => x.UserId == userId
                    && x.Revoked == null)
                .OrderBy(x => x.Expires)
                .Take(1)
                .FirstOrDefaultAsync();
        }
        public async Task<IUserRefreshToken> GetRefreshTokenAsync(
            int userId,
            string token, 
            bool asNoTracking = true)
        {
            return await GetRefreshTokensQuery(asNoTracking)
                .FirstOrDefaultAsync(x => x.UserId == userId
                    && x.Token == token);
        }
        public async Task<IEnumerable<IUserRefreshToken>> GetRefreshTokensAsync(int userId)
        {
            return await GetRefreshTokensQuery()
                .Where(x => x.UserId == userId 
                    && x.Revoked == null)
                .OrderByDescending(x => x.Expires)
                .Take(1)
                .ToListAsync();
        }
        public async Task<IUserRefreshToken> GetOrAddRefreshTokenAsync(int userId)
        {
            return await GetRefreshTokenAsync(userId)
                ?? await AddRefreshTokenAsync(userId);
        }

        public async Task<bool> DoesUsernameExistAsync(string username)
        {
            return await GetQuery()
                .AnyAsync(x => x.Username == username);
        }

        public async Task<IUser> AddAsync(UserDto userDto)
        {
            await _userDtoValidator.ValidateAndThrowAsync(userDto);

            if (await DoesUsernameExistAsync(userDto.Username))
                throw new AuthFriendlyException(HttpStatusCode.BadRequest,
                    $"{nameof(User)} with Username='{userDto.Username}' already exists");

            var user = await GetAsync(userDto) is null
                ? _mapper.Map<User>(userDto)
                : throw new AuthFriendlyException(HttpStatusCode.BadRequest,
                    $"User with FirstName='{userDto.FirstName}', " +
                    $"LastName='{userDto.LastName}', " +
                    $"Email='{userDto.Email}' " +
                    $"and Username='{userDto.Username}' already exists");

            user.Password = Hash(userDto.Password);
            user.RoleId = await _utilRepo.GetRoleIdAsync<User>();

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Created User with UserId={UserId}, UserPublicKey={UserPublicKey}, " +
                "UserFirstName={UserFirstName}, " +
                "UserLastName={UserLastName}, " +
                "UserEmail={UserEmail} and " +
                "UserUsername={UserUsername}",
                user.Id, user.PublicKey, user.FirstName, user.LastName, user.Email, user.Username);

            await AddRefreshTokenAsync(user.Id);

            var profile = new UserProfile { UserId = user.Id };

            await _context.UserProfiles.AddAsync(profile);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Created {EntityName} Profile for UserId={UserId}", 
                nameof(User),
                user.Id);

            return user;
        }

        public async Task<IUserRefreshToken> AddRefreshTokenAsync(int userId)
        {
            var refreshToken = new UserRefreshToken
            {
                UserId = userId,
                Expires = DateTime.UtcNow.AddSeconds(_envConfig.JwtRefreshExpires)
            };

            await _context.UserRefreshTokens
                .AddAsync(refreshToken);

            await _context.SaveChangesAsync();

            _logger.LogInformation("Created UserRefreshToken for User with UserId={UserId}",
                userId);

            return refreshToken;
        }

        public async Task<IUser> UpdatePasswordAsync(
            string username, 
            string oldPassword, 
            string newPassword)
        {
            var user = await GetByUsernameAsync(username, asNoTracking: false);

            if (!Check(user.Password, oldPassword))
                throw new AuthFriendlyException(HttpStatusCode.BadRequest,
                    $"Incorrect password for User with Username='{username}'");

            user.Password = Hash(newPassword);

            await _context.SaveChangesAsync();

            _logger.LogInformation("Updated Password for User with Username='{UserUsername}'", username);

            return user;
        }

        public async Task SoftDeleteAsync(int id)
        {
            var user = await GetAsync(id, false) as User;

            user.IsDeleted = true;

            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Soft Deleted {EntityName} with Id={UserId}",
                nameof(User),
                id);
        }

        public async Task<IUserRefreshToken> RevokeTokenAsync(
            int userId, 
            string token)
        {
            var refreshToken = await GetRefreshTokenAsync(
                userId,
                token,
                asNoTracking: false)
                as UserRefreshToken
                ?? throw new AuthNotFoundException($"{nameof(UserRefreshToken)} with UserId='{userId}' and Token='{token}' was not found");

            refreshToken.Revoked = DateTime.UtcNow;

            _context.UserRefreshTokens
                .Update(refreshToken);

            await _context.SaveChangesAsync();

            _logger.LogInformation("Revoked UserRefreshToken='{UserRefreshToken}' for UserId='{UserId}'",
                token,
                userId);

            return refreshToken;
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

        public bool Check(string hash, string password)
        {
            var parts = hash.Split('.', 3);

            if (parts.Length != 3)
                throw new FormatException("Unexpected hash format. " +
                  "Should be formatted as `{iterations}.{salt}.{hash}`");

            var iterations = Convert.ToInt32(parts[0]);
            var salt = Convert.FromBase64String(parts[1]);
            var key = Convert.FromBase64String(parts[2]);

            using (var algorithm = new Rfc2898DeriveBytes(
              password,
              salt,
              iterations,
              HashAlgorithmName.SHA256))
            {
                var keyToCheck = algorithm.GetBytes(_envConfig.HashKeySize);
                var verified = keyToCheck.SequenceEqual(key);

                return verified;
            }
        }
    }
}