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
    }
}
