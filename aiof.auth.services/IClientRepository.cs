using System;
using System.Threading.Tasks;

using aiof.auth.data;

namespace aiof.auth.services
{
    public interface IClientRepository
    {
        Task<IClient> GetClientAsync(int id);
        Task<IClient> GetClientAsync(string apiKey);
        Task<IClient> AddClientAsync(ClientDto clientDto);
        Task<IClient> RegenerateKeysAsync(int id);
        Task<IClient> EnableDisableClientAsync(int id, bool enable = true);
    }
}