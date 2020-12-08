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
        private readonly ITenant _tenant;

        public UserController(
            IUserRepository repo,
            IAuthRepository authRepo,
            ITenant tenant)
        {
            _repo = repo ?? throw new ArgumentNullException(nameof(repo));
            _authRepo = authRepo ?? throw new ArgumentNullException(nameof(authRepo));
            _tenant = tenant;
        }

        /// <summary>
        /// Get User
        /// </summary>
        [Authorize]
        [HttpGet]
        [ProducesResponseType(typeof(IAuthProblemDetailBase), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(IAuthProblemDetailBase), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(IUser), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAsync()
        {
            return Ok(await _repo.GetAsync(_tenant));
        }

        /// <summary>
        /// Get User by id
        /// </summary>
        [Authorize(Roles = Roles.Admin)]
        [HttpGet]
        [Route("{id}")]
        [ProducesResponseType(typeof(IAuthProblemDetail), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(IAuthProblemDetailBase), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(IAuthProblemDetailBase), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(IUser), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAsync([FromRoute, Required] int id)
        {
            return Ok(await _repo.GetAsync(id));
        }

        /// <summary>
        /// Get User by username
        /// </summary>
        [Authorize(Roles = Roles.Admin)]
        [HttpGet("username/{username}")]
        [ProducesResponseType(typeof(IAuthProblemDetail), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(IAuthProblemDetailBase), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(IAuthProblemDetailBase), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(IUser), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetByUsernameAsync([FromRoute, Required] string username)
        {
            return Ok(await _repo.GetByUsernameAsync(username));
        }

        /// <summary>
        /// Get User first non-revoked refresh token
        /// </summary>
        [Authorize(Roles = Roles.Admin)]
        [HttpGet]
        [Route("{id}/refresh/token")]
        [ProducesResponseType(typeof(IAuthProblemDetail), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(IAuthProblemDetailBase), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(IAuthProblemDetailBase), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(IUserRefreshToken), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetRefreshTokenAsync([FromRoute, Required] int id)
        {
            return Ok(await _repo.GetRefreshTokenAsync(id));
        }

        /// <summary>
        /// Get User refresh tokens
        /// </summary>
        [Authorize(Roles = Roles.Admin)]
        [HttpGet]
        [Route("{id}/refresh/tokens")]
        [ProducesResponseType(typeof(IAuthProblemDetailBase), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(IAuthProblemDetailBase), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(IEnumerable<IUserRefreshToken>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetRefreshTokensAsync([FromRoute, Required] int id)
        {
            return Ok(await _repo.GetRefreshTokensAsync(id));
        }

        /// <summary>
        /// Create User
        /// </summary>
        [AllowAnonymous]
        [HttpPost]
        [ProducesResponseType(typeof(IAuthProblemDetail), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ITokenUserResponse), StatusCodes.Status201Created)]
        public async Task<IActionResult> AddAsync([FromBody, Required] UserDto userDto)
        {
            return Created(nameof(User), _authRepo.GenerateJwtToken(await _repo.AddAsync(userDto)));
        }

        /// <summary>
        /// Hash password
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
