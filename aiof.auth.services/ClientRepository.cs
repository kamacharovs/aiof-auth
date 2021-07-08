using System;
using System.Net;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using AutoMapper;
using FluentValidation;

using aiof.auth.data;

namespace aiof.auth.services
{
    public class ClientRepository : BaseRepository, IClientRepository
    {
        private readonly ILogger<ClientRepository> _logger;
        private readonly IEnvConfiguration _envConfig;
        private readonly IUtilRepository _utilRepo;
        private readonly IMapper _mapper;
        private readonly AuthContext _context;
        private readonly AbstractValidator<ClientDto> _clientDtoValidator;

        public ClientRepository(
            ILogger<ClientRepository> logger,
            IEnvConfiguration envConfig,
            IUtilRepository utilRepo,
            IMapper mapper,
            AuthContext context,
            AbstractValidator<ClientDto> clientDtoValidator)
            : base(logger, envConfig, context)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _envConfig = envConfig ?? throw new ArgumentNullException(nameof(envConfig));
            _utilRepo = utilRepo ?? throw new ArgumentNullException(nameof(utilRepo));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
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

        private IQueryable<ClientRefreshToken> GetRefreshTokensQuery(bool asNoTracking = true)
        {
            return asNoTracking
                ? _context.ClientRefreshTokens
                    .AsNoTracking()
                    .AsQueryable()
                : _context.ClientRefreshTokens
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

        public async Task<IClient> GetByRefreshTokenAsync(
            string token,
            bool asNoTracking = true)
        {
            return await GetClientQuery(asNoTracking)
                .Include(x => x.RefreshTokens)
                .FirstOrDefaultAsync(x => x.RefreshTokens.Any(x => x.Token == token))
                ?? throw new AuthNotFoundException($"RefreshToken='{token}' was not found");
        }
        public async Task<IClientRefreshToken> GetRefreshTokenAsync(
            int clientId, 
            string token, 
            bool asNoTracking = true)
        {
            return await GetRefreshTokensQuery(asNoTracking)
                .FirstOrDefaultAsync(x => x.ClientId == clientId
                    && x.Token == token);
        }
        public async Task<IClientRefreshToken> GetRefreshTokenAsync(
            int clientId,  
            bool asNoTracking = true)
        {
            return await GetRefreshTokensQuery(asNoTracking)
                .FirstOrDefaultAsync(x => x.ClientId == clientId
                    && DateTime.UtcNow < x.Expires);
        }

        public async Task<IEnumerable<IClientRefreshToken>> GetRefreshTokensAsync(int clientId)
        {
            return await GetRefreshTokensQuery()
                .Where(x => x.ClientId == clientId)
                .ToListAsync();
        }

        public async Task<IClientRefreshToken> GetOrAddRefreshTokenAsync(IClient client)
        {
            var clientRefreshToken = await GetRefreshTokenAsync(client.Id)
                ?? await AddRefreshTokenAsync(client.Id);

            async Task<IClientRefreshToken> AddRefreshTokenAsync(int clientId)
            {
                var clientRefreshToken = new ClientRefreshToken
                {
                    ClientId = clientId,
                    Expires = DateTime.UtcNow.AddSeconds(_envConfig.JwtRefreshExpires)
                };

                await _context.ClientRefreshTokens.AddAsync(clientRefreshToken);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Added {EntityName} for ClientId={ClientId}",
                    nameof(ClientRefreshToken),
                    clientId);

                return clientRefreshToken;
            }

            return clientRefreshToken;
        }

        public async Task<IClient> AddClientAsync(ClientDto clientDto)
        {
            await _clientDtoValidator.ValidateAndThrowAsync(clientDto);

            var client = _mapper.Map<Client>(clientDto)
                .GenerateApiKeys();

            client.Role = await _utilRepo.GetRoleAsync<Client>(clientDto.RoleId) as Role;

            await _context.Clients.AddAsync(client);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Created {EntityName} with Id={ClientId} and PublicKey={ClientPublicKey}",
                nameof(Client),
                client.Id,
                client.PublicKey);

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

        public async Task<IClientRefreshToken> RevokeTokenAsync(int clientId, string token)
        {
            var clientRefreshToken = await GetRefreshTokenAsync(
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

            _logger.LogInformation("Revoked {EntityName}={ClientRefreshToken} for CliendId={ClientId}",
                nameof(ClientRefreshToken),
                token,
                clientId);

            return clientRefreshToken;
        }

        public async Task<IClient> EnableAsync(int id)
        {
            return await EnableDisableClientAsync(id);
        }
        public async Task<IClient> DisableAsync(int id)
        {
            return await EnableDisableClientAsync(id, false);
        }

        private async Task<IClient> EnableDisableClientAsync(
            int id,
            bool enable = true)
        {
            var client = await GetAsync(id, asNoTracking: false);

            client.Enabled = enable;

            await _context.SaveChangesAsync();

            return client;
        }
        
        public async Task<IClient> RegenerateKeysAsync(int id)
        {
            return await base.RegenerateKeysAync<Client>(id);
        }
    }
}