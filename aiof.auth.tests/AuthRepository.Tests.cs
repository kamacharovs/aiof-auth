using System;
using System.Threading.Tasks;

using Xunit;

using aiof.auth.data;
using aiof.auth.services;

namespace aiof.auth.tests
{
    public class AuthRepositoryTests
    {
        private readonly IAuthRepository _repo;

        public AuthRepositoryTests()
        {
            _repo = Helper.GetRequiredService<IAuthRepository>() ?? throw new ArgumentNullException(nameof(IAuthRepository));
        }

        [Fact]
        public void GenerateApiKey()
        {
            var apiKey = _repo.GenerateApiKey();

            Assert.NotNull(apiKey);
            Assert.True(apiKey.Length > 30);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        public async Task GetUserTokenAsync_Valid(int id)
        {
            var userToken = await _repo.GetUserTokenAsync(id);

            Assert.NotNull(userToken);
        }

        [Theory]
        [InlineData("api-key")]
        [InlineData("api-key-jbro")]
        public async Task GetUserAsync_By_ApiKey(string apiKey)
        {
            var user = await _repo.GetUserAsync(apiKey);

            Assert.NotNull(user);
            Assert.NotNull(user.FirstName);
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
        }
    }
}
