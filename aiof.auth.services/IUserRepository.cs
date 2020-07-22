using System;
using System.Threading.Tasks;

using aiof.auth.data;

namespace aiof.auth.services
{
    public interface IUserRepository
    {
        Task<IUser> GetUserAsync(int id);
        Task<IUser> GetUserAsync(Guid publicKey);
        Task<IUser> GetUserAsync(string username, string password);
        Task<IUser> AddUserAsync(UserDto userDto);
        Task<IUser> UpdateUserPasswordAsync(
            string username, 
            string oldPassword, 
            string newPassword);
        string Hash(string password);
        bool Check(string hash, string password);
    }
}