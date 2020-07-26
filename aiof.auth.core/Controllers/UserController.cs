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
    [Route("user")]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _repo;

        public UserController(IUserRepository repo)
        {
            _repo = repo ?? throw new ArgumentNullException(nameof(repo));
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetUserAsync([FromRoute]int id)
        {
            return Ok(await _repo.GetUserAsync(id));
        }

        [HttpGet]
        [Route("{username}/{password}")]
        public async Task<IActionResult> GetUserUsernamePasswordAsync([FromRoute]string username, string password)
        {
            return Ok(await _repo.GetUserAsync(username, password));
        }

        [HttpPost]
        public async Task<IActionResult> AddUserAsync([FromBody]UserDto userDto)
        {
            return Ok(await _repo.AddUserAsync(userDto));
        }
    }
}
