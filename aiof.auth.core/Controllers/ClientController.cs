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
    [Route("auth/client")]
    public class ClientController : ControllerBase
    {
        private readonly IClientRepository _repo;

        public ClientController(IClientRepository repo)
        {
            _repo = repo ?? throw new ArgumentNullException(nameof(repo));
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetClientAsync([FromRoute]int id)
        {
            return Ok(await _repo.GetClientAsync(id));
        }

        [HttpPost]
        public async Task<IActionResult> AddClientAsync([FromBody]ClientDto clientDto)
        {
            return Ok(await _repo.AddClientAsync(clientDto));
        }
    }
}
