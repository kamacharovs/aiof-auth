using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;

using aiof.auth.data;
using aiof.auth.services;

namespace aiof.auth.core.Controllers
{
    /// <summary>
    /// Everything aiof client
    /// </summary>
    [Authorize(Roles = Roles.Admin)]
    [ApiController]
    [Route("client")]
    [Produces(Keys.ApplicationJson)]
    [Consumes(Keys.ApplicationJson)]
    [ProducesResponseType(typeof(IAuthProblemDetail), StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
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
        [ProducesResponseType(typeof(IAuthProblemDetail), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(IClient), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetClientAsync([FromRoute, Required] int id)
        {
            return Ok(await _repo.GetClientAsync(id));
        }

        /// <summary>
        /// Disable an existing Client
        /// </summary>
        [HttpGet]
        [Route("{id}/disable")]
        [ProducesResponseType(typeof(IAuthProblemDetail), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(IClient), StatusCodes.Status200OK)]
        public async Task<IActionResult> DisableClientAsync([FromRoute, Required] int id)
        {
            return Ok(await _repo.EnableDisableClientAsync(id, false));
        }

        /// <summary>
        /// Enable an existing Client
        /// </summary>
        [HttpGet]
        [Route("{id}/enable")]
        [ProducesResponseType(typeof(IAuthProblemDetail), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(IClient), StatusCodes.Status200OK)]
        public async Task<IActionResult> EnableClientAsync([FromRoute, Required] int id)
        {
            return Ok(await _repo.EnableDisableClientAsync(id));
        }

        /// <summary>
        /// Regenerate PrimaryApiKey and SecondaryApiKey of an existing Client
        /// </summary>
        [HttpGet]
        [Route("{id}/regenerate/keys")]
        [ProducesResponseType(typeof(IAuthProblemDetail), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(IClient), StatusCodes.Status200OK)]
        public async Task<IActionResult> RegenerateKeysAsync([FromRoute, Required] int id)
        {
            return Ok(await _repo.RegenerateKeysAsync(id));
        }

        /// <summary>
        /// Get Client Refresh Tokens of an existing Client
        /// </summary>
        [HttpGet]
        [Route("{id}/refresh/tokens")]
        [ProducesResponseType(typeof(IEnumerable<IClientRefreshToken>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetRefreshTokensAsync([FromRoute, Required] int id)
        {
            return Ok(await _repo.GetRefreshTokensAsync(id));
        }

        /// <summary>
        /// Create a Client
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(IAuthProblemDetail), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(IClient), StatusCodes.Status201Created)]
        public async Task<IActionResult> AddClientAsync([FromBody, Required] ClientDto clientDto)
        {
            return Created(nameof(User), await _repo.AddClientAsync(clientDto));
        }
    }
}
