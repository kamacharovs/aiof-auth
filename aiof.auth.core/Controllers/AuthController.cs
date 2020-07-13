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

        [HttpGet]
        [Route("user/{id}")]
        public async Task<IActionResult> GetTokenAsync([FromRoute]int id)
        {
            return Ok(await _repo.GetUserTokenAsync(id));
        }

        [HttpPost]
        [Route("user")]
        public async Task<IActionResult> AddUserAsync([FromBody]UserDto userDto)
        {
            return Ok(await _repo.AddUserAsync(userDto));
        }
    }
}
