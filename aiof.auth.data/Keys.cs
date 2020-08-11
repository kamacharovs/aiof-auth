using System;

namespace aiof.auth.data
{
    public static class Keys
    {
        public const string ApplicationJson = "application/json";

        public const string MemCache = nameof(MemCache);
        public const string Ttl = nameof(Ttl);

        public const string PostgreSQL = nameof(PostgreSQL);

        public const string Jwt = nameof(Jwt);
        public const string Expires = nameof(Expires);
        public const string RefreshExpires = nameof(RefreshExpires);
        public const string Type = nameof(Type);
        public const string Issuer = nameof(Issuer);
        public const string Audience = nameof(Audience);
        public const string Secret = nameof(Secret);

        public const string Hash = nameof(Hash);
        public const string Iterations = nameof(Iterations);
        public const string SaltSize = nameof(SaltSize);
        public const string KeySize = nameof(KeySize);

        public const string OpenApi = nameof(OpenApi);
        public const string Version = nameof(Version);
        public const string Title = nameof(Title);
        public const string Description = nameof(Description);
        public const string Contact = nameof(Contact);
        public const string Name = nameof(Name);
        public const string Email = nameof(Email);
        public const string Url = nameof(Url);
        public const string License = nameof(License);
        public const string OpenApiVersion = nameof(OpenApi) + ":" + nameof(Version);
        public const string OpenApiTitle = nameof(OpenApi) + ":" + nameof(Title);
        public const string OpenApiDescription = nameof(Description) + ":" + nameof(Description);
        public const string OpenApiContactName = nameof(OpenApi) + ":" + nameof(Contact) + ":" + nameof(Name);
        public const string OpenApiContactEmail = nameof(OpenApi) + ":" + nameof(Contact) + ":" + nameof(Email);
        public const string OpenApiContactUrl = nameof(OpenApi) + ":" + nameof(Contact) + ":" + nameof(Url);
        public const string OpenApiLicenseName = nameof(OpenApi) + ":" + nameof(License) + ":" + nameof(Name);
        public const string OpenApiLicenseUrl = nameof(OpenApi) + ":" + nameof(License) + ":" + nameof(Url);

        ///
        /// Caching keys
        ///
        public static string Base<T>(int id)
            where T : IPublicKeyId
        {
            return $"{typeof(T).Name.ToLower()}.id.{id}";
        }
        public static string Base<T>(string apiKey)
            where T : IApiKey
        {
            return $"{typeof(T).Name.ToLower()}.apikey.{apiKey}";
        }

        public static string User(string username)
        {
            return $"user.username.{username}";
        }
        public static string User(int id)
        {
            return $"user.id.{id}";
        }

        public static string Client(int id)
        {
            return $"client.id.{id}";
        }
        public static string Client(string apiKey)
        {
            return $"client.apikey.{apiKey}";
        }
    }
}