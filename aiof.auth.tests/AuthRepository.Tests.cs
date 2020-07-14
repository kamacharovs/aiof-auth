using System;
using System.Collections.Generic;
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

        [Theory]
        [MemberData(nameof(UsersId))]
        public async Task GetUserTokenAsync_Valid(int id)
        {
            var userToken = await _repo.GetUserTokenAsync(id);

            Assert.NotNull(userToken);
        }

        [Theory]
        [MemberData(nameof(UsersIdApiKey))]
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
        [MemberData(nameof(UsersApiKey))]
        public async Task GetUserAsync_By_ApiKey(string apiKey)
        {
            var user = await _repo.GetUserAsync(apiKey);

            Assert.NotNull(user);
            Assert.NotNull(user.FirstName);
        }

        [Theory]
        [MemberData(nameof(UsersApiKey))]
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
        }

        [Fact]
        public void GenerateApiKey()
        {
            var apiKey = _repo.GenerateApiKey();

            Assert.NotNull(apiKey);
            Assert.True(apiKey.Length > 30);
        }

        [Theory]
        [MemberData(nameof(UsersId))]
        public async Task GenerateToken_With_Valid_User(int id)
        {
            var user = await _repo.GetUserAsync(id);

            var token = _repo.GenerateJwtToken(user);

            Assert.NotNull(token);
            Assert.True(token.Length > 10);
        }


        static FakeDataManager _Fake
            => Helper.GetRequiredService<FakeDataManager>() ?? throw new ArgumentNullException(nameof(FakeDataManager));

        public static IEnumerable<object[]> UsersId()
        {
            return _Fake.GetFakeUsersData(
                id: true
            );
        }

        public static IEnumerable<object[]> UsersApiKey()
        {
            return _Fake.GetFakeUsersData(
                apiKey: true
            );
        }

        public static IEnumerable<object[]> UsersIdApiKey()
        {
            return _Fake.GetFakeUsersData(
                id: true,
                apiKey: true
            );
        }
    }
}
