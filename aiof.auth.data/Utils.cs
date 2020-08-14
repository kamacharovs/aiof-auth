using System;
using System.Text;
using System.Linq;
using System.Net;
using System.Security.Cryptography;

using JetBrains.Annotations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace aiof.auth.data
{
    public static class Utils
    {
        public static string GenerateApiKey<T>(int length = 32)
            where T : class
        {
            var key = new byte[length];
            using (var generator = RandomNumberGenerator.Create())
                generator.GetBytes(key);
            var tkey = Encoding.ASCII.GetBytes(typeof(T).Name.ToLowerInvariant());
            return $"{Convert.ToBase64String(tkey)}.{Convert.ToBase64String(key)}";
        }

        public static Client GenerateApiKeys(
            [NotNull] this Client client, 
            int length = 32)
        {
            client.PrimaryApiKey = GenerateApiKey<Client>(length);
            client.SecondaryApiKey = GenerateApiKey<Client>(length);

            return client;
        }

        public static Type DecodeApiKey<T>(this string apiKey)
            where T : class
        {
            var apiKeyEntity = apiKey.Split('.').First();
            
            if (apiKeyEntity == nameof(T).ToLowerInvariant())
                return typeof(T);
            else
                throw new AuthFriendlyException(HttpStatusCode.BadRequest,
                    $"Error");
        }

        public static string ToSnakeCase(
            [NotNull] this string str)
        {
            return string.Concat(str.Select((x, i) => i > 0 && char.IsUpper(x) ? "_" + x.ToString() : x.ToString())).ToLower();
        }

        public static string ToHyphenCase(
            [NotNull] this string str)
        {
            return str.Replace(' ', '-').ToLower();
        }

        public static PropertyBuilder HasSnakeCaseColumnName(
            [NotNull] this PropertyBuilder propertyBuilder)
        {
            propertyBuilder.Metadata.SetColumnName(
                propertyBuilder
                    .Metadata
                    .Name
                    .ToSnakeCase());

            return propertyBuilder;
        }
      
        public static T ParseEnum<T>(string value)
        {
            return (T) Enum.Parse(typeof(T), value, true);
        }

        public static T ToEnum<T>(
            [NotNull] this string value)
        {
            return (T) Enum.Parse(typeof(T), value, true);
        }
        public static AlgType ToEnum(
            [NotNull] this string value)
        {
            return (AlgType) Enum.Parse(typeof(AlgType), value, true);
        }
    }
}