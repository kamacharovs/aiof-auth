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

        public AuthController(IAuthRepository repo)
        {
            _repo = repo ?? throw new ArgumentNullException(nameof(repo));
        }

        [HttpPost]
        [Route("token")]
        public async Task<IActionResult> GetTokenAsync([FromBody]TokenRequest<User> tokenRequest)
        {
            return Ok(await _repo.GetUserTokenAsync(tokenRequest.ApiKey));
        }
    }
}
