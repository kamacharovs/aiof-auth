using System;
using System.Threading.Tasks;

namespace aiof.auth.services
{
    public interface IAuthRepository
    {
        Task<string> GetUserTokenAsync(int id);
    }
}