using System;

using Xunit;

using aiof.auth.data;

namespace aiof.auth.tests
{
    [Trait(Helper.Category, Helper.UnitTest)]
    public class UtilsTests
    {
        [Theory]
        [MemberData(nameof(Helper.ApiKeyLength), MemberType = typeof(Helper))]
        public void GenerateApiKey_Valid(int length)
        {
            var apiKey = Utils.GenerateApiKey(length);

            Assert.NotNull(apiKey);
            Assert.Contains('=', apiKey.ToCharArray());
        }

        [Theory]
        [MemberData(nameof(Helper.ApiKeyLength), MemberType = typeof(Helper))]
        public void GenerateApiKey_From_IClient(int length)
        {
            var client = new Client
            {
                Name = "test"
            };

            Assert.Null(client.PrimaryApiKey);
            Assert.Null(client.SecondaryApiKey);

            client.GenerateApiKeys(length);

            Assert.NotNull(client.PrimaryApiKey);
            Assert.NotNull(client.SecondaryApiKey);
        }

        [Theory]
        [MemberData(nameof(Helper.ApiKeyLength), MemberType = typeof(Helper))]
        public void GenerateApiKeys_From_IApiKey(int length)
        {
            var clientApiKey = new Client { } as IApiKey;

            Assert.Null(clientApiKey.PrimaryApiKey);
            Assert.Null(clientApiKey.SecondaryApiKey);

            clientApiKey.GenerateApiKeys(length);

            Assert.NotNull(clientApiKey.PrimaryApiKey);
            Assert.NotNull(clientApiKey.SecondaryApiKey);
        }
    }
}