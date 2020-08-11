using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Xunit;
using FluentValidation;

using aiof.auth.data;
using aiof.auth.services;

namespace aiof.auth.tests
{
    [Trait(Helper.Category, Helper.UnitTest)]
    public class AuthRepositoryTests
    {
        private readonly IAuthRepository _repo;
        private readonly IUserRepository _userRepo;
        private readonly IClientRepository _clientRepo;
        private readonly IEnvConfiguration _envConfig;

        public AuthRepositoryTests()
        {
            _repo = Helper.GetRequiredService<IAuthRepository>() ?? throw new ArgumentNullException(nameof(IAuthRepository));
            _userRepo = Helper.GetRequiredService<IUserRepository>() ?? throw new ArgumentNullException(nameof(IUserRepository));
            _clientRepo = Helper.GetRequiredService<IClientRepository>() ?? throw new ArgumentNullException(nameof(IClientRepository));
            _envConfig = Helper.GetRequiredService<IEnvConfiguration>() ?? throw new ArgumentNullException(nameof(IEnvConfiguration));
        }

        [Theory]
        [MemberData(nameof(Helper.UsersId), MemberType = typeof(Helper))]
        public async Task GenerateToken_With_Valid_User(int id)
        {
            var user = await _userRepo.GetUserAsync(id);

            var token = _repo.GenerateJwtToken(user);

            Assert.NotNull(token);
            Assert.True(token.AccessToken.Length > 10);
        }

        [Theory]
        [MemberData(nameof(Helper.UsersUsernamePassword), MemberType = typeof(Helper))]
        public async Task Auth_User_With_Username_Password(string username, string password)
        {
            var req = new TokenRequest
            {
                Username = username,
                Password = password
            };
            var token = await _repo.GetTokenAsync(req);

            Assert.NotNull(token);
            Assert.NotNull(token.AccessToken);
            Assert.Equal(_envConfig.JwtExpires, token.ExpiresIn);
            Assert.Equal("Bearer", token.TokenType);
        }

        [Theory]
        [MemberData(nameof(Helper.ClientsApiKey), MemberType = typeof(Helper))]
        public async Task Auth_Client_With_ApiKey(string apiKey)
        {
            var req = new TokenRequest { ApiKey = apiKey };
            var token = await _repo.GetTokenAsync(req);

            Assert.NotNull(token);
            Assert.NotNull(token.AccessToken);
            Assert.NotNull(token.RefreshToken);
            Assert.Equal(_envConfig.JwtExpires, token.ExpiresIn);
            Assert.Equal(_envConfig.JwtType, token.TokenType);
        }

        [Theory]
        [MemberData(nameof(Helper.ClientRefreshToken), MemberType = typeof(Helper))]
        public async Task Auth_Client_With_RefreshToken(string refreshToken)
        {
            var req = new TokenRequest { Token = refreshToken };
            var token = await _repo.GetTokenAsync(req);

            Assert.NotNull(token);
            Assert.NotNull(token.AccessToken);
            Assert.Equal(_envConfig.JwtRefreshExpires, token.ExpiresIn);
            Assert.Equal(_envConfig.JwtType, token.TokenType);
        }

        [Fact]
        public async Task Auth_With_EmptyCredentials_Throws_AuthValidationException()
        {
            var req = new TokenRequest { };

            await Assert.ThrowsAsync<ValidationException>(() => _repo.GetTokenAsync(req));
        }
        
        [Theory]
        [MemberData(nameof(Helper.UsersId), MemberType = typeof(Helper))]
        public async Task ValidateToken_With_Valid_User(int id)
        {
            var user = await _userRepo.GetUserAsync(id);

            var token = _repo.GenerateJwtToken(user);
            var tokenValidation = _repo.ValidateToken<User>(token.AccessToken);

            Assert.NotNull(tokenValidation);
            Assert.True(tokenValidation.IsAuthenticated);
        }

        [Theory]
        [MemberData(nameof(Helper.ClientRefreshClientIdToken), MemberType = typeof(Helper))]
        public async Task RevokeTokenAsync(
            int clientId, 
            string token)
        {
            var revokedTokenResp = await _repo.RevokeTokenAsync(clientId, token);

            Assert.NotNull(revokedTokenResp);
            Assert.Equal(clientId, revokedTokenResp.ClientId);
            Assert.Equal(token, revokedTokenResp.Token);
            Assert.NotNull(revokedTokenResp.Revoked);
        }

        [Fact]
        public void ValidateToken_Expired()
        {
            Assert.Throws<AuthFriendlyException>(() => _repo.ValidateToken<Client>(Helper.ExpiredJwtToken));
        }

        [Theory]
        [MemberData(nameof(Helper.ClientsApiKey), MemberType = typeof(Helper))]
        public async Task ValidateToken_IsAuthenticated(string apiKey)
        {
            var tokenReq = new TokenRequest { ApiKey = apiKey };
            var token = await _repo.GetTokenAsync(tokenReq);
            var validationReq = new ValidationRequest { AccessToken = token.AccessToken };
            var validation = _repo.ValidateToken(validationReq);

            Assert.NotNull(validation);
            Assert.True(validation.IsAuthenticated);
            Assert.Equal(TokenResultStatus.Valid.ToString(), validation.Status);
        }
        [Fact]
        public void ValidateToken_Expired_ThrowsUnauthorized()
        {
            var token = Helper.ExpiredJwtToken;
            var validationReq = new ValidationRequest { AccessToken = token };

            Assert.Throws<AuthFriendlyException>(() => _repo.ValidateToken(validationReq));
        }

        [Fact]
        public void GetPublicJsonWebKey_Valid()
        {
            var jwk = _repo.GetPublicJsonWebKey();

            Assert.NotNull(jwk);
            Assert.Equal(AiofClaims.Sig, jwk.Use);
        }

        [Theory]
        [InlineData("testhost", true)]
        [InlineData("aiof-auth", true)]
        [InlineData("aiof-auth-dev", true)]
        public void GetOpenIdConfig_Valid(string host, bool isHttps)
        {
            var openIdConfig = _repo.GetOpenIdConfig(
                host,
                isHttps);

            Assert.NotNull(openIdConfig);
            Assert.NotNull(openIdConfig.Issuer);
            Assert.NotNull(openIdConfig.TokenEndpoint);
            Assert.NotNull(openIdConfig.TokenRefreshEndpoint);
            Assert.NotEmpty(openIdConfig.ResponseTypes);
            Assert.NotEmpty(openIdConfig.SubjectTypesSupported);
            Assert.NotEmpty(openIdConfig.SigningAlgorithmsSupported);
            Assert.NotEmpty(openIdConfig.ClaimTypesSupported);
            Assert.NotEmpty(openIdConfig.ClaimsSupported);
        }
    }
}