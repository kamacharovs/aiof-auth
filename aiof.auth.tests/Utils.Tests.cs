using System;

using Xunit;

using aiof.auth.data;

namespace aiof.auth.tests
{
    [Trait(Helper.Category, Helper.UnitTest)]
    public class UtilsTests
    {
        [Theory]
        [InlineData(32)]
        [InlineData(64)]
        [InlineData(128)]
        public void GenerateApiKey_Valid(int length)
        {
            var apiKey = Utils.GenerateApiKey(length);

            Assert.NotNull(apiKey);
            Assert.Contains('=', apiKey.ToCharArray());
        }

        [Theory]
        [InlineData(32)]
        [InlineData(64)]
        [InlineData(128)]
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
    }
}