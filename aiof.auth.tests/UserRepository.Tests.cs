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
        [InlineData(333)]
        [InlineData(555)]
        [InlineData(999)]
        public async Task GetUserAsync_By_Id_NotFound(int id)
        {
            await Assert.ThrowsAnyAsync<AuthNotFoundException>(() => _repo.GetUserAsync(id));
        }

        [Theory]
        [MemberData(nameof(Helper.UsersPublicKey), MemberType = typeof(Helper))]
        public async Task GetUserAsync_By_PublicKey(Guid publicKey)
        {
            var user = await _repo.GetUserAsync(publicKey);

            Assert.NotNull(user);
            Assert.NotNull(user.FirstName);
            Assert.NotNull(user.LastName);
            Assert.NotNull(user.Email);
            Assert.NotNull(user.Username);
            Assert.NotNull(user.Password);
            Assert.NotEqual(new DateTime(), user.Created);
        }

        [Theory]
        [MemberData(nameof(Helper.RandomUserDtos), MemberType = typeof(Helper))]
        public async Task GetUserAsync_By_UserDto_NotFound(
            string firstName,
            string lastName,
            string email,
            string username,
            string password)
        {
            var userDto = new UserDto
            {
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                Username = username,
                Password = password
            };

            var user = await _repo.GetUserAsync(userDto);

            Assert.Null(user);

            var user2 = await _repo.GetUserAsync(
                firstName,
                lastName,
                email,
                username);

            Assert.Null(user2);
        }

        [Theory]
        [MemberData(nameof(Helper.RandomUserDtos), MemberType = typeof(Helper))]
        public async Task AddUserAsync_Valid(
            string firstName,
            string lastName,
            string email,
            string username,
            string password)
        {
            var userDto = new UserDto
            {
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                Username = username,
                Password = password
            };

            var user = await _repo.AddUserAsync(userDto);

            Assert.NotNull(user);
            Assert.NotNull(user.FirstName);
            Assert.NotNull(user.LastName);
            Assert.NotNull(user.Email);
            Assert.NotNull(user.Username);
            Assert.NotNull(user.Password);
            Assert.NotEqual(new DateTime(), user.Created);
        }

        [Fact]
        public async Task AddUserAsync_Check_Password_Hash()
        {
            var password = "Password123";
            var userDto = new UserDto
            {
                FirstName = "Test",
                LastName = "McTest",
                Email = "notanemail@email.com",
                Username = "test.mctest",
                Password = password
            };

            var user = await _repo.AddUserAsync(userDto);

            Assert.True(_repo.Check(user.Password, password));
        }

        [Theory]
        [MemberData(nameof(Helper.UsersUsernamePassword), MemberType = typeof(Helper))]
        public async Task UpdateUserPasswordAsync_Is_Successful(string username, string password)
        {
            var newPassword = "newpassword123";

            var user = await _repo.UpdatePasswordAsync(
                username,
                password,
                newPassword);

            Assert.True(_repo.Check(user.Password, newPassword));
        }
    }
}
