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
        public async Task<IActionResult> GetTokenAsync([FromBody]TokenRequest req)
        {
            return Ok(await _repo.GetTokenAsync(req));
        }

        [HttpPut]
        [Route("token/revoke")]
        public async Task<IActionResult> RevokeRefreshTokenAsync([FromBody]RevokeRequest request)
        {
            await _repo.RevokeTokenAsync(request.ClientId, request.Token);

            return Ok("Success");
        }

        [HttpGet]
        [Route("claims")]
        public IActionResult GetClaims()
        {
            return Ok(AiofClaims.All);
        }
    }
}
