using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Xunit;

using aiof.auth.data;
using aiof.auth.services;

namespace aiof.auth.tests
{
    public class UserRepositoryTests
    {
        private readonly IUserRepository _repo;

        public UserRepositoryTests()
        {
            _repo = Helper.GetRequiredService<IUserRepository>() ?? throw new ArgumentNullException(nameof(IUserRepository));
        }

        [Theory]
        [MemberData(nameof(Helper.UsersIdApiKey), MemberType=typeof(Helper))]
        public async Task GetUserAsync_By_Id_ApiKey(int id, string apiKey)
        {
            var user = await _repo.GetUserAsync(id, apiKey);

            Assert.NotNull(user);
            Assert.NotNull(user.FirstName);
            Assert.NotNull(user.LastName);
            Assert.NotNull(user.PrimaryApiKey);
            Assert.NotNull(user.SecondaryApiKey);
        }

        [Theory]
        [MemberData(nameof(Helper.UsersApiKey), MemberType=typeof(Helper))]
        public async Task GetUserAsync_By_ApiKey(string apiKey)
        {
            var user = await _repo.GetUserAsync(apiKey);

            Assert.NotNull(user);
            Assert.NotNull(user.FirstName);
        }

        [Theory]
        [MemberData(nameof(Helper.UsersApiKey), MemberType=typeof(Helper))]
        public async Task GetUserAsPublicKeyId_By_ApiKey(string apiKey)
        {
            var user = await _repo.GetUserAsPublicKeyId(apiKey);

            Assert.NotEqual(0, user.Id);
            Assert.NotEqual(Guid.Empty, user.PublicKey);
        }

        [Theory]
        [InlineData(333)]
        [InlineData(555)]
        [InlineData(999)]
        public async Task GetUserAsync_By_Id_NotFound(int id)
        {
            await Assert.ThrowsAnyAsync<AuthNotFoundException>(() => _repo.GetUserAsync(id));
        }

        [Theory]
        [InlineData("maybeanapikey")]
        [InlineData("notanapikey")]
        [InlineData("")]
        public async Task GetUserAsync_By_ApiKey_NotFound(string apiKey)
        {
            await Assert.ThrowsAnyAsync<AuthNotFoundException>(() => _repo.GetUserAsync(apiKey));
        }

        [Fact]
        public async Task AddUserAsync_Valid()
        {
            var userDto = new UserDto
            {
                FirstName = "Test",
                LastName = "McTest",
                Email = "notanemail@email.com",
                Username = "test.mctest",
                Password = "123456"
            };

            var user = await _repo.AddUserAsync(userDto);

            Assert.NotNull(user);
            Assert.NotNull(user.FirstName);
            Assert.NotNull(user.LastName);
            Assert.NotNull(user.PrimaryApiKey);
            Assert.NotNull(user.SecondaryApiKey);
            Assert.NotNull(user.Password);
        }

        [Fact]
        public async Task AddUserAsync_Check_Password_Hash()
        {
            var password = "password123";
            var userDto = new UserDto
            {
                FirstName = "Test",
                LastName = "McTest",
                Email = "notanemail@email.com",
                Username = "test.mctest",
                Password = password
            };

            var user = await _repo.AddUserAsync(userDto);

            Assert.True(_repo.Check(user.Password, password).Verified);
        }

        [Theory]
        [MemberData(nameof(Helper.UsersApiKey), MemberType=typeof(Helper))]
        public async Task GetUserTokenAsync_Valid(string apiKey)
        {
            var userToken = await _repo.GetUserTokenAsync(apiKey);

            Assert.NotNull(userToken);
        }

        [Theory]
        [MemberData(nameof(Helper.UsersUsernamePassword), MemberType=typeof(Helper))]
        public async Task UpdateUserPasswordAsync_Is_Successful(string username, string password)
        {
            var newPassword = "newpassword123";

            var user = await _repo.UpdateUserPasswordAsync(
                username,
                password, 
                newPassword);

            Assert.True(_repo.Check(user.Password, newPassword).Verified);
        }
    }
}
