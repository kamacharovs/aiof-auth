using System;
using System.Linq;
using System.Security.Cryptography;

using JetBrains.Annotations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

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

        public static Client GenerateApiKeys(
            [NotNull] this Client client, 
            int length = 32)
        {
            client.PrimaryApiKey = GenerateApiKey(length);
            client.SecondaryApiKey = GenerateApiKey(length);

            return client;
        }
        public static IApiKey GenerateApiKeys(
            [NotNull] this IApiKey entity, 
            int length = 32)
        {
            entity.PrimaryApiKey = GenerateApiKey(length);
            entity.SecondaryApiKey = GenerateApiKey(length);

            return entity;
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