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
        [Theory]
        [MemberData(nameof(Helper.ClientsId), MemberType = typeof(Helper))]
        public async Task GetAsync_ById_IsSuccessful(int id)
        {
            var repo = new ServiceHelper() { ClientId = id }.GetRequiredService<IClientRepository>();

            var client = await repo.GetAsync(id);

            Assert.NotNull(client);
            Assert.Equal(id, client.Id);
            Assert.NotNull(client.PrimaryApiKey);
            Assert.NotNull(client.SecondaryApiKey);
            Assert.NotNull(client.Slug);
            Assert.NotEqual(0, client.RoleId);
            Assert.NotNull(client.Role);
            Assert.NotEqual(new DateTime(), client.Created);
        }

        [Theory]
        [MemberData(nameof(Helper.ClientsApiKey), MemberType = typeof(Helper))]
        public async Task GetAsync_ByApiKey_IsSuccessful(string apiKey)
        {
            var repo = new ServiceHelper().GetRequiredService<IClientRepository>();

            var client = await repo.GetAsync(apiKey);

            Assert.NotNull(client);
            Assert.NotNull(client.PrimaryApiKey);
            Assert.NotNull(client.SecondaryApiKey);
            Assert.NotNull(client.Slug);
            Assert.NotEqual(0, client.RoleId);
            Assert.NotNull(client.Role);
            Assert.NotEqual(new DateTime(), client.Created);
        }

        [Theory]
        [MemberData(nameof(Helper.RandomClientDtos), MemberType = typeof(Helper))]
        public async Task AddAsync_IsSuccessful(
            string name, 
            bool enabled)
        {
            var repo = new ServiceHelper().GetRequiredService<IClientRepository>();

            var client = await repo.AddClientAsync(new ClientDto
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
            Assert.NotNull(client.Slug);
            Assert.NotEqual(0, client.RoleId);
            Assert.NotNull(client.Role);
            Assert.NotEqual(new DateTime(), client.Created);
        }

        [Theory]
        [MemberData(nameof(Helper.ClientsId), MemberType = typeof(Helper))]
        public async Task EnableClientAsync_IsSuccessful(int id)
        {
            var repo = new ServiceHelper().GetRequiredService<IClientRepository>();

            var client = await repo.EnableAsync(id);

            Assert.NotNull(client);
            Assert.True(client.Enabled);
        }

        [Theory]
        [MemberData(nameof(Helper.ClientsId), MemberType = typeof(Helper))]
        public async Task DisableClientAsync_IsSuccessful(int id)
        {
            var repo = new ServiceHelper().GetRequiredService<IClientRepository>();

            var client = await repo.DisableAsync(id);

            Assert.NotNull(client);
            Assert.False(client.Enabled);
        }

        [Theory]
        [MemberData(nameof(Helper.ClientsId), MemberType = typeof(Helper))]
        public async Task RegenerateKeysAsync_IsSuccessful(int id)
        {
            var repo = new ServiceHelper().GetRequiredService<IClientRepository>();

            var client = await repo.GetAsync(id);
            var pKey = client.PrimaryApiKey;
            var sKey = client.SecondaryApiKey;           

            Assert.NotNull(client);

            client = await repo.RegenerateKeysAsync(id);

            Assert.NotNull(client);
            Assert.NotEqual(pKey, client.PrimaryApiKey);
            Assert.NotEqual(sKey, client.SecondaryApiKey);
        }
    }
}