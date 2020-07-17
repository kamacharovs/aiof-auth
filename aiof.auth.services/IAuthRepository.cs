using System;
using System.Security.Claims;
using System.Collections.Generic;

using aiof.auth.data;

namespace aiof.auth.services
{
    public interface IAuthRepository
    {
        ITokenResponse GenerateJwtToken(IUser user);
        ITokenResponse GenerateJwtToken(IClient client);
        ITokenResponse GenerateJwtToken(IEnumerable<Claim> claims, IPublicKeyId entity = null);
        ITokenResult ValidateToken(string token);
        bool IsAuthenticated(string token);
        string GenerateApiKey();
    }
}