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
    /// <summary>
    /// Everything aiof client
    /// </summary>
    [ApiController]
    [Route("client")]
    [Produces(Keys.ApplicationJson)]
    [Consumes(Keys.ApplicationJson)]
    [ProducesResponseType(typeof(AuthProblemDetail), StatusCodes.Status500InternalServerError)]
    public class ClientController : ControllerBase
    {
        private readonly IClientRepository _repo;

        public ClientController(IClientRepository repo)
        {
            _repo = repo ?? throw new ArgumentNullException(nameof(repo));
        }

        /// <summary>
        /// Get an existing Client by Id
        /// </summary>
        [HttpGet]
        [Route("{id}")]
        [ProducesResponseType(typeof(AuthProblemDetail), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(IClient), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetClientAsync([FromRoute]int id)
        {
            return Ok(await _repo.GetClientAsync(id));
        }

        /// <summary>
        /// Disable an existing Client
        /// </summary>
        [HttpGet]
        [Route("{id}/disable")]
        [ProducesResponseType(typeof(AuthProblemDetail), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(IClient), StatusCodes.Status200OK)]
        public async Task<IActionResult> DisableClientAsync([FromRoute]int id)
        {
            return Ok(await _repo.EnableDisableClientAsync(id, false));
        }

        /// <summary>
        /// Enable an existing Client
        /// </summary>
        [HttpGet]
        [Route("{id}/enable")]
        [ProducesResponseType(typeof(AuthProblemDetail), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(IClient), StatusCodes.Status200OK)]
        public async Task<IActionResult> EnableClientAsync([FromRoute]int id)
        {
            return Ok(await _repo.EnableDisableClientAsync(id));
        }

        /// <summary>
        /// Regenerate PrimaryApiKey and SecondaryApiKey of an existing Client
        /// </summary>
        [HttpGet]
        [Route("{id}/regenerate/keys")]
        [ProducesResponseType(typeof(AuthProblemDetail), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(IClient), StatusCodes.Status200OK)]
        public async Task<IActionResult> RegenerateKeysAsync([FromRoute]int id)
        {
            return Ok(await _repo.RegenerateKeysAsync(id));
        }

        /// <summary>
        /// Get Client Refresh Tokens of an existing Client
        /// </summary>
        [HttpGet]
        [Route("{id}/refresh/tokens")]
        [ProducesResponseType(typeof(IEnumerable<IClientRefreshToken>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetRefreshTokensAsync([FromRoute]int id)
        {
            return Ok(await _repo.GetRefreshTokensAsync(id));
        }

        /// <summary>
        /// Create a Client
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(AuthProblemDetail), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(IClient), StatusCodes.Status201Created)]
        public async Task<IActionResult> AddClientAsync([FromBody]ClientDto clientDto)
        {
            return Created(nameof(User), await _repo.AddClientAsync(clientDto));
        }
    }
}
