using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

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

        public UserController(IUserRepository repo)
        {
            _repo = repo ?? throw new ArgumentNullException(nameof(repo));
        }

        /// <summary>
        /// Get an existing User by Id
        /// </summary>
        [HttpGet]
        [Route("{id}")]
        [ProducesResponseType(typeof(IAuthProblemDetail), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(IUser), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetUserAsync([FromRoute, Required] int id)
        {
            return Ok(await _repo.GetUserAsync(id));
        }

        /// <summary>
        /// Get an existing User by Username and Password
        /// </summary>
        [HttpGet]
        [Route("{username}/{password}")]
        [ProducesResponseType(typeof(IAuthProblemDetail), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(IUser), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetUserUsernamePasswordAsync(
            [FromRoute, Required] string username,
            [FromRoute, Required] string password)
        {
            return Ok(await _repo.GetUserAsync(username, password));
        }

        /// <summary>
        /// Create a User
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(IAuthProblemDetail), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(IUser), StatusCodes.Status201Created)]
        public async Task<IActionResult> AddUserAsync([FromBody, Required] UserDto userDto)
        {
            return Created(nameof(User), await _repo.AddUserAsync(userDto));
        }

        /// <summary>
        /// Hash a Password
        /// </summary>
        [HttpGet]
        [Route("hash/{password}")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public IActionResult HashPassword([FromRoute, Required] string password)
        {
            return Ok(_repo.Hash(password));
        }
    }
}
