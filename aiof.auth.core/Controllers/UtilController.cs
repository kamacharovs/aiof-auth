using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;

using aiof.auth.data;

namespace aiof.auth.core.Controllers
{
    /// <summary>
    /// Everything utility
    /// </summary>
    [AllowAnonymous]
    [ApiController]
    [ApiVersion(Constants.ApiV1)]
    [Route(Constants.ApiUtilRoute)]
    [Produces(Constants.ApplicationJson)]
    [Consumes(Constants.ApplicationJson)]
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