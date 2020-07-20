using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Xunit;

using aiof.auth.data;
using aiof.auth.services;

namespace aiof.auth.tests
{
    public class ClientRepositoryTests
    {
        private readonly IClientRepository _repo;

        public ClientRepositoryTests()
        {
            _repo = Helper.GetRequiredService<IClientRepository>() ?? throw new ArgumentNullException(nameof(IUserRepository));
        }

        [Theory]
        [MemberData(nameof(Helper.ClientDtos), MemberType=typeof(Helper))]
        public async Task AddClientAsync_Valid(string name, string slug, bool enabled)
        {
            var client = await _repo.AddClientAsync(new ClientDto
            {
                Name = name,
                Slug = slug,
                Enabled = enabled
            });

            Assert.NotNull(client);
            Assert.NotEqual(0, client.Id);
            Assert.NotEqual(Guid.Empty, client.PublicKey);
            Assert.Equal(name, client.Name);
            Assert.Equal(slug, client.Slug);
            Assert.Equal(enabled, client.Enabled);
        }
    }
}