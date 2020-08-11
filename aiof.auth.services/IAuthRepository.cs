using System;
using System.Security.Claims;
using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.IdentityModel.Tokens;

using aiof.auth.data;

namespace aiof.auth.services
{
    public interface IAuthRepository
    {
        Task<ITokenResponse> GetTokenAsync(ITokenRequest request);
        Task<IRevokeResponse> RevokeTokenAsync(
            int clientId, 
            string token);
        ITokenResponse GenerateJwtToken(IUser user);
        ITokenResponse GenerateJwtToken(
            IClient client, 
            string refreshToken = null,
            int? expiresIn = null);
        ITokenResponse GenerateJwtToken<T>(
            IEnumerable<Claim> claims, 
            string refreshToken = null, 
            int? expiresIn = null)
            where T : class, IPublicKeyId;
        ITokenResult ValidateToken<T>(string token)
            where T : class, IPublicKeyId;
        ITokenResult ValidateToken(IValidationRequest request);
        JsonWebKey GetPublicJsonWebKey();
        IOpenIdConfig GetOpenIdConfig(
             string host,
             bool isHttps);
    }
}