using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Xunit;

using aiof.auth.data;
using aiof.auth.services;

namespace aiof.auth.tests
{
    [Trait(Helper.Category, Helper.UnitTest)]
    public class UtilRepositoryTests
    {
        private readonly IUtilRepository _repo;

        public UtilRepositoryTests()
        {
            _repo = Helper.GetRequiredService<IUtilRepository>() ?? throw new ArgumentNullException(nameof(IUtilRepository));
        }

        [Theory]
        [InlineData(2)]
        [InlineData(77)]
        [InlineData(88)]
        [InlineData(99)]
        public async Task GetRoleAsync_User_WithId_Defaults(int id)
        {
            var role = await _repo.GetRoleAsync<User>(id);

            Assert.NotNull(role);
            Assert.Equal(role.Name, Roles.User);
        }

        [Theory]
        [InlineData(3)]
        [InlineData(77)]
        [InlineData(88)]
        [InlineData(99)]
        public async Task GetRoleAsync_Client_WithId_Defaults(int id)
        {
            var role = await _repo.GetRoleAsync<Client>(id);

            Assert.NotNull(role);
            Assert.Equal(role.Name, Roles.Client);
        }
    }
}