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
        private readonly IEnvConfiguration _envConfig;
        private readonly AuthContext _context;

        public BaseRepository(
            ILogger logger,
            IEnvConfiguration envConfig,
            AuthContext context)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _envConfig = envConfig ?? throw new ArgumentNullException(nameof(envConfig));
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

        public async Task<T> GetEntityAsync<T>(
            int id, 
            bool asNoTracking = true)
            where T : class, IPublicKeyId, IApiKey
        {
            return await GetEntityQuery<T>(asNoTracking)
                .FirstOrDefaultAsync(x => x.Id == id)
                ?? throw new AuthNotFoundException($"{typeof(T).Name} with Id='{id}' was not found");
        }

        public async Task<T> SoftDeleteAsync<T>(int id)
            where T : class, IPublicKeyId, IApiKey, IEnable
        {
            var entity = await GetEntityAsync<T>(
                id, 
                asNoTracking: false);

            entity.Enabled = false;

            _context.Set<T>()
                .Update(entity);

            await _context.SaveChangesAsync();

            _logger.LogInformation("Soft Deleted {EntityName} with Id={EntityId} and PublicKey={EntityPublicKey}",
                typeof(T).Name,
                entity.Id,
                entity.PublicKey);

            return entity;
        }

        public async Task DeleteAsync<T>(int id)
            where T : class, IPublicKeyId, IApiKey
        {
            var entity = await GetEntityAsync<T>(id, asNoTracking: false);
            var publicKey = entity.PublicKey;

            var entityJson = JsonSerializer.Serialize(entity);

            _context.Set<T>()
                .Remove(entity);

            await _context.SaveChangesAsync();

            _logger.LogInformation("Deleted {EntityName}. {EntityName}Json='{EntityJson}'",
                typeof(T).Name,
                typeof(T).Name,
                entityJson);
        }

        public async Task<T> RegenerateKeysAync<T>(int id)
            where T : class, IPublicKeyId, IApiKey
        {
            var entity = await GetEntityAsync<T>(id, asNoTracking: false);

            entity.PrimaryApiKey = Utils.GenerateApiKey<T>();
            entity.SecondaryApiKey = Utils.GenerateApiKey<T>();

            await _context.SaveChangesAsync();

            _logger.LogInformation("Regenerated Keys for {EntityName} with Id={id} and PublicKey={EntityPublicKey}",
                typeof(T).Name,
                id,
                entity.PublicKey);

            return entity;
        }
    }
}