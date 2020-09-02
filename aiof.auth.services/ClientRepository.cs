using System;
using System.Net;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Caching.Memory;

using AutoMapper;
using FluentValidation;

using aiof.auth.data;

namespace aiof.auth.services
{
    public class ClientRepository : BaseRepository, IClientRepository
    {
        private readonly ILogger<ClientRepository> _logger;
        private readonly IMapper _mapper;
        private readonly IEnvConfiguration _envConfig;
        private readonly AuthContext _context;
        private readonly AbstractValidator<ClientDto> _clientDtoValidator;

        public ClientRepository(
            ILogger<ClientRepository> logger,
            IMemoryCache cache,
            IMapper mapper,
            IEnvConfiguration envConfig,
            AuthContext context,
            AbstractValidator<ClientDto> clientDtoValidator)
            : base(logger, cache, envConfig, context)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _envConfig = envConfig ?? throw new ArgumentNullException(nameof(envConfig));
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _clientDtoValidator = clientDtoValidator ?? throw new ArgumentNullException(nameof(clientDtoValidator));
        }

        private IQueryable<Client> GetClientQuery(bool asNoTracking = true)
        {
            return asNoTracking
                ? _context.Clients
                    .Include(x => x.Role)
                    .AsNoTracking()
                    .AsQueryable()
                : _context.Clients
                    .Include(x => x.Role)
                    .AsQueryable();
        }

        private IQueryable<ClientRefreshToken> GetClientRefreshTokenQuery(bool asNoTracking = true)
        {
            return asNoTracking
                ? _context.ClientRefreshTokens
                    .Include(x => x.Client)
                        .ThenInclude(x => x.Role)
                    .AsNoTracking()
                    .AsQueryable()
                : _context.ClientRefreshTokens
                    .Include(x => x.Client)
                        .ThenInclude(x => x.Role)
                    .AsQueryable();
        }

        public async Task<IClient> GetAsync(
            int id,
            bool asNoTracking = true)
        {
            return await GetClientQuery(asNoTracking)
                .FirstOrDefaultAsync(x => x.Id == id)
                ?? throw new AuthNotFoundException($"{nameof(Client)} with Id='{id}' was not found");
        }

        public async Task<IClient> GetAsync(
            string apiKey,
            bool asNoTracking = true)
        {
            return await GetClientQuery(asNoTracking)
                .FirstOrDefaultAsync(x => x.PrimaryApiKey == apiKey
                    || x.SecondaryApiKey == apiKey)
                ?? throw new AuthNotFoundException($"{nameof(Client)} with ApiKey='{apiKey}' was not found");
        }

        public async Task<IClientRefreshToken> GetRefreshTokenAsync(
            string token,
            bool asNoTracking = true)
        {
            return await GetClientRefreshTokenQuery(asNoTracking)
                .FirstOrDefaultAsync(x => x.Token == token
                    && x.Client.Enabled)
                ?? throw new AuthNotFoundException($"RefreshToken='{token}' was not found");
        }
        public async Task<IClientRefreshToken> GetClientRefreshTokenAsync(
            int clientId, 
            string refreshToken, 
            bool asNoTracking = true)
        {
            return await GetClientRefreshTokenQuery(asNoTracking)
                .FirstOrDefaultAsync(x => x.ClientId == clientId
                    && x.Client.Enabled
                    && x.Token == refreshToken);
        }
        public async Task<IClientRefreshToken> GetClientRefreshTokenAsync(
            int clientId,  
            bool asNoTracking = true)
        {
            return await GetClientRefreshTokenQuery(asNoTracking)
                .FirstOrDefaultAsync(x => x.ClientId == clientId
                    && x.Client.Enabled
                    && DateTime.UtcNow < x.Expires);
        }

        public async Task<IEnumerable<IClientRefreshToken>> GetRefreshTokensAsync(int clientId)
        {
            return await GetClientRefreshTokenQuery()
                .Where(x => x.ClientId == clientId)
                .ToListAsync();
        }

        public async Task<IClient> AddClientAsync(ClientDto clientDto)
        {
            await _clientDtoValidator.ValidateAndThrowAsync(clientDto);

            var client = _mapper.Map<Client>(clientDto)
                .GenerateApiKeys();

            await _context.Clients
                .AddAsync(client);

            await _context.SaveChangesAsync();

            _logger.LogInformation($"Created Client with Id='{client.Id}' and PublicKey='{client.PublicKey}'");

            return client;
        }
        public async IAsyncEnumerable<IClient> AddClientsAsync(IEnumerable<ClientDto> clientDtos)
        {
            if (clientDtos.Count() > 15)
                throw new AuthFriendlyException(HttpStatusCode.BadRequest,
                    $"Cannot add more than 15 Clients at a time");
                    
            foreach (var clientDto in clientDtos)
                yield return await AddClientAsync(clientDto);
        }

        public async Task<IClientRefreshToken> AddClientRefreshTokenAsync(string clientApiKey)
        {
            var client = await GetAsync(clientApiKey);

            if (!client.Enabled)
                throw new AuthFriendlyException(HttpStatusCode.BadRequest,
                    $"The current Client with ApiKey='{clientApiKey}' is DISABLED");

            var clientRefreshToken = await GetClientRefreshTokenAsync(client.Id)
                ?? await AddClientRefreshTokenAsync(client.Id);

            async Task<IClientRefreshToken> AddClientRefreshTokenAsync(int clientId)
            {
                var clientRefreshToken = new ClientRefreshToken
                {
                    ClientId = clientId,
                    Expires = DateTime.UtcNow.AddSeconds(_envConfig.JwtRefreshExpires)
                };

                await _context.ClientRefreshTokens
                    .AddAsync(clientRefreshToken);

                await _context.SaveChangesAsync();

                await _context.Entry(clientRefreshToken)
                    .Reference(x => x.Client)
                    .LoadAsync();

                _logger.LogInformation($"Added Client refresh token for ClientId='{clientId}' and PublicKey='{clientRefreshToken.Client.PublicKey}'");

                return clientRefreshToken;
            }

            return clientRefreshToken;
        }

        public async Task<IClientRefreshToken> RevokeTokenAsync(int clientId, string token)
        {
            var clientRefreshToken = await GetClientRefreshTokenAsync(
                clientId, 
                token,
                asNoTracking: false)
                as ClientRefreshToken
                ?? throw new AuthNotFoundException();

            clientRefreshToken.Revoked = DateTime.UtcNow;
            clientRefreshToken.Expires = DateTime.UtcNow;

            _context.ClientRefreshTokens
                .Update(clientRefreshToken);

            await _context.SaveChangesAsync();

            _logger.LogInformation($"Revoked token='{token}' for cliendId='{clientId}'");

            return clientRefreshToken;
        }

        public async Task<IClient> SoftDeleteAsync(int id)
        {
            return await base.SoftDeleteAsync<Client>(id);
        }

        public async Task<IClient> RegenerateKeysAsync(int id)
        {
            return await base.RegenerateKeysAync<Client>(id);
        }

        public async Task<IClient> EnableDisableClientAsync(
            int id,
            bool enable = true)
        {
            var client = await GetAsync(id, asNoTracking: false);

            client.Enabled = enable;

            await _context.SaveChangesAsync();

            return client;
        }
    }
}