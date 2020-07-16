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
        private readonly IUserRepository _userRepo;

        public AuthRepositoryTests()
        {
            _repo = Helper.GetRequiredService<IAuthRepository>() ?? throw new ArgumentNullException(nameof(IAuthRepository));
            _userRepo = Helper.GetRequiredService<IUserRepository>() ?? throw new ArgumentNullException(nameof(IUserRepository));
        }

        [Theory]
        [MemberData(nameof(Helper.UsersId), MemberType=typeof(Helper))]
        public async Task GenerateToken_With_Valid_User(int id)
        {
            var user = await _userRepo.GetUserAsync(id);

            var token = _repo.GenerateJwtToken(user);

            Assert.NotNull(token);
            Assert.True(token.AccessToken.Length > 10);
        }

        [Theory]
        [MemberData(nameof(Helper.UsersId), MemberType=typeof(Helper))]
        public async Task ValidateToken_With_Valid_User(int id)
        {
            var user = await _userRepo.GetUserAsync(id);

            var token = _repo.GenerateJwtToken(user);
            var tokenValidation = _repo.ValidateToken(token.AccessToken);

            Assert.NotNull(tokenValidation);
            Assert.True(tokenValidation.Principal.Identity.IsAuthenticated);
        }

        [Fact]
        public void ValidateToken_Expired()
        {
            var validation = _repo.ValidateToken(Helper.ExpiredJwtToken);

            Assert.Equal(TokenResultStatus.Expired, validation.Status);
        }

        [Fact]
        public void GenerateApiKey()
        {
            var apiKey = _repo.GenerateApiKey();

            Assert.NotNull(apiKey);
            Assert.True(apiKey.Length > 30);
        }
    }
}