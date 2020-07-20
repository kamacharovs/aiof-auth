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
    public class ClientRepository : BaseRepository<Client>, IClientRepository
    {
        private readonly ILogger<ClientRepository> _logger;
        private readonly IAuthRepository _repo;
        private readonly IMapper _mapper;
        private readonly AuthContext _context;
        private readonly AbstractValidator<ClientDto> _clientDtoValidator;

        public ClientRepository(
            ILogger<ClientRepository> logger,
            IAuthRepository repo,
            IMapper mapper,
            AuthContext context,
            AbstractValidator<ClientDto> clientDtoValidator)
            : base(logger, repo, context)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _repo = repo ?? throw new ArgumentNullException(nameof(repo));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _clientDtoValidator = clientDtoValidator ?? throw new ArgumentNullException(nameof(clientDtoValidator));
        }     

        public async Task<IClient> GetClientAsync(int id)
        {
            return await base.GetEntityAsync(id);
        }

        public async Task<IClient> GetClientAsync(string apiKey)
        {
            return await base.GetEntityAsync(apiKey);
        }

        public async Task<IClient> AddClientAsync(ClientDto clientDto)
        {
            var validation = _clientDtoValidator.Validate(clientDto);

            if (!validation.IsValid)
                throw new AuthValidationException(validation.Errors);

            var client = _mapper.Map<Client>(clientDto);

            client.PrimaryApiKey = _repo.GenerateApiKey();
            client.SecondaryApiKey = _repo.GenerateApiKey();

            await _context.Clients
                .AddAsync(client);

            await _context.SaveChangesAsync();

            _logger.LogInformation($"Created Client with Id='{client.Id}' and PublicKey='{client.PublicKey}'");

            return client;
        }
        public async IAsyncEnumerable<IClient> AddClientsAsync(IEnumerable<ClientDto> clientDtos)
        {
            foreach (var clientDto in clientDtos)
                yield return await AddClientAsync(clientDto);
        }

        public async Task<IClient> RegenerateKeysAsync(int id)
        {
            return await base.RegenerateKeysAync(id);
        }

        public async Task<IClient> EnableDisableClientAsync(int id, bool enable = true)
        {
            var client = await base.GetEntityAsync(id, asNoTracking: false);

            client.Enabled = enable;

            await _context.SaveChangesAsync();

            return client;
        }
    }
}