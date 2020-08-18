using System;
using System.Threading.Tasks;

using aiof.auth.data;

namespace aiof.auth.services
{
    public interface IUserRepository
    {
        Task<IUser> GetUserAsync(int id);
        Task<IUser> GetUserAsync(Guid publicKey);
        Task<IUser> GetUserAsync(string apiKey);
        Task<IUser> GetUserByUsernameAsync(
            string username, 
            bool asNoTracking = true);
        Task<IUser> GetUserAsync(string username, string password);
        Task<IUser> GetUserAsync(
            string firstName,
            string lastName,
            string email,
            string username);
        Task<IUser> GetUserAsync(UserDto userDto);
        Task<bool> DoesUsernameExistAsync(string username);
        Task<IUser> AddUserAsync(UserDto userDto);
        Task<IUser> UpdatePasswordAsync(
            string username, 
            string oldPassword, 
            string newPassword);
        string Hash(string password);
        bool Check(string hash, string password);
    }
}