using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.FeatureManagement.Mvc;

using aiof.auth.data;
using aiof.auth.services;

namespace aiof.auth.core.Controllers
{
    [ApiController]
    [Route("auth")]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _repo;

        public AuthController(IAuthRepository repo)
        {
            _repo = repo ?? throw new ArgumentNullException(nameof(repo));
        }

        /// <summary>
        /// Generate a JWT for <see cref="IUser"/>, <see cref="IClient"/>
        /// </summary>
        [HttpPost]
        [Route("token")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ITokenResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetTokenAsync([FromBody]TokenRequest req)
        {
            return Ok(await _repo.GetTokenAsync(req));
        }

        /// <summary>
        /// Generate a refresh JWT for <see cref="IClient"/>
        /// </summary>
        [FeatureGate(FeatureFlags.RefreshToken)]
        [HttpPost]
        [Route("token/refresh")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ITokenResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> RefreshTokenAsync([FromBody]TokenRequest req)
        {
            return Ok(await _repo.GetTokenAsync(req));
        }

        /// <summary>
        /// Revoke an existing <see cref="IClient"/> refresh token
        /// </summary>
        [HttpPut]
        [Route("token/revoke")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        public async Task<IActionResult> RevokeRefreshTokenAsync([FromBody]RevokeRequest request)
        {
            return Ok(await _repo.RevokeTokenAsync(request.ClientId, request.Token));
        }

        /// <summary>
        /// Get all available claims for JWT
        /// </summary>
        [HttpGet]
        [Route("claims")]
        [ProducesResponseType(typeof(IEnumerable<string>), StatusCodes.Status200OK)]
        public IActionResult GetClaims()
        {
            return Ok(AiofClaims.All);
        }

        /// <summary>
        /// Get OpenId Configuration for JWT creation
        /// </summary>
        [FeatureGate(FeatureFlags.OpenId)]
        [HttpGet]
        [Route(".well-known/openid-configuration")]
        public IActionResult GetOpenIdConfig()
        {
            return Ok(_repo.GetOpenIdConfig(
                HttpContext.Request.Host.ToString(),
                HttpContext.Request.IsHttps));
        }
    }
}
