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
    /// Everything aiof user
    /// </summary>
    [ApiController]
    [Route("user")]
    [Produces(Keys.ApplicationJson)]
    [Consumes(Keys.ApplicationJson)]
    [ProducesResponseType(typeof(IAuthProblemDetail), StatusCodes.Status500InternalServerError)]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _repo;
        private readonly IAuthRepository _authRepo;

        public UserController(
            IUserRepository repo,
            IAuthRepository authRepo)
        {
            _repo = repo ?? throw new ArgumentNullException(nameof(repo));
            _authRepo = authRepo ?? throw new ArgumentNullException(nameof(authRepo));
        }

        /// <summary>
        /// Get an existing User by Id
        /// </summary>
        [Authorize(Roles = Roles.Admin)]
        [HttpGet]
        [Route("{id}")]
        [ProducesResponseType(typeof(IAuthProblemDetail), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(IAuthProblemDetailBase), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(IAuthProblemDetailBase), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(IUser), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetUserAsync([FromRoute, Required] int id)
        {
            return Ok(await _repo.GetUserAsync(id));
        }

        /// <summary>
        /// Get an existing User by Username
        /// </summary>
        [Authorize(Roles = Roles.Admin)]
        [HttpGet]
        [ProducesResponseType(typeof(IAuthProblemDetail), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(IAuthProblemDetailBase), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(IAuthProblemDetailBase), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(IUser), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetUserByUsernameAsync([FromQuery, Required] string username)
        {
            return Ok(await _repo.GetUserByUsernameAsync(username));
        }

        /// <summary>
        /// Get an existing User by Username and Password
        /// </summary>
        [Authorize(Roles = Roles.Admin)]
        [HttpGet]
        [Route("{username}/{password}")]
        [ProducesResponseType(typeof(IAuthProblemDetail), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(IAuthProblemDetailBase), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(IAuthProblemDetailBase), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(IUser), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetUserUsernamePasswordAsync(
            [FromRoute, Required] string username,
            [FromRoute, Required] string password)
        {
            return Ok(await _repo.GetUserAsync(username, password));
        }

        /// <summary>
        /// Get a User first non-revoked refresh token
        /// </summary>
        [Authorize(Roles = Roles.Admin)]
        [HttpGet]
        [Route("{id}/refresh/token")]
        [ProducesResponseType(typeof(IAuthProblemDetail), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(IAuthProblemDetailBase), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(IAuthProblemDetailBase), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetUserRefreshTokenAsync([FromRoute, Required] int id)
        {
            return Ok(await _repo.GetRefreshTokenAsync(id));
        }

        /// <summary>
        /// Get a User refresh tokens
        /// </summary>
        [Authorize(Roles = Roles.Admin)]
        [HttpGet]
        [Route("{id}/refresh/tokens")]
        [ProducesResponseType(typeof(IAuthProblemDetailBase), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(IAuthProblemDetailBase), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetUserRefreshTokensAsync([FromRoute, Required] int id)
        {
            return Ok(await _repo.GetRefreshTokensAsync(id));
        }

        /// <summary>
        /// Create a User
        /// </summary>
        [AllowAnonymous]
        [HttpPost]
        [ProducesResponseType(typeof(IAuthProblemDetail), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(IUser), StatusCodes.Status201Created)]
        public async Task<IActionResult> AddUserAsync([FromBody, Required] UserDto userDto)
        {
            return Created(nameof(User), _authRepo.GenerateJwtToken(await _repo.AddUserAsync(userDto)));
        }

        /// <summary>
        /// Hash a Password
        /// </summary>
        [Authorize(Roles = Roles.Admin)]
        [HttpGet]
        [Route("hash/{password}")]
        [ProducesResponseType(typeof(IAuthProblemDetailBase), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(IAuthProblemDetailBase), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public IActionResult HashPassword([FromRoute, Required] string password)
        {
            return Ok(_repo.Hash(password));
        }
    }
}
