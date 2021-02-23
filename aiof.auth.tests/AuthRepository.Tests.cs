using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;

using Xunit;
using FluentValidation;

using aiof.auth.data;
using aiof.auth.services;

namespace aiof.auth.tests
{
    [Trait(Helper.Category, Helper.UnitTest)]
    public class AuthRepositoryTests
    {
        [Theory]
        [MemberData(nameof(Helper.UsersId), MemberType = typeof(Helper))]
        public async Task GenerateToken_WithValidUser_IsSuccessful(int id)
        {
            var serviceHelper = new ServiceHelper { UserId = id };
            var userRepo = serviceHelper.GetRequiredService<IUserRepository>();
            var repo = serviceHelper.GetRequiredService<IAuthRepository>();

            var user = await userRepo.GetAsync(id);

            var token = repo.GenerateJwtToken(user);

            Assert.NotNull(token);
            Assert.True(token.AccessToken.Length > 10);
        }

        [Theory]
        [MemberData(nameof(Helper.UsersEmailPassword), MemberType = typeof(Helper))]
        public async Task AuthUser_WithEmailPassword_IsSuccessful(string email, string password)
        {
            var serviceHelper = new ServiceHelper();
            var envConfig = serviceHelper.GetRequiredService<IEnvConfiguration>();
            var repo = serviceHelper.GetRequiredService<IAuthRepository>();

            var req = new TokenRequest
            {
                Email = email,
                Password = password
            };
            var token = await repo.GetTokenAsync(req);

            Assert.NotNull(token);
            Assert.NotNull(token.AccessToken);
            Assert.Equal(envConfig.JwtExpires, token.ExpiresIn);
            Assert.Equal("Bearer", token.TokenType);
        }

        [Theory]
        [MemberData(nameof(Helper.ClientsApiKey), MemberType = typeof(Helper))]
        public async Task AuthClient_WithApiKey_IsSuccessful(string apiKey)
        {
            var serviceHelper = new ServiceHelper();
            var envConfig = serviceHelper.GetRequiredService<IEnvConfiguration>();
            var repo = serviceHelper.GetRequiredService<IAuthRepository>();

            var req = new TokenRequest { ApiKey = apiKey };
            var token = await repo.GetTokenAsync(req);

            Assert.NotNull(token);
            Assert.NotNull(token.AccessToken);
            Assert.NotNull(token.RefreshToken);
            Assert.Equal(envConfig.JwtExpires, token.ExpiresIn);
            Assert.Equal(envConfig.JwtType, token.TokenType);
        }

        [Theory]
        [MemberData(nameof(Helper.ClientRefreshToken), MemberType = typeof(Helper))]
        public async Task AuthClient_WithRefreshToken_IsSuccessful(string refreshToken)
        {
            var serviceHelper = new ServiceHelper();
            var envConfig = serviceHelper.GetRequiredService<IEnvConfiguration>();
            var repo = serviceHelper.GetRequiredService<IAuthRepository>();

            var req = new TokenRequest { Token = refreshToken };
            var token = await repo.GetTokenAsync(req);

            Assert.NotNull(token);
            Assert.NotNull(token.AccessToken);
            Assert.Equal(envConfig.JwtRefreshExpires, token.ExpiresIn);
            Assert.Equal(envConfig.JwtType, token.TokenType);
        }

        [Fact]
        public async Task Auth_WithEmptyCredentials_ThrowsAuthValidationException()
        {
            var repo = new ServiceHelper().GetRequiredService<IAuthRepository>();

            var req = new TokenRequest { };

            await Assert.ThrowsAsync<ValidationException>(() => repo.GetTokenAsync(req));
        }

        [Fact]
        public void GetRsaKey_Public()
        {
            var repo = new ServiceHelper().GetRequiredService<IAuthRepository>();

            var rsaSecKey = repo.GetRsaKey(RsaKeyType.Public);

            Assert.NotNull(rsaSecKey);
            Assert.Equal("RSA", rsaSecKey.Rsa.SignatureAlgorithm);
        }
        
        [Theory]
        [MemberData(nameof(Helper.UsersId), MemberType = typeof(Helper))]
        public async Task ValidateToken_WithValidUser_IsSuccessful(int id)
        {
            var serviceHelper = new ServiceHelper { UserId = id };
            var userRepo = serviceHelper.GetRequiredService<IUserRepository>();
            var repo = serviceHelper.GetRequiredService<IAuthRepository>();

            var user = await userRepo.GetAsync(id);

            var token = repo.GenerateJwtToken(user);
            var tokenValidation = repo.ValidateToken(token.AccessToken);

            Assert.NotNull(tokenValidation);
            Assert.True(tokenValidation.IsAuthenticated);
        }

        [Theory]
        [MemberData(nameof(Helper.ClientRefreshClientIdToken), MemberType = typeof(Helper))]
        public async Task RevokeTokenAsync_IsSuccessful(
            int clientId, 
            string token)
        {
            var repo = new ServiceHelper().GetRequiredService<IAuthRepository>();

            var revokedTokenResp = await repo.RevokeTokenAsync(token, clientId: clientId);

            Assert.NotNull(revokedTokenResp);
            Assert.Equal(token, revokedTokenResp.Token);
            Assert.NotNull(revokedTokenResp.Revoked);
        }

        
        [Theory]
        [MemberData(nameof(Helper.ClientsApiKey), MemberType = typeof(Helper))]
        public async Task ValidateClientToken_IsAuthenticated(string apiKey)
        {
            var repo = new ServiceHelper().GetRequiredService<IAuthRepository>();

            var tokenReq = new TokenRequest { ApiKey = apiKey };
            var token = await repo.GetTokenAsync(tokenReq);
            var validationReq = new ValidationRequest { AccessToken = token.AccessToken };
            var validation = repo.ValidateToken(validationReq.AccessToken);

            Assert.NotNull(validation);
            Assert.True(validation.IsAuthenticated);
            Assert.Equal(TokenStatus.Valid, validation.Status);
        }
        [Theory]
        [MemberData(nameof(Helper.UsersEmailPassword), MemberType = typeof(Helper))]
        public async Task ValidateUserToken_IsAuthenticated(string email, string password)
        {
            var repo = new ServiceHelper().GetRequiredService<IAuthRepository>();

            var tokenReq = new TokenRequest { Email = email, Password = password };
            var token = await repo.GetTokenAsync(tokenReq);
            var validationReq = new ValidationRequest { AccessToken = token.AccessToken };
            var validation = repo.ValidateToken(validationReq.AccessToken);

            Assert.NotNull(validation);
            Assert.True(validation.IsAuthenticated);
            Assert.Equal(TokenStatus.Valid, validation.Status);
        }
        [Fact]
        public void ValidateToken_Expired()
        {
            var repo = new ServiceHelper().GetRequiredService<IAuthRepository>();

            Assert.Throws<AuthFriendlyException>(() => repo.ValidateToken(Helper.ExpiredJwtToken));
        }
        [Fact]
        public void ValidateClientToken_Expired_ThrowsUnauthorized()
        {
            var repo = new ServiceHelper().GetRequiredService<IAuthRepository>();

            var token = Helper.ExpiredJwtToken;
            var validationReq = new ValidationRequest { AccessToken = token };

            Assert.Throws<AuthFriendlyException>(() => repo.ValidateToken(validationReq.AccessToken));
        }

        [Theory]
        [MemberData(nameof(Helper.UsersEmailPassword), MemberType = typeof(Helper))]
        public async Task Introspect_IsSuccessful(string email, string password)
        {
            var serviceHelper = new ServiceHelper();
            var repo = serviceHelper.GetRequiredService<IAuthRepository>();
            var req = new TokenRequest
            {
                Email = email,
                Password = password
            };
            var token = (await repo.GetTokenAsync(req)).AccessToken;

            serviceHelper.Token = token;
            repo = serviceHelper.GetRequiredService<IAuthRepository>();

            var introspectResult = repo.Introspect();

            Assert.NotNull(introspectResult);
            Assert.Equal(TokenStatus.Valid.ToString(), introspectResult.Status);
        }
        [Fact]
        public void Introspect_Invalid()
        {
            var repo = new ServiceHelper().GetRequiredService<IAuthRepository>();

            var introspectResult = repo.Introspect();

            Assert.NotNull(introspectResult);
            Assert.Equal(TokenStatus.Invalid.ToString(), introspectResult.Status);
        }

        [Fact]
        public void GetPublicJsonWebKey_IsSuccessful()
        {
            var repo = new ServiceHelper().GetRequiredService<IAuthRepository>();

            var jwk = repo.GetPublicJsonWebKey();

            Assert.NotNull(jwk);
            Assert.Equal(AiofClaims.Sig, jwk.Use);
        }

        [Theory]
        [InlineData("testhost", true)]
        [InlineData("aiof-auth", true)]
        [InlineData("aiof-auth-dev", true)]
        public void GetOpenIdConfig_IsSuccessful(string host, bool isHttps)
        {
            var repo = new ServiceHelper().GetRequiredService<IAuthRepository>();

            var openIdConfig = repo.GetOpenIdConfig(
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