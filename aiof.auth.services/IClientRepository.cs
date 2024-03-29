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
        Task<IEnumerable<IClientRefreshToken>> GetRefreshTokensAsync(
            int clientId,
            bool asNoTracking = true);
        Task<IClientRefreshToken> GetOrAddRefreshTokenAsync(IClient client);
        Task<IClient> AddClientAsync(ClientDto clientDto);
        IAsyncEnumerable<IClient> AddClientsAsync(IEnumerable<ClientDto> clientDtos);
        Task RevokeAsync(int clientId);
        Task<IClient> EnableAsync(int id);
        Task<IClient> DisableAsync(int id);
        Task<IClient> RegenerateKeysAsync(int id);
    }
}