using System;
using System.Threading.Tasks;

using aiof.auth.data;

namespace aiof.auth.services
{
    public interface IAuthRepository
    {
        Task<IUser> GetUserAsync(int id);
        Task<string> GetUserTokenAsync(int id);
    }
}