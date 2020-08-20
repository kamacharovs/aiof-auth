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
        ITokenUserResponse GenerateJwtToken(IUser user);
        ITokenResponse GenerateJwtToken(
            IClient client, 
            string refreshToken = null,
            int? expiresIn = null);
        ITokenResponse GenerateJwtToken<T>(
            IEnumerable<Claim> claims, 
            IPublicKeyId entity = null, 
            string refreshToken = null, 
            int? expiresIn = null)
            where T : class, IPublicKeyId;
        AlgType GetAlgType<T>()
            where T : class, IPublicKeyId;
        RsaSecurityKey GetRsaKey(RsaKeyType rsaKeyType);
        ITokenResult ValidateUserToken(string token);
        ITokenResult ValidateClientToken(string token);
        ITokenResult ValidateToken<T>(string token)
            where T : class, IPublicKeyId;   
        Task<IRevokeResponse> RevokeTokenAsync(
            int clientId, 
            string token);
        JsonWebKey GetPublicJsonWebKey();
        IOpenIdConfig GetOpenIdConfig(
             string host,
             bool isHttps);
    }
}