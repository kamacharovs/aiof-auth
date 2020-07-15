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
        Task<IUser> GetUserAsync(int id, string apiKey);
        Task<IPublicKeyId> GetUserAsPublicKeyId(string apiKey);
        Task<IPublicKeyId> GetEntityAsync<T>(int id)
            where T : class, IPublicKeyId;
        Task<IUser> AddUserAsync(UserDto userDto);
        string GenerateApiKey();
        string Hash(string password);
        (bool Verified, bool NeedsUpgrade) Check(string hash, string password);
    }
}