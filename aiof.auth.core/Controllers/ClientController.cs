using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

using aiof.auth.data;
using aiof.auth.services;

namespace aiof.auth.core.Controllers
{
    [ApiController]
    [Route("client")]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public class ClientController : ControllerBase
    {
        private readonly IClientRepository _repo;

        public ClientController(IClientRepository repo)
        {
            _repo = repo ?? throw new ArgumentNullException(nameof(repo));
        }

        /// <summary>
        /// Get an existing <see cref="IClient"/> by Id
        /// </summary>
        [HttpGet]
        [Route("{id}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(IClient), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetClientAsync([FromRoute]int id)
        {
            return Ok(await _repo.GetClientAsync(id));
        }

        /// <summary>
        /// Disable an existing <see cref="IClient"/>
        /// </summary>
        [HttpGet]
        [Route("{id}/disable")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(IClient), StatusCodes.Status200OK)]
        public async Task<IActionResult> DisableClientAsync([FromRoute]int id)
        {
            return Ok(await _repo.EnableDisableClientAsync(id, false));
        }

        /// <summary>
        /// Enable an existing <see cref="IClient"/>
        /// </summary>
        [HttpGet]
        [Route("{id}/enable")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(IClient), StatusCodes.Status200OK)]
        public async Task<IActionResult> EnableClientAsync([FromRoute]int id)
        {
            return Ok(await _repo.EnableDisableClientAsync(id));
        }

        /// <summary>
        /// Regenerate <see cref="IClient.PrimaryApiKey"/> and <see cref="IClient.SecondaryApiKey"/> of an existing <see cref="IClient"/>
        /// </summary>
        [HttpGet]
        [Route("{id}/regenerate/keys")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(IClient), StatusCodes.Status200OK)]
        public async Task<IActionResult> RegenerateKeysAsync([FromRoute]int id)
        {
            return Ok(await _repo.RegenerateKeysAsync(id));
        }

        /// <summary>
        /// Get <see cref="IClientRefreshToken"/>'s of an existing <see cref="IClient"/>
        /// </summary>
        [HttpGet]
        [Route("{id}/refresh/tokens")]
        [ProducesResponseType(typeof(IEnumerable<IClientRefreshToken>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetRefreshTokensAsync([FromRoute]int id)
        {
            return Ok(await _repo.GetRefreshTokensAsync(id));
        }

        /// <summary>
        /// Create a <see cref="IClient"/>
        /// </summary>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(IClient), StatusCodes.Status200OK)]
        public async Task<IActionResult> AddClientAsync([FromBody]ClientDto clientDto)
        {
            return Ok(await _repo.AddClientAsync(clientDto));
        }
    }
}
