using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

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
            _repo = new ServiceHelper().GetRequiredService<IUtilRepository>() ?? throw new ArgumentNullException(nameof(IUtilRepository));
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
            Assert.NotEqual(0, role.Id);
            Assert.NotEqual(Guid.Empty, role.PublicKey);
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
            Assert.NotEqual(0, role.Id);
            Assert.NotEqual(Guid.Empty, role.PublicKey);
            Assert.Equal(role.Name, Roles.Client);
        }

        [Theory]
        [InlineData(null)]
        [InlineData(77)]
        [InlineData(88)]
        [InlineData(99)]
        public async Task GetRoleAsync_Non_UserOrClient_Defaults(int? id)
        {
            var role = await _repo.GetRoleAsync<Helper.TestPublicKeyId>(id);

            Assert.NotNull(role);
            Assert.NotEqual(0, role.Id);
            Assert.NotEqual(Guid.Empty, role.PublicKey);
            Assert.Equal(Roles.Basic, role.Name);
        }

        [Fact]
        public async Task GetRoleIdAsync_User_IsSuccessful()
        {
            var roleId = await _repo.GetRoleIdAsync<User>();

            Assert.NotEqual(0, roleId);
        }

        [Fact]
        public async Task GetRoleIdAsync_Client_IsSuccessful()
        {
            var roleId = await _repo.GetRoleIdAsync<Client>();

            Assert.NotEqual(0, roleId);
        }

        [Fact]
        public async Task GetRoleIdAsync_NonUserClient_Defaults()
        {
            var roleId = await _repo.GetRoleIdAsync<Role>();

            Assert.NotEqual(0, roleId);
        }

        [Theory]
        [InlineData("Test")]
        [InlineData("AdminUser")]
        [InlineData("Administrator")]
        public async Task QuickAddRoleAsync_IsSuccessful(string name)
        {
            var role = await _repo.QuickAddRoleAsync(name);

            Assert.NotNull(role);
            Assert.NotEqual(0, role.Id);
            Assert.NotEqual(Guid.Empty, role.PublicKey);
            Assert.Equal(name, role.Name);
        }

        [Theory]
        [MemberData(nameof(Helper.RoleNames), MemberType = typeof(Helper))]
        public async Task QuickAddRoleAsync_ExistingRoles_IsSuccessful(string name)
        {
            var role = await _repo.QuickAddRoleAsync(name);

            Assert.NotNull(role);
            Assert.NotEqual(0, role.Id);
            Assert.NotEqual(Guid.Empty, role.PublicKey);
            Assert.Equal(name, role.Name);
        }
    }
}