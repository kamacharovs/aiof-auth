using System;
using System.Security.Cryptography;

namespace aiof.auth.data
{
    public static class Utils
    {
        public static string GenerateApiKey(int length = 32)
        {
            var key = new byte[length];
            using (var generator = RandomNumberGenerator.Create())
                generator.GetBytes(key);
            return Convert.ToBase64String(key);
        }

        public static Client GenerateApiKeys(this Client client, int length = 32)
        {
            client.PrimaryApiKey = GenerateApiKey(length);
            client.SecondaryApiKey = GenerateApiKey(length);

            return client;
        }
        public static IApiKey GenerateApiKeys(this IApiKey entity, int length = 32)
        {
            entity.PrimaryApiKey = GenerateApiKey(length);
            entity.SecondaryApiKey = GenerateApiKey(length);

            return entity;
        }
    }
}