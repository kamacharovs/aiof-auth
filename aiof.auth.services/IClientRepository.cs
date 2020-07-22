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
        Task<IClient> AddClientAsync(ClientDto clientDto);
        IAsyncEnumerable<IClient> AddClientsAsync(IEnumerable<ClientDto> clientDtos);
        Task<ITokenResponse> GetTokenAsync(string apiKey);
        Task<IClient> RegenerateKeysAsync(int id);
        Task<IClient> EnableDisableClientAsync(int id, bool enable = true);
    }
}