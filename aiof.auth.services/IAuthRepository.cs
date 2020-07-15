using System;
using System.Threading.Tasks;

using aiof.auth.data;

namespace aiof.auth.services
{
    public interface IAuthRepository
    {
        Task<ITokenResponse> GetUserTokenAsync(string apiKey);
        ITokenResponse GenerateJwtToken(IUser user);
    }
}