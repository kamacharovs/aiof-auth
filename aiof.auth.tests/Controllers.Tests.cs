using System;
using System.Net;
using System.Threading.Tasks;
using System.Text.Json;

using Microsoft.AspNetCore.Mvc;

using Xunit;

using aiof.auth.data;
using aiof.auth.services;
using aiof.auth.core.Controllers;

namespace aiof.auth.tests
{
    [Trait(Helper.Category, Helper.UnitTest)]
    public class ControllersTests
    {
        private readonly AuthController _authController;

        public ControllersTests()
        {
            _authController = new AuthController(
                Helper.GetRequiredService<IAuthRepository>(),
                Helper.GetRequiredService<IClientRepository>()
            ) ?? throw new ArgumentNullException(nameof(AuthController));
        }

        [Theory]
        [MemberData(nameof(Helper.ClientsApiKey), MemberType=typeof(Helper))]
        public async Task GetTokenAsync_Client_ApiKey(string apiKey)
        {
            var resp = await _authController.GetTokenAsync(
                new TokenRequest { ApiKey = apiKey }
            ) as OkObjectResult;

            Assert.Equal((int)HttpStatusCode.OK, resp.StatusCode);

            var tokenResp = JsonSerializer.Deserialize<TokenResponse>(
                JsonSerializer.Serialize(resp.Value));

            Assert.NotNull(tokenResp);
            Assert.NotNull(tokenResp.AccessToken);
        }
    }
}