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
            bool asNoTracking = true);
        Task<IUser> GetAsync(
            string email, 
            string password,
            bool asNoTracking = true);
        Task<IUser> GetAsync(
            string firstName,
            string lastName,
            string email);
        Task<IUser> GetAsync(UserDto userDto);
        Task<IUser> GetByRefreshTokenAsync(string refreshToken);
        Task<IUserRefreshToken> GetRefreshTokenAsync(int userId);
        Task<IEnumerable<IUserRefreshToken>> GetRefreshTokensAsync(
            int userId,
            bool asNoTracking = true);
        Task<IUserRefreshToken> GetOrAddRefreshTokenAsync(int userId);
        Task<bool> DoesEmailExistAsync(string email);
        Task<IUser> AddAsync(UserDto userDto);
        Task<IUser> UpdatePasswordAsync(
            ITenant tenant,
            string oldPassword, 
            string newPassword);
        Task<IUser> UpdatePasswordAsync(
            string email,
            string oldPassword,
            string newPassword);
        Task RevokeAsync(int userId);
        string Hash(string password);
        bool Check(string hash, string password);
    }
}