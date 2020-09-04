using System;

namespace aiof.auth.data
{
    public static class Keys
    {
        public const string ApplicationJson = "application/json";
        public const string ApplicationProblemJson = "application/problem+json";

        public const string MemCache = nameof(MemCache);
        public const string Ttl = nameof(Ttl);
        public const string MemCacheTtl = nameof(MemCache) + ":" + nameof(Ttl);

        public const string PostgreSQL = nameof(PostgreSQL);

        public const string Jwt = nameof(Jwt);
        public const string Bearer = nameof(Bearer);
        public const string Expires = nameof(Expires);
        public const string RefreshExpires = nameof(RefreshExpires);
        public const string Type = nameof(Type);
        public const string Issuer = nameof(Issuer);
        public const string Audience = nameof(Audience);
        public const string PrivateKey = nameof(PrivateKey);
        public const string PublicKey = nameof(PublicKey);
        public const string Default = nameof(Default);
        public const string JwtExpires = nameof(Jwt) + ":" + nameof(Expires);
        public const string JwtRefreshExpires = nameof(Jwt) + ":" + nameof(RefreshExpires);
        public const string JwtType = nameof(Jwt) + ":" + nameof(Type);
        public const string JwtIssuer = nameof(Jwt) + ":" + nameof(Issuer);
        public const string JwtAudience = nameof(Jwt) + ":" + nameof(Audience);
        public const string JwtPrivateKey = nameof(Jwt) + ":" + nameof(PrivateKey);
        public const string JwtPublicKey = nameof(Jwt) + ":" + nameof(PublicKey);

        public const string Hash = nameof(Hash);
        public const string Iterations = nameof(Iterations);
        public const string SaltSize = nameof(SaltSize);
        public const string KeySize = nameof(KeySize);
        public const string HashIterations = nameof(Hash) + ":" + nameof(Iterations);
        public const string HashSaltSize = nameof(Hash) + ":" + nameof(SaltSize);
        public const string HashKeySize = nameof(Hash) + ":" + nameof(KeySize);

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

        public static string User = nameof(data.User);
        public static string Client = nameof(data.Client);
        public static string UserRefreshToken = nameof(data.UserRefreshToken);

        public static string Base<T>(int id)
            where T : IPublicKeyId
        {
            return $"{typeof(T).Name.ToLowerInvariant()}.id.{id}";
        }
        public static string Base<T>(Guid publicKey)
            where T : IPublicKeyId
        {
            return $"{typeof(T).Name.ToLowerInvariant()}.publicKey.{publicKey}";
        }
        public static string Base<T>(string apiKey)
            where T : IApiKey
        {
            return $"{typeof(T).Name.ToLowerInvariant()}.apikey.{apiKey}";
        }
    }
}