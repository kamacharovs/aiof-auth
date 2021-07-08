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
    [ApiVersion(Constants.ApiV1)]
    [Route(Constants.ApiClientRoute)]
    [Produces(Constants.ApplicationJson)]
    [Consumes(Constants.ApplicationJson)]
    [ProducesResponseType(typeof(IAuthProblemDetail), StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(typeof(IAuthProblemDetailBase), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(IAuthProblemDetailBase), StatusCodes.Status403Forbidden)]
    public class ClientController : ControllerBase
    {
        private readonly IClientRepository _repo;

        public ClientController(IClientRepository repo)
        {
            _repo = repo ?? throw new ArgumentNullException(nameof(repo));
        }

        /// <summary>
        /// Get Client by Id
        /// </summary>
        [HttpGet]
        [Route("{id}")]
        [ProducesResponseType(typeof(IAuthProblemDetail), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(IClient), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAsync([FromRoute, Required] int id)
        {
            return Ok(await _repo.GetAsync(id));
        }

        /// <summary>
        /// Disable Client
        /// </summary>
        [HttpPost]
        [Route("{id}/disable")]
        [ProducesResponseType(typeof(IAuthProblemDetail), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(IClient), StatusCodes.Status200OK)]
        public async Task<IActionResult> DisableClientAsync([FromRoute, Required] int id)
        {
            return Ok(await _repo.DisableAsync(id));
        }

        /// <summary>
        /// Enable Client
        /// </summary>
        [HttpPost]
        [Route("{id}/enable")]
        [ProducesResponseType(typeof(IAuthProblemDetail), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(IClient), StatusCodes.Status200OK)]
        public async Task<IActionResult> EnableClientAsync([FromRoute, Required] int id)
        {
            return Ok(await _repo.EnableAsync(id));
        }

        /// <summary>
        /// Regenerate PrimaryApiKey and SecondaryApiKey of Client
        /// </summary>
        [HttpPost]
        [Route("{id}/regenerate/keys")]
        [ProducesResponseType(typeof(IAuthProblemDetail), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(IClient), StatusCodes.Status200OK)]
        public async Task<IActionResult> RegenerateKeysAsync([FromRoute, Required] int id)
        {
            return Ok(await _repo.RegenerateKeysAsync(id));
        }

        /// <summary>
        /// Get Client refresh tokens
        /// </summary>
        [HttpGet]
        [Route("{id}/refresh/tokens")]
        [ProducesResponseType(typeof(IEnumerable<IClientRefreshToken>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetRefreshTokensAsync([FromRoute, Required] int id)
        {
            return Ok(await _repo.GetRefreshTokensAsync(id));
        }

        /// <summary>
        /// Create Client
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
