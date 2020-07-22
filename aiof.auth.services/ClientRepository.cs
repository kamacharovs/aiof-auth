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
        private readonly IMapper _mapper;
        private readonly AuthContext _context;
        private readonly AbstractValidator<ClientDto> _clientDtoValidator;

        public ClientRepository(
            ILogger<ClientRepository> logger,
            IMapper mapper,
            AuthContext context,
            AbstractValidator<ClientDto> clientDtoValidator)
            : base(logger, context)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _clientDtoValidator = clientDtoValidator ?? throw new ArgumentNullException(nameof(clientDtoValidator));
        }

        private IQueryable<ClientRefreshToken> GetClientRefreshTokenQuery(bool asNoTracking = true)
        {
            return asNoTracking
                ? _context.ClientRefreshTokens
                    .Include(x => x.Client)
                    .AsNoTracking()
                    .AsQueryable()
                : _context.ClientRefreshTokens
                    .Include(x => x.Client)
                    .AsQueryable();
        }

        public async Task<IClient> GetClientAsync(int id)
        {
            return await base.GetEntityAsync<Client>(id);
        }

        public async Task<IClient> GetClientAsync(string apiKey)
        {
            return await base.GetEntityAsync<Client>(apiKey);
        }

        public async Task<IClientRefreshToken> GetClientRefreshTokenAsync(
            int clientId, 
            string refreshToken, 
            bool asNoTracking = true)
        {
            return await GetClientRefreshTokenQuery(asNoTracking)
                .FirstOrDefaultAsync(x => x.ClientId == clientId
                    && x.RefreshToken == refreshToken);
        }

        public async Task<IClient> AddClientAsync(ClientDto clientDto)
        {
            var validation = _clientDtoValidator.Validate(clientDto);

            if (!validation.IsValid)
                throw new AuthValidationException(validation.Errors);

            var client = _mapper.Map<Client>(clientDto);

            client.PrimaryApiKey = Utils.GenerateApiKey();
            client.SecondaryApiKey = Utils.GenerateApiKey();

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

        public async Task<(IClient Client, IClientRefreshToken ClientRefreshToken)> AddClientRefreshTokenAsync(string clientApiKey)
        {
            var client = await GetClientAsync(clientApiKey);
            var clientRefreshToken = _mapper.Map<ClientRefreshToken>(client);

            clientRefreshToken.RefreshToken = Utils.GenerateApiKey(64);

            await _context.ClientRefreshTokens
                .AddAsync(clientRefreshToken);

            await _context.SaveChangesAsync();

            return (client, clientRefreshToken);
        }

        public async Task<IClient> RegenerateKeysAsync(int id)
        {
            return await base.RegenerateKeysAync<Client>(id);
        }

        public async Task<IClient> EnableDisableClientAsync(int id, bool enable = true)
        {
            var client = await base.GetEntityAsync<Client>(id, asNoTracking: false);

            client.Enabled = enable;

            await _context.SaveChangesAsync();

            return client;
        }
    }
}