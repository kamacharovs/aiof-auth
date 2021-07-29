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
            Assert.NotNull(user.Role);
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

            Assert.NotNull(user);
            Assert.True(repo.Check(user.Password, newPassword));
        }
        [Theory]
        [MemberData(nameof(Helper.UsersId), MemberType = typeof(Helper))]
        public async Task UpdatePasswordAsync_WithTenant_IncorrectPassword_Throws_400(int id)
        {
            var serviceHelper = new ServiceHelper() { UserId = id };
            var repo = serviceHelper.GetRequiredService<IUserRepository>();
            var tenant = serviceHelper.GetRequiredService<ITenant>();

            var exception = await Assert.ThrowsAsync<AuthFriendlyException>(() => repo.UpdatePasswordAsync(
                tenant,
                "completelyfakepassword",
                "6j1mWDopz8@"));

            Assert.Equal(400, exception.StatusCode);
            Assert.Contains("incorrect", exception.Message, StringComparison.InvariantCultureIgnoreCase);
        }
        [Theory]
        [MemberData(nameof(Helper.UsersIdPassword), MemberType = typeof(Helper))]
        public async Task UpdatePasswordAsync_WithTenant_NotFound_Throws_404(int id, string password)
        {
            var serviceHelper = new ServiceHelper() { UserId = id * 100 };
            var repo = serviceHelper.GetRequiredService<IUserRepository>();
            var tenant = serviceHelper.GetRequiredService<ITenant>();

            var exception = await Assert.ThrowsAsync<AuthNotFoundException>(() => repo.UpdatePasswordAsync(
                tenant,
                password + "notfound",
                "6j1mWDopz8@"));

            Assert.Equal(404, exception.StatusCode);
            Assert.Contains("not found", exception.Message, StringComparison.InvariantCultureIgnoreCase);
        }

        [Theory]
        [MemberData(nameof(Helper.UsersEmailPassword), MemberType = typeof(Helper))]
        public async Task UpdatePasswordUnauthenticatedAsync_IsSuccessful(string email, string password)
        {
            var serviceHelper = new ServiceHelper();
            var repo = serviceHelper.GetRequiredService<IUserRepository>();

            var newPassword = "6j1mWDopz8@";
            var user = await repo.UpdatePasswordAsync(
                email,
                password,
                newPassword);

            Assert.NotNull(user);
            Assert.True(repo.Check(user.Password, newPassword));
        }
        [Theory]
        [MemberData(nameof(Helper.UsersEmailPassword), MemberType = typeof(Helper))]
        public async Task UpdatePasswordUnauthenticatedAsync_BadRequest_Throws_400(string email, string password)
        {
            var serviceHelper = new ServiceHelper();
            var repo = serviceHelper.GetRequiredService<IUserRepository>();

            var exception = await Assert.ThrowsAsync<AuthFriendlyException>(() => repo.UpdatePasswordAsync(
                email,
                password + "incorrect",
                ""));

            Assert.Equal(400, exception.StatusCode);
            Assert.Contains("incorrect", exception.Message, StringComparison.InvariantCultureIgnoreCase);
        }
        [Theory]
        [MemberData(nameof(Helper.UsersEmailPassword), MemberType = typeof(Helper))]
        public async Task UpdatePasswordUnauthenticatedAsync_NotFound_Throws_404(string email, string password)
        {
            var serviceHelper = new ServiceHelper();
            var repo = serviceHelper.GetRequiredService<IUserRepository>();

            var exception = await Assert.ThrowsAsync<AuthNotFoundException>(() => repo.UpdatePasswordAsync(
                email + "not found",
                password,
                "6j1mWDopz8@"));

            Assert.Equal(404, exception.StatusCode);
            Assert.Contains("not found", exception.Message, StringComparison.InvariantCultureIgnoreCase);
        }

        [Theory]
        [MemberData(nameof(Helper.RandomPasswords), MemberType = typeof(Helper))]
        public void Hash_IsSuccessful(string password)
        {
            var repo = new ServiceHelper().GetRequiredService<IUserRepository>();

            var hashedPassword = repo.Hash(password);

            Assert.NotNull(hashedPassword);
            Assert.True(hashedPassword.Length > 0);
        }

        [Theory]
        [MemberData(nameof(Helper.RandomPasswords), MemberType = typeof(Helper))]
        public void Check_IsSuccessful(string password)
        {
            var repo = new ServiceHelper().GetRequiredService<IUserRepository>();

            var hashedPassword = repo.Hash(password);
            var check = repo.Check(hashedPassword, password);

            Assert.True(check);
        }
        [Theory]
        [MemberData(nameof(Helper.RandomPasswords), MemberType = typeof(Helper))]
        public void Check_InvalidFormat_Throws_FormatException(string password)
        {
            var repo = new ServiceHelper().GetRequiredService<IUserRepository>();

            var check = Assert.Throws<FormatException>(() => repo.Check($"incorrectpassword", password));

            Assert.Contains("unexpected hash format", check.Message, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}
