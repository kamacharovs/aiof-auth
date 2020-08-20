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
    /// Everything utility
    /// </summary>
    [ApiController]
    [Route("util")]
    [Produces(Keys.ApplicationJson)]
    [Consumes(Keys.ApplicationJson)]
    [ProducesResponseType(typeof(IAuthProblemDetail), StatusCodes.Status500InternalServerError)]
    public class UtilController : ControllerBase
    {
        /// <summary>
        /// Generate User ApiKey
        /// </summary>
        [HttpGet]
        [Route("apikey/{length}/user")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public IActionResult GenerateUserApiKey([FromRoute, Required] int length)
        {
            return Ok(Utils.GenerateApiKey<User>(length));
        }

        /// <summary>
        /// Generate Client ApiKey
        /// </summary>
        [HttpGet]
        [Route("apikey/{length}/client")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public IActionResult GenerateClientApiKey([FromRoute, Required] int length)
        {
            return Ok(Utils.GenerateApiKey<Client>(length));
        }
    }
}