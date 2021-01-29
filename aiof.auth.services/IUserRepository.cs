using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using aiof.auth.data;

namespace aiof.auth.services
{
    public interface IUserRepository
    {
        Task<IUser> GetAsync(
            int id,
            bool asNoTracking = true);
        Task<IUser> GetAsync(
            ITenant tenant,
            bool asNoTracking = true);
        Task<IUser> GetAsync(Guid publicKey);
        Task<IUser> GetAsync(string apiKey);
        Task<IUser> GetByEmailAsync(
            string email, 
            bool asNoTracking = false);
        Task<IUser> GetAsync(
            string email, 
            string password);
        Task<IUser> GetAsync(
            string firstName,
            string lastName,
            string email);
        Task<IUser> GetAsync(UserDto userDto);
        Task<IUser> GetByRefreshTokenAsync(string refreshToken);
        Task<IUserRefreshToken> GetRefreshTokenAsync(int userId);
        Task<IEnumerable<IUserRefreshToken>> GetRefreshTokensAsync(int userId);
        Task<IUserRefreshToken> GetOrAddRefreshTokenAsync(int userId);
        Task<bool> DoesEmailExistAsync(string email);
        Task<IUser> AddAsync(UserDto userDto);
        Task<IUser> UpdatePasswordAsync(
            string email, 
            string oldPassword, 
            string newPassword);
        Task<IUserRefreshToken> RevokeTokenAsync(
            int userId,
            string token);
        string Hash(string password);
        bool Check(string hash, string password);
    }
}