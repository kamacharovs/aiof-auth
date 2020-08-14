using System;
using System.Threading;
using System.Threading.Tasks;

using Xunit;

using aiof.auth.data;
using aiof.auth.services;

namespace aiof.auth.tests
{
    [Trait(Helper.Category, Helper.UnitTest)]
    public class ClientRepositoryTests
    {
        private readonly IClientRepository _repo;

        public ClientRepositoryTests()
        {
            _repo = Helper.GetRequiredService<IClientRepository>() ?? throw new ArgumentNullException(nameof(IUserRepository));
        }

        [Theory]
        [MemberData(nameof(Helper.ClientsId), MemberType = typeof(Helper))]
        public async Task GetClientAsync_By_Id(int id)
        {
            var client = await _repo.GetClientAsync(id);

            Assert.NotNull(client);
            Assert.Equal(id, client.Id);
            Assert.NotNull(client.PrimaryApiKey);
            Assert.NotNull(client.SecondaryApiKey);
        }

        [Theory]
        [MemberData(nameof(Helper.ClientsApiKey), MemberType = typeof(Helper))]
        public async Task GetClientAsync_By_ApiKey(string apiKey)
        {
            var client = await _repo.GetClientAsync(apiKey);

            Assert.NotNull(client);
            Assert.NotNull(client.PrimaryApiKey);
            Assert.NotNull(client.SecondaryApiKey);
        }

        [Theory]
        [MemberData(nameof(Helper.RandomClientDtos), MemberType = typeof(Helper))]
        public async Task AddClientAsync_Valid(
            string name, 
            bool enabled)
        {
            var client = await _repo.AddClientAsync(new ClientDto
            {
                Name = name,
                Enabled = enabled
            });

            Assert.NotNull(client);
            Assert.NotEqual(0, client.Id);
            Assert.NotEqual(Guid.Empty, client.PublicKey);
            Assert.Equal(name, client.Name);
            Assert.Equal(enabled, client.Enabled);
            Assert.NotNull(client.PrimaryApiKey);
            Assert.NotNull(client.SecondaryApiKey);
        }

        [Theory]
        [MemberData(nameof(Helper.ClientRefreshClientIdToken), MemberType = typeof(Helper))]
        public async Task RevokeTokenAsync(
            int clientId, 
            string token)
        {
            var clientRefreshTokenBefore = await _repo.GetClientRefreshTokenAsync(
                clientId,
                token,
                asNoTracking: true);

            Assert.True(DateTime.UtcNow < clientRefreshTokenBefore.Expires);

            Thread.Sleep(1);
            var clientRefreshTokenAfter = await _repo.RevokeTokenAsync(clientId, token);

            Assert.False(DateTime.UtcNow < clientRefreshTokenAfter.Expires);
        }

        [Theory]
        [MemberData(nameof(Helper.ClientsId), MemberType = typeof(Helper))]
        public async Task SoftDeleteAsync_Valid(int id)
        {
            var client = await _repo.SoftDeleteAsync(id);

            Assert.NotNull(client);
            Assert.False(client.Enabled);

            var clientInDb = await _repo.GetClientAsync(id);

            Assert.NotNull(clientInDb);
        }
    }
}