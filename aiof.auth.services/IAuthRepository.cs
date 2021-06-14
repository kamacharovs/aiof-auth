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
        ITokenResponse GenerateJwt(
            IUser user,
            string refreshToken = null,
            int? expiresIn = null);
        ITokenResponse GenerateJwt(
            IClient client, 
            string refreshToken = null,
            int? expiresIn = null);
        ITokenResponse GenerateJwt<T>(
            IEnumerable<Claim> claims, 
            IPublicKeyId entity = null, 
            string refreshToken = null, 
            int? expiresIn = null)
            where T : class, IPublicKeyId;
        RsaSecurityKey GetRsaKey(RsaKeyType rsaKeyType);
        ITokenResult ValidateToken(string token);
        Task<IRevokeResponse> RevokeTokenAsync(
            string token,
            int? userId = null,
            int? clientId = null);
        IIntrospectTokenResult Introspect();
        JsonWebKey GetPublicJsonWebKey();
        IOpenIdConfig GetOpenIdConfig(
             string host,
             bool isHttps);
    }
}