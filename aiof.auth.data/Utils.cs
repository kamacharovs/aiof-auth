using System;
using System.Text;
using System.Linq;
using System.Net;
using System.Security.Cryptography;

using JetBrains.Annotations;

namespace aiof.auth.data
{
    public static partial class Utils
    {
        public static string GenerateApiKey<T>(int length = 32)
            where T : class
        {
            var key = new byte[length];
            using (var generator = RandomNumberGenerator.Create())
                generator.GetBytes(key);
            return $"{typeof(T).Name.Base64Encode()}.{Convert.ToBase64String(key)}";
        }

        public static Client GenerateApiKeys(
            [NotNull] this Client client,
            int length = 32)
        {
            client.PrimaryApiKey = GenerateApiKey<Client>(length);
            client.SecondaryApiKey = GenerateApiKey<Client>(length);

            return client;
        }

        public static string DecodeKey(
            [NotNull] this string apiKey)
        {
            return apiKey.Split('.')
                .First()
                .Base64Decode();
        }

        public static string Base64Encode(
            [NotNull] this string plainText)
        {
            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            return Convert.ToBase64String(plainTextBytes);
        }
        public static string Base64Decode(
            [NotNull] this string base64EncodedData)
        {
            var base64EncodedBytes = Convert.FromBase64String(base64EncodedData);
            return Encoding.UTF8.GetString(base64EncodedBytes);
        }

        public static string ToSnakeCase(
            [NotNull] this string value)
        {
            return string.Concat(value.Select((x, i) => i > 0 && char.IsUpper(x) ? "_" + x.ToString() : x.ToString())).ToLower();
        }

        public static string ToHyphenCase(
            [NotNull] this string value)
        {
            return value.Replace(' ', '-').ToLower();
        }
    }
}