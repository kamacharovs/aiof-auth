using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.FeatureManagement.Mvc;
using Microsoft.IdentityModel.Tokens;

using aiof.auth.data;
using aiof.auth.services;

namespace aiof.auth.core.Controllers
{
    /// <summary>
    /// Everything aiof authentication
    /// </summary>
    [ApiController]
    [Route("auth")]
    [Produces(Constants.ApplicationJson)]
    [Consumes(Constants.ApplicationJson)]
    [ProducesResponseType(typeof(IAuthProblemDetail), StatusCodes.Status500InternalServerError)]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _repo;

        public AuthController(IAuthRepository repo)
        {
            _repo = repo ?? throw new ArgumentNullException(nameof(repo));
        }

        /// <summary>
        /// Generate JWT
        /// </summary>
        [AllowAnonymous]
        [HttpPost]
        [Route("token")]
        [ProducesResponseType(typeof(IAuthProblemDetail), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(IAuthProblemDetail), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ITokenResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetTokenAsync([FromBody, Required] TokenRequest req)
        {
            return Ok(await _repo.GetTokenAsync(req));
        }

        /// <summary>
        /// Validate JWT
        /// </summary>
        [AllowAnonymous]
        [HttpPost]
        [Route("token/validate")]
        [ProducesResponseType(typeof(IAuthProblemDetail), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(IAuthProblemDetailBase), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ITokenResult), StatusCodes.Status200OK)]
        public IActionResult ValidateToken([FromBody, Required] ValidationRequest req)
        {
            return Ok(_repo.ValidateToken(req.AccessToken));
        }

        /// <summary>
        /// Generate refresh JWT for Client
        /// </summary>
        [FeatureGate(FeatureFlags.RefreshToken)]
        [AllowAnonymous]
        [HttpPost]
        [Route("token/refresh")]
        [ProducesResponseType(typeof(IAuthProblemDetail), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(IAuthProblemDetail), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ITokenResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> RefreshTokenAsync([FromBody, Required] TokenRequest req)
        {
            return Ok(await _repo.GetTokenAsync(req));
        }

        /// <summary>
        /// Revoke Client refresh token
        /// </summary>
        [Authorize(Roles = Roles.Admin)]
        [HttpPut]
        [Route("token/client/revoke")]
        [ProducesResponseType(typeof(IAuthProblemDetail), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(IAuthProblemDetail), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(IAuthProblemDetailBase), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(IAuthProblemDetailBase), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(IRevokeResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> RevokeClientRefreshTokenAsync([FromBody, Required] RevokeClientRequest request)
        {
            return Ok(await _repo.RevokeTokenAsync(request.Token, clientId: request.ClientId));
        }

        /// <summary>
        /// Revoke User refresh token
        /// </summary>
        [Authorize(Roles = Roles.Admin)]
        [HttpPut]
        [Route("token/user/revoke")]
        [ProducesResponseType(typeof(IAuthProblemDetail), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(IAuthProblemDetail), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(IAuthProblemDetailBase), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(IAuthProblemDetailBase), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(IRevokeResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> RevokeUserRefreshTokenAsync([FromBody, Required] RevokeUserRequest request)
        {
            return Ok(await _repo.RevokeTokenAsync(request.Token, userId: request.UserId));
        }

        /// <summary>
        /// Get all claims for JWT
        /// </summary>
        [AllowAnonymous]
        [HttpGet]
        [Route("claims")]
        [ProducesResponseType(typeof(IEnumerable<string>), StatusCodes.Status200OK)]
        public IActionResult GetClaims()
        {
            return Ok(AiofClaims.All);
        }

        /// <summary>
        /// Instrospect an access token
        /// </summary>
        [Authorize]
        [HttpGet]
        [Route("introspect")]
        [ProducesResponseType(typeof(IAuthProblemDetailBase), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(IIntrospectTokenResult), StatusCodes.Status200OK)]
        public IActionResult Introspect()
        {
            return Ok(_repo.Introspect());
        }

        /// <summary>
        /// Get JWKS for JWT creation
        /// </summary>
        [FeatureGate(FeatureFlags.OpenId)]
        [AllowAnonymous]
        [HttpGet]
        [Route("jwks")]
        [ProducesResponseType(typeof(JsonWebKey), StatusCodes.Status200OK)]
        public IActionResult GetJwks()
        {
            return Ok(_repo.GetPublicJsonWebKey());
        }

        /// <summary>
        /// Get OpenId Configuration for JWT creation
        /// </summary>
        [FeatureGate(FeatureFlags.OpenId)]
        [AllowAnonymous]
        [HttpGet]
        [Route(".well-known/openid-configuration")]
        [ProducesResponseType(typeof(IOpenIdConfig), StatusCodes.Status200OK)]
        public IActionResult GetOpenIdConfig()
        {
            return Ok(_repo.GetOpenIdConfig(
                HttpContext.Request.Host.ToString(),
                HttpContext.Request.IsHttps));
        }
    }
}
