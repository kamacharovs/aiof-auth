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

        [Theory]
        [MemberData(nameof(Helper.UsersUsernamePassword), MemberType=typeof(Helper))]
        public async Task AuthUser_With_Username_Password(string username, string password)
        {
            var req = new TokenRequest 
            { 
                Username = username,
                Password = password
            };
            var token = await _repo.GetTokenAsync(req);

            Assert.NotNull(token);
            Assert.NotNull(token.AccessToken);
            Assert.True(token.ExpiresIn > 0);
            Assert.Equal("Bearer", token.TokenType);
        }

        [Theory]
        [MemberData(nameof(Helper.ClientsApiKey), MemberType=typeof(Helper))]
        public async Task AuthClient_With_ApiKey(string apiKey)
        {
            var req = new TokenRequest { ApiKey = apiKey };
            var token = await _repo.GetTokenAsync(req);

            Assert.NotNull(token);
            Assert.NotNull(token.AccessToken);
            Assert.NotNull(token.RefreshToken);
            Assert.True(token.ExpiresIn > 0);
            Assert.Equal("Bearer", token.TokenType);
        }
    }
}