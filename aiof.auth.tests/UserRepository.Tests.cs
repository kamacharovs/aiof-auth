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
        [Theory]
        [MemberData(nameof(Helper.UsersId), MemberType = typeof(Helper))]
        public async Task GetAsync_ByTenant_IsSuccessful(int id)
        {
            var serviceHelper = new ServiceHelper() { UserId = id };
            var repo = serviceHelper.GetRequiredService<IUserRepository>();
            var tenant = serviceHelper.GetRequiredService<ITenant>();

            var user = await repo.GetAsync(tenant);

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
            var serviceHelper = new ServiceHelper() { UserId = id };
            var repo = serviceHelper.GetRequiredService<IUserRepository>();
            var tenant = serviceHelper.GetRequiredService<ITenant>();

            await Assert.ThrowsAnyAsync<AuthNotFoundException>(() => repo.GetAsync(tenant));
        }

        [Theory]
        [InlineData(333)]
        [InlineData(555)]
        [InlineData(999)]
        public async Task GetAsync_ById_NotFound(int id)
        {
            var serviceHelper = new ServiceHelper() { UserId = id };
            var repo = serviceHelper.GetRequiredService<IUserRepository>();

            await Assert.ThrowsAnyAsync<AuthNotFoundException>(() => repo.GetAsync(id));
        }

        [Theory]
        [MemberData(nameof(Helper.UsersPublicKey), MemberType = typeof(Helper))]
        public async Task GetAsync_ByPublicKey_IsSuccessful(Guid publicKey)
        {
            var serviceHelper = new ServiceHelper() { PublicKey = publicKey };
            var repo = serviceHelper.GetRequiredService<IUserRepository>();

            var user = await repo.GetAsync(publicKey);

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
            var serviceHelper = new ServiceHelper();
            var repo = serviceHelper.GetRequiredService<IUserRepository>();

            var userDto = new UserDto
            {
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                Password = password
            };

            var user = await repo.GetAsync(userDto);

            Assert.Null(user);

            var user2 = await repo.GetAsync(
                firstName,
                lastName,
                email);

            Assert.Null(user2);
        }

        [Theory]
        [MemberData(nameof(Helper.UserRefreshTokensToken), MemberType = typeof(Helper))]
        public async Task GetAsync_ByRefreshToken_IsSuccessful(string refreshToken)
        {
            var repo = new ServiceHelper().GetRequiredService<IUserRepository>();

            var user = await repo.GetByRefreshTokenAsync(refreshToken);

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
            var repo = new ServiceHelper() { UserId = userId }.GetRequiredService<IUserRepository>();

            var refreshToken = await repo.GetRefreshTokenAsync(userId);

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
            var repo = new ServiceHelper() { UserId = userId }.GetRequiredService<IUserRepository>();

            var refreshTokens = await repo.GetRefreshTokensAsync(userId);

            Assert.NotNull(refreshTokens);
            Assert.NotEmpty(refreshTokens);
        }
        [Theory]
        [MemberData(nameof(Helper.UserRefreshTokensUserId), MemberType = typeof(Helper))]
        public async Task GetOrAddRefreshTokenAsync_NotRevoked_IsSuccessful(int userId)
        {
            var repo = new ServiceHelper() { UserId = userId }.GetRequiredService<IUserRepository>();

            var refreshToken = await repo.GetOrAddRefreshTokenAsync(userId);

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
            var repo = new ServiceHelper() { UserId = userId }.GetRequiredService<IUserRepository>();

            var revokedToken = await repo.RevokeTokenAsync(userId, token);

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
            var repo = new ServiceHelper().GetRequiredService<IUserRepository>();

            var userDto = new UserDto
            {
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                Password = password
            };

            var user = await repo.AddAsync(userDto);

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
            var repo = new ServiceHelper().GetRequiredService<IUserRepository>();

            var password = "Password123";
            var userDto = new UserDto
            {
                FirstName = "Test",
                LastName = "McTest",
                Email = "notanemail@email.com",
                Password = password
            };

            var user = await repo.AddAsync(userDto);

            Assert.True(repo.Check(user.Password, password));
        }

        [Theory]
        [MemberData(nameof(Helper.UsersIdPassword), MemberType = typeof(Helper))]
        public async Task UpdatePasswordAsync_WithTenant_IsSuccessful(int id, string password)
        {
            var serviceHelper = new ServiceHelper() { UserId = id };
            var repo = serviceHelper.GetRequiredService<IUserRepository>();
            var tenant = serviceHelper.GetRequiredService<ITenant>();

            var newPassword = "6j1mWDopz8@";
            var user = await repo.UpdatePasswordAsync(
                tenant,
                password,
                newPassword);

            Assert.True(repo.Check(user.Password, newPassword));
        }
        [Theory]
        [MemberData(nameof(Helper.UsersId), MemberType = typeof(Helper))]
        public async Task UpdatePasswordAsync_WithTenant_IncorrectPassword_ThrowsBadRequest(int id)
        {
            var serviceHelper = new ServiceHelper() { UserId = id };
            var repo = serviceHelper.GetRequiredService<IUserRepository>();
            var tenant = serviceHelper.GetRequiredService<ITenant>();

            await Assert.ThrowsAsync<AuthFriendlyException>(() => repo.UpdatePasswordAsync(
                tenant,
                "completelyfakepassword",
                "6j1mWDopz8@"));
        }
    }
}
