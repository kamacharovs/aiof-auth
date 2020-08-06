using System;
using System.Linq;

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

        [Theory]
        [InlineData(nameof(User.Username))]
        [InlineData(nameof(User.Password))]
        [InlineData(nameof(User.Email))]
        [InlineData(nameof(Client.Name))]
        [InlineData(nameof(Client.Slug))]
        public void ToSnakeCase_With_NoUpperCase(string str)
        {
            var snakeCaseStr = str.ToSnakeCase();

            Assert.NotNull(snakeCaseStr);
            Assert.DoesNotContain(snakeCaseStr, char.IsUpper);
        }
        [Theory]
        [InlineData(nameof(User.PublicKey))]
        [InlineData(nameof(User.FirstName))]
        [InlineData(nameof(User.LastName))]
        [InlineData(nameof(Client.PrimaryApiKey))]
        [InlineData(nameof(Client.SecondaryApiKey))]
        public void ToSnakeCase_With_Underscore(string str)
        {
            var snakeCaseStr = str.ToSnakeCase();

            Assert.NotNull(snakeCaseStr);
            Assert.DoesNotContain(snakeCaseStr, char.IsUpper);
            Assert.Contains('_', snakeCaseStr);
        }

        [Theory]
        [MemberData(nameof(Helper.ClientsName), MemberType = typeof(Helper))]
        public void ToHyphenCase(string str)
        {
            var hypenCase = str.ToHyphenCase();

            Assert.NotNull(hypenCase);

            if (str.Contains(' '))
                Assert.Contains('-', hypenCase.ToCharArray());
        }
    }
}