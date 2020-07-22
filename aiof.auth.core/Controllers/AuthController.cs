using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

using aiof.auth.data;
using aiof.auth.services;

namespace aiof.auth.core.Controllers
{
    [ApiController]
    [Route("auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _repo;
        private readonly IClientRepository _clientRepo;

        public AuthController(IAuthRepository repo, IClientRepository clientRepo)
        {
            _repo = repo ?? throw new ArgumentNullException(nameof(repo));
            _clientRepo = clientRepo ?? throw new ArgumentNullException(nameof(clientRepo));
        }

        [HttpPost]
        [Route("token")]
        public async Task<IActionResult> GetTokenAsync([FromBody]TokenRequest<Client> request)
        {
            return Ok(await _clientRepo.GetTokenAsync(request.ApiKey));
        }

        [HttpGet]
        [Route("claims")]
        public IActionResult GetClaims()
        {
            return Ok(AiofClaims.All);
        }
    }
}
