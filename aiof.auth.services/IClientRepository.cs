using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using aiof.auth.data;

namespace aiof.auth.services
{
    public interface IClientRepository
    {
        Task<IClient> GetAsync(
            int id,
            bool asNoTracking = true);
        Task<IClient> GetAsync(
            string apiKey,
            bool asNoTracking = true);
        Task<IClient> GetByRefreshTokenAsync(
            string token,
            bool asNoTracking = true);
        Task<IClientRefreshToken> GetRefreshTokenAsync(
            int clientId, 
            string refreshToken, 
            bool asNoTracking = true);
        Task<IEnumerable<IClientRefreshToken>> GetRefreshTokensAsync(int clientId);
        Task<IClientRefreshToken> GetOrAddRefreshTokenAsync(string clientApiKey);
        Task<IClient> AddClientAsync(ClientDto clientDto);
        IAsyncEnumerable<IClient> AddClientsAsync(IEnumerable<ClientDto> clientDtos);
        Task<IClientRefreshToken> RevokeTokenAsync(int clientId, string token);
        Task<IClient> SoftDeleteAsync(int id);
        Task<IClient> RegenerateKeysAsync(int id);
        Task<IClient> EnableDisableClientAsync(int id, bool enable = true);
    }
}