using System;
using System.Threading.Tasks;

using aiof.auth.data;

namespace aiof.auth.services
{
    public interface IAuthRepository
    {
        Task<IUser> GetUserAsync(int id);
        Task<IUser> GetUserAsync(string apiKey);
        Task<string> GetUserTokenAsync(int id);
        Task<IUser> AddUserAsync(UserDto userDto);
        string GenerateApiKey();
    }
}