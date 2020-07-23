using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using aiof.auth.data;

namespace aiof.auth.services
{
    public interface IClientRepository
    {
        Task<IClient> GetClientAsync(int id);
        Task<IClient> GetClientAsync(string apiKey);
        Task<IClientRefreshToken> GetClientRefreshTokenAsync(
            int clientId, 
            string refreshToken, 
            bool asNoTracking = true);
        Task<IClient> AddClientAsync(ClientDto clientDto);
        IAsyncEnumerable<IClient> AddClientsAsync(IEnumerable<ClientDto> clientDtos);
        Task<IClientRefreshToken> AddClientRefreshTokenAsync(string clientApiKey);
        Task<IClientRefreshToken> RevokeTokenAsync(int clientId, string token);
        Task<IClient> RegenerateKeysAsync(int id);
        Task<IClient> EnableDisableClientAsync(int id, bool enable = true);
    }
}