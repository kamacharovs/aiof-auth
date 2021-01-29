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
        public async Task GenerateToken_WithValidUser_IsSuccessful(int id)
        {
            var user = await _userRepo.GetAsync(id);

            var token = _repo.GenerateJwtToken(user);

            Assert.NotNull(token);
            Assert.True(token.AccessToken.Length > 10);
        }

        [Theory]
        [MemberData(nameof(Helper.UsersEmailPassword), MemberType = typeof(Helper))]
        public async Task AuthUser_WithEmailPassword_IsSuccessful(string email, string password)
        {
            var req = new TokenRequest
            {
                Email = email,
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
        public async Task AuthClient_WithApiKey_IsSuccessful(string apiKey)
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
        public async Task AuthClient_WithRefreshToken_IsSuccessful(string refreshToken)
        {
            var req = new TokenRequest { Token = refreshToken };
            var token = await _repo.GetTokenAsync(req);

            Assert.NotNull(token);
            Assert.NotNull(token.AccessToken);
            Assert.Equal(_envConfig.JwtRefreshExpires, token.ExpiresIn);
            Assert.Equal(_envConfig.JwtType, token.TokenType);
        }

        [Fact]
        public async Task Auth_WithEmptyCredentials_ThrowsAuthValidationException()
        {
            var req = new TokenRequest { };

            await Assert.ThrowsAsync<ValidationException>(() => _repo.GetTokenAsync(req));
        }

        [Fact]
        public void GetRsaKey_Public()
        {
            var rsaSecKey = _repo.GetRsaKey(RsaKeyType.Public);

            Assert.NotNull(rsaSecKey);
            Assert.Equal("RSA", rsaSecKey.Rsa.SignatureAlgorithm);
        }
        
        [Theory]
        [MemberData(nameof(Helper.UsersId), MemberType = typeof(Helper))]
        public async Task ValidateToken_WithValidUser_IsSuccessful(int id)
        {
            var user = await _userRepo.GetAsync(id);

            var token = _repo.GenerateJwtToken(user);
            var tokenValidation = _repo.ValidateToken(token.AccessToken);

            Assert.NotNull(tokenValidation);
            Assert.True(tokenValidation.IsAuthenticated);
        }

        [Theory]
        [MemberData(nameof(Helper.ClientRefreshClientIdToken), MemberType = typeof(Helper))]
        public async Task RevokeTokenAsync_IsSuccessful(
            int clientId, 
            string token)
        {
            var revokedTokenResp = await _repo.RevokeTokenAsync(token, clientId: clientId);

            Assert.NotNull(revokedTokenResp);
            Assert.Equal(token, revokedTokenResp.Token);
            Assert.NotNull(revokedTokenResp.Revoked);
        }

        
        [Theory]
        [MemberData(nameof(Helper.ClientsApiKey), MemberType = typeof(Helper))]
        public async Task ValidateClientToken_IsAuthenticated(string apiKey)
        {
            var tokenReq = new TokenRequest { ApiKey = apiKey };
            var token = await _repo.GetTokenAsync(tokenReq);
            var validationReq = new ValidationRequest { AccessToken = token.AccessToken };
            var validation = _repo.ValidateToken(validationReq.AccessToken);

            Assert.NotNull(validation);
            Assert.True(validation.IsAuthenticated);
            Assert.Equal(TokenResultStatus.Valid.ToString(), validation.Status);
        }
        [Theory]
        [MemberData(nameof(Helper.UsersEmailPassword), MemberType = typeof(Helper))]
        public async Task ValidateUserToken_IsAuthenticated(string email, string password)
        {
            var tokenReq = new TokenRequest { Email = email, Password = password };
            var token = await _repo.GetTokenAsync(tokenReq);
            var validationReq = new ValidationRequest { AccessToken = token.AccessToken };
            var validation = _repo.ValidateToken(validationReq.AccessToken);

            Assert.NotNull(validation);
            Assert.True(validation.IsAuthenticated);
            Assert.Equal(TokenResultStatus.Valid.ToString(), validation.Status);
        }
        [Fact]
        public void ValidateToken_Expired()
        {
            Assert.Throws<AuthFriendlyException>(() => _repo.ValidateToken(Helper.ExpiredJwtToken));
        }
        [Fact]
        public void ValidateClientToken_Expired_ThrowsUnauthorized()
        {
            var token = Helper.ExpiredJwtToken;
            var validationReq = new ValidationRequest { AccessToken = token };

            Assert.Throws<AuthFriendlyException>(() => _repo.ValidateToken(validationReq.AccessToken));
        }

        [Fact]
        public void GetPublicJsonWebKey_IsSuccessful()
        {
            var jwk = _repo.GetPublicJsonWebKey();

            Assert.NotNull(jwk);
            Assert.Equal(AiofClaims.Sig, jwk.Use);
        }

        [Theory]
        [InlineData("testhost", true)]
        [InlineData("aiof-auth", true)]
        [InlineData("aiof-auth-dev", true)]
        public void GetOpenIdConfig_IsSuccessful(string host, bool isHttps)
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