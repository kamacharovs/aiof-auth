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
    /// Everything aiof user
    /// </summary>
    [ApiController]
    [Route("user")]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(IUser), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetUserAsync([FromRoute]int id)
        {
            return Ok(await _repo.GetUserAsync(id));
        }
        
        /// <summary>
        /// Get an existing User by Username and Password
        /// </summary>
        [HttpGet]
        [Route("{username}/{password}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(IUser), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetUserUsernamePasswordAsync([FromRoute]string username, string password)
        {
            return Ok(await _repo.GetUserAsync(username, password));
        }

        /// <summary>
        /// Create a User
        /// </summary>
        /// <response code="201">Returns the newly created User</response>
        /// <response code="400">If validations fail</response> 
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(IUser), StatusCodes.Status201Created)]
        public async Task<IActionResult> AddUserAsync([FromBody]UserDto userDto)
        {
            return Created(nameof(User), await _repo.AddUserAsync(userDto));
        }

        /// <summary>
        /// Hash a Password
        /// </summary>
        /// <param name="password"></param>
        /// <returns><see cref="string"/></returns>
        [HttpGet]
        [Route("hash/{password}")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public IActionResult HashPassword([FromRoute]string password)
        {
            return Ok(_repo.Hash(password));
        }
    }
}
