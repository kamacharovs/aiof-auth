using System;
using System.Net;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using aiof.auth.data;

namespace aiof.auth.services
{
    public abstract class BaseRepository<T>
        where T : class, IPublicKeyId, IApiKey
    {
        private readonly ILogger _logger;
        private readonly IAuthRepository _repo;
        private readonly AuthContext _context;

        public BaseRepository(
            ILogger logger,
            IAuthRepository repo,
            AuthContext context)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _repo = repo ?? throw new ArgumentNullException(nameof(repo));
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public IQueryable<T> GetEntityQuery(bool asNoTracking = true)
        {
            return asNoTracking
                ? _context.Set<T>()
                    .AsNoTracking()
                    .AsQueryable()
                : _context.Set<T>()
                    .AsQueryable();
        }

        public async Task<T> GetEntityAsync(int id, bool asNoTracking = true)
        {
            return await GetEntityQuery(asNoTracking)
                .FirstOrDefaultAsync(x => x.Id == id)
                ?? throw new AuthNotFoundException();
        }

        public async Task<T> GetEntityAsync(Guid publicKey)
        {
            return await GetEntityQuery()
                .FirstOrDefaultAsync(x => x.PublicKey == publicKey)
                ?? throw new AuthNotFoundException();
        }

        public async Task<T> GetEntityAsync(string apiKey)
        {
            return await GetEntityQuery()
                .FirstOrDefaultAsync(x => x.PrimaryApiKey == apiKey
                    || x.SecondaryApiKey == apiKey)
                ?? throw new AuthNotFoundException();
        }

        public async Task<T> RegenerateKeysAync(int id)
        {
            var entity = await GetEntityAsync(id, asNoTracking: false);

            entity.PrimaryApiKey = _repo.GenerateApiKey();
            entity.SecondaryApiKey = _repo.GenerateApiKey();

            await _context.SaveChangesAsync();

            _logger.LogInformation($"Regenerated Keys for {typeof(T).Name} with Id='{id}' and PublicKey='{entity.PublicKey}'");

            return entity;
        }
    }
}