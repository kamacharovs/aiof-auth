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
        public void GenerateApiKey_Client_Valid(int length)
        {
            var apiKey = Utils.GenerateApiKey<Client>(length);

            Assert.NotNull(apiKey);
            Assert.Contains('=', apiKey.ToCharArray());
        }

        [Theory]
        [MemberData(nameof(Helper.ApiKeyLength), MemberType = typeof(Helper))]
        public void GenerateApiKey_User_Valid(int length)
        {
            var apiKey = Utils.GenerateApiKey<User>(length);

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
            var clientApiKey = new Client { };

            Assert.Null(clientApiKey.PrimaryApiKey);
            Assert.Null(clientApiKey.SecondaryApiKey);

            clientApiKey.GenerateApiKeys(length);

            Assert.NotNull(clientApiKey.PrimaryApiKey);
            Assert.NotNull(clientApiKey.SecondaryApiKey);
        }

        [Theory]
        [MemberData(nameof(Helper.UsersApiKeys), MemberType = typeof(Helper))]
        public void DecodeApiKey_Users_ApiKey_Valid(string primaryApiKey, string secondaryApiKey)
        {
            var user1 = primaryApiKey.DecodeKey();
            var user2 = secondaryApiKey.DecodeKey();

            Assert.Equal(nameof(User), user1);
            Assert.Equal(nameof(User), user2);
        }

        [Theory]
        [InlineData(nameof(User))]
        [InlineData(nameof(Client))]
        [InlineData(nameof(ClientRefreshToken))]
        public void Base64Encode_Base64Decode_Valid(string str)
        {
            var encoded = str.Base64Encode();

            Assert.NotNull(encoded);
            Assert.True(encoded.Length > 0);

            var decoded = encoded.Base64Decode();

            Assert.NotNull(decoded);
            Assert.True(decoded.Length > 0);
            Assert.Equal(str, decoded);
        }

        [Theory]
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