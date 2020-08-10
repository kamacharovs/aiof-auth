using System;

namespace aiof.auth.data
{
    public static class Keys
    {
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