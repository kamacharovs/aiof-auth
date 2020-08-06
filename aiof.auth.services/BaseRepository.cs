using System;
using System.Net;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using aiof.auth.data;

namespace aiof.auth.services
{
    public abstract class BaseRepository
    {
        private readonly ILogger _logger;
        private readonly AuthContext _context;

        public BaseRepository(
            ILogger logger,
            AuthContext context)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public IQueryable<T> GetEntityQuery<T>(bool asNoTracking = true)
            where T : class, IPublicKeyId, IApiKey
        {
            return asNoTracking
                ? _context.Set<T>()
                    .AsNoTracking()
                    .AsQueryable()
                : _context.Set<T>()
                    .AsQueryable();
        }
        public IQueryable<T> GetEntityPublicKeyIdQuery<T>(bool asNoTracking = true)
            where T : class, IPublicKeyId
        {
            return asNoTracking
                ? _context.Set<T>()
                    .AsNoTracking()
                    .AsQueryable()
                : _context.Set<T>()
                    .AsQueryable();
        }
        public IQueryable<T> GetEntityApiKeyQuery<T>(bool asNoTracking = true)
            where T : class, IApiKey
        {
            return asNoTracking
                ? _context.Set<T>()
                    .AsNoTracking()
                    .AsQueryable()
                : _context.Set<T>()
                    .AsQueryable();
        }

        public async Task<T> GetEntityPublicKeyAsync<T>(int id, bool asNoTracking = true)
            where T : class, IPublicKeyId
        {
            return await GetEntityPublicKeyIdQuery<T>(asNoTracking)
                .FirstOrDefaultAsync(x => x.Id == id)
                ?? throw new AuthNotFoundException($"{typeof(T).Name} with Id='{id}' was not found");
        }

        public async Task<T> GetEntityAsync<T>(int id, bool asNoTracking = true)
            where T : class, IPublicKeyId, IApiKey
        {
            return await GetEntityQuery<T>(asNoTracking)
                .FirstOrDefaultAsync(x => x.Id == id)
                ?? throw new AuthNotFoundException($"{typeof(T).Name} with Id='{id}' was not found");
        }

        public async Task<T> GetEntityAsync<T>(Guid publicKey)
            where T : class, IPublicKeyId
        {
            return await GetEntityPublicKeyIdQuery<T>()
                .FirstOrDefaultAsync(x => x.PublicKey == publicKey)
                ?? throw new AuthNotFoundException($"{typeof(T).Name} with PublicId='{publicKey}' was not found");
        }

        public async Task<T> GetEntityAsync<T>(string apiKey)
            where T : class, IApiKey
        {
            return await GetEntityApiKeyQuery<T>()
                .FirstOrDefaultAsync(x => x.PrimaryApiKey == apiKey
                    || x.SecondaryApiKey == apiKey)
                ?? throw new AuthNotFoundException($"{typeof(T).Name} with ApiKey='{apiKey}' was not found");
        }

        public async Task DeleteEntityAsync<T>(int id)
            where T : class, IPublicKeyId
        {
            var entity = await GetEntityPublicKeyAsync<T>(id, asNoTracking: false);
            var publicKey = entity.PublicKey;

            var entityJson = JsonSerializer.Serialize(entity);

            _context.Set<T>()
                .Remove(entity);

            await _context.SaveChangesAsync();

            _logger.LogInformation($"Deleted {typeof(T).Name}. {typeof(T).Name}Json='{entityJson}'");
        }

        public async Task<T> RegenerateKeysAync<T>(int id)
            where T : class, IPublicKeyId, IApiKey
        {
            var entity = await GetEntityAsync<T>(id, asNoTracking: false);

            entity.PrimaryApiKey = Utils.GenerateApiKey();
            entity.SecondaryApiKey = Utils.GenerateApiKey();

            await _context.SaveChangesAsync();

            _logger.LogInformation($"Regenerated Keys for {typeof(T).Name} with Id='{id}' and PublicKey='{entity.PublicKey}'");

            return entity;
        }
    }
}