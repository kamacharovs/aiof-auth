using System;
using System.Net;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using AutoMapper;
using FluentValidation;

using aiof.auth.data;

namespace aiof.auth.services
{
    public class ClientRepository
    {
        private readonly ILogger<ClientRepository> _logger;
        private readonly IAuthRepository _repo;
        private readonly IEnvConfiguration _envConfig;
        private readonly IMapper _mapper;
        private readonly AuthContext _context;
        private readonly AbstractValidator<ClientDto> _clientDtoValidator;

        public ClientRepository(
            ILogger<ClientRepository> logger,
            IAuthRepository repo,
            IEnvConfiguration envConfig,
            IMapper mapper,
            AuthContext context,
            AbstractValidator<ClientDto> clientDtoValidator)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _repo = repo ?? throw new ArgumentNullException(nameof(repo));
            _envConfig = envConfig ?? throw new ArgumentNullException(nameof(envConfig));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _clientDtoValidator = clientDtoValidator ?? throw new ArgumentNullException(nameof(clientDtoValidator));
        }


    }
}