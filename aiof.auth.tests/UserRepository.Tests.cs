using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Xunit;

using aiof.auth.data;
using aiof.auth.services;

namespace aiof.auth.tests
{
    [Trait(Helper.Category, Helper.UnitTest)]
    public class UserRepositoryTests
    {
        private readonly IUserRepository _repo;

        public UserRepositoryTests()
        {
            _repo = Helper.GetRequiredService<IUserRepository>() ?? throw new ArgumentNullException(nameof(IUserRepository));
        }

        [Theory]
        [MemberData(nameof(Helper.UsersId), MemberType = typeof(Helper))]
        public async Task GetAsync_ByTenant_IsSuccessful(int id)
        {
            var user = await _repo.GetAsync(Helper.GetMockedTenant(userId: id));

            Assert.NotNull(user);
            Assert.NotNull(user.FirstName);
            Assert.NotNull(user.LastName);
            Assert.NotNull(user.Email);
            Assert.NotNull(user.Password);
            Assert.NotEqual(0, user.RoleId);
            Assert.NotEqual(new DateTime(), user.Created);
            Assert.False(user.IsDeleted);
        }

        [Theory]
        [InlineData(333)]
        [InlineData(555)]
        [InlineData(999)]
        public async Task GetAsync_ByTenant_NotFound_Throws(int id)
        {
            await Assert.ThrowsAnyAsync<AuthNotFoundException>(() => _repo.GetAsync(Helper.GetMockedTenant(userId: id)));
        }

        [Theory]
        [InlineData(333)]
        [InlineData(555)]
        [InlineData(999)]
        public async Task GetAsync_ById_NotFound(int id)
        {
            await Assert.ThrowsAnyAsync<AuthNotFoundException>(() => _repo.GetAsync(id));
        }

        [Theory]
        [MemberData(nameof(Helper.UsersPublicKey), MemberType = typeof(Helper))]
        public async Task GetAsync_ByPublicKey_IsSuccessful(Guid publicKey)
        {
            var user = await _repo.GetAsync(publicKey);

            Assert.NotNull(user);
            Assert.NotNull(user.FirstName);
            Assert.NotNull(user.LastName);
            Assert.NotNull(user.Email);
            Assert.NotNull(user.Password);
            Assert.NotEqual(0, user.RoleId);
            Assert.NotEqual(new DateTime(), user.Created);
            Assert.False(user.IsDeleted);
        }

        [Theory]
        [MemberData(nameof(Helper.RandomUserDtos), MemberType = typeof(Helper))]
        public async Task GetAsync_ByUserDto_NotFound(
            string firstName,
            string lastName,
            string email,
            string password)
        {
            var userDto = new UserDto
            {
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                Password = password
            };

            var user = await _repo.GetAsync(userDto);

            Assert.Null(user);

            var user2 = await _repo.GetAsync(
                firstName,
                lastName,
                email);

            Assert.Null(user2);
        }

        [Theory]
        [MemberData(nameof(Helper.UserRefreshTokensToken), MemberType = typeof(Helper))]
        public async Task GetAsync_ByRefreshToken_IsSuccessful(string refreshToken)
        {
            var user = await _repo.GetByRefreshTokenAsync(refreshToken);

            Assert.NotNull(user);
            Assert.NotNull(user.FirstName);
            Assert.NotNull(user.LastName);
            Assert.NotNull(user.Email);
            Assert.NotNull(user.Password);
            Assert.NotEqual(new DateTime(), user.Created);
        }
        [Theory]
        [MemberData(nameof(Helper.UserRefreshTokensUserId), MemberType = typeof(Helper))]
        public async Task GetRefreshTokenAsync_ById_IsSuccessful(int userId)
        {
            var refreshToken = await _repo.GetRefreshTokenAsync(userId);

            Assert.NotNull(refreshToken);
            Assert.NotNull(refreshToken.Token);
            Assert.Equal(userId, refreshToken.UserId);
            Assert.NotEqual(new DateTime(), refreshToken.Created);
            Assert.NotEqual(new DateTime(), refreshToken.Expires);
            Assert.Null(refreshToken.Revoked);
        }
        [Theory]
        [MemberData(nameof(Helper.UserRefreshTokensUserId), MemberType = typeof(Helper))]
        public async Task GetRefreshTokensAsync_IsSuccessful(int userId)
        {
            var refreshTokens = await _repo.GetRefreshTokensAsync(userId);

            Assert.NotNull(refreshTokens);
            Assert.NotEmpty(refreshTokens);
        }
        [Theory]
        [MemberData(nameof(Helper.UserRefreshTokensUserId), MemberType = typeof(Helper))]
        public async Task GetOrAddRefreshTokenAsync_NotRevoked_IsSuccessful(int userId)
        {
            var refreshToken = await _repo.GetOrAddRefreshTokenAsync(userId);

            Assert.NotNull(refreshToken);
            Assert.NotNull(refreshToken.Token);
            Assert.Equal(userId, refreshToken.UserId);
            Assert.NotEqual(new DateTime(), refreshToken.Created);
            Assert.NotEqual(new DateTime(), refreshToken.Expires);
            Assert.Null(refreshToken.Revoked);
        }
        [Theory]
        [MemberData(nameof(Helper.UserRefreshTokensUserIdToken), MemberType = typeof(Helper))]
        public async Task RevokeTokenAsync_IsSuccessful(int userId, string token)
        {
            var revokedToken = await _repo.RevokeTokenAsync(userId, token);

            Assert.NotNull(revokedToken);
            Assert.NotNull(revokedToken.Revoked);
        }

        [Theory]
        [MemberData(nameof(Helper.RandomUserDtos), MemberType = typeof(Helper))]
        public async Task AddAsync_IsSuccessful(
            string firstName,
            string lastName,
            string email,
            string password)
        {
            var userDto = new UserDto
            {
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                Password = password
            };

            var user = await _repo.AddAsync(userDto);

            Assert.NotNull(user);
            Assert.NotNull(user.FirstName);
            Assert.NotNull(user.LastName);
            Assert.NotNull(user.Email);
            Assert.NotNull(user.Password);
            Assert.NotEqual(new DateTime(), user.Created);
        }

        [Fact]
        public async Task AddAsync_CheckPasswordHash_IsSuccessful()
        {
            var password = "Password123";
            var userDto = new UserDto
            {
                FirstName = "Test",
                LastName = "McTest",
                Email = "notanemail@email.com",
                Password = password
            };

            var user = await _repo.AddAsync(userDto);

            Assert.True(_repo.Check(user.Password, password));
        }

        [Theory]
        [MemberData(nameof(Helper.UsersEmailPassword), MemberType = typeof(Helper))]
        public async Task UpdatePasswordAsync_IsSuccessful(string email, string password)
        {
            var newPassword = "newpassword123";

            var user = await _repo.UpdatePasswordAsync(
                email,
                password,
                newPassword);

            Assert.True(_repo.Check(user.Password, newPassword));
        }
    }
}
