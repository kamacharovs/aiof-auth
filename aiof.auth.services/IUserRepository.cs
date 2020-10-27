using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using aiof.auth.data;

namespace aiof.auth.services
{
    public interface IUserRepository
    {
        Task<IUser> GetUserAsync(
            int id,
            bool asNoTracking = true);
        Task<IUser> GetUserAsync(Guid publicKey);
        Task<IUser> GetUserAsync(string apiKey);
        Task<IUser> GetUserByUsernameAsync(
            string username, 
            bool asNoTracking = false);
        Task<IUser> GetUserAsync(
            string username, 
            string password);
        Task<IUser> GetUserAsync(
            string firstName,
            string lastName,
            string email,
            string username);
        Task<IUser> GetUserAsync(UserDto userDto);
        Task<IUser> GetUserByRefreshTokenAsync(string refreshToken);
        Task<IUserRefreshToken> GetRefreshTokenAsync(int userId);
        Task<IEnumerable<IUserRefreshToken>> GetRefreshTokensAsync(int userId);
        Task<IUserRefreshToken> GetOrAddRefreshTokenAsync(int userId);
        Task<bool> DoesUsernameExistAsync(string username);
        Task<IUser> AddUserAsync(UserDto userDto);
        Task<IUser> UpdatePasswordAsync(
            string username, 
            string oldPassword, 
            string newPassword);
        Task<IUserRefreshToken> RevokeTokenAsync(
            int userId,
            string token);
        string Hash(string password);
        bool Check(string hash, string password);
    }
}