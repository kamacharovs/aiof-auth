using System;
using System.Threading.Tasks;

using aiof.auth.data;

namespace aiof.auth.services
{
    public interface IAuthRepository
    {
        ITokenResponse GenerateJwtToken(IUser user);
        string GenerateApiKey();
    }
}