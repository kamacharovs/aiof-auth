using System;
using System.Security.Claims;
using System.Collections.Generic;
using System.Threading.Tasks;

using aiof.auth.data;

namespace aiof.auth.services
{
    public interface IAuthRepository
    {
        Task<ITokenResponse> GetTokenAsync(ITokenRequest request);
        ITokenResponse GenerateJwtToken(IUser user);
        ITokenResponse GenerateJwtToken(IClient client, string refreshToken = null);
        ITokenResponse GenerateJwtToken(
            IEnumerable<Claim> claims, 
            IPublicKeyId entity = null,
            string refreshToken = null);
        ITokenResult ValidateToken(string token);
        bool IsAuthenticated(string token);
    }
}