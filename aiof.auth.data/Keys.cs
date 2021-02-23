using System;

namespace aiof.auth.data
{
    public static class Keys
    {
        public const string MemCache = nameof(MemCache);
        public const string Ttl = nameof(Ttl);
        public const string MemCacheTtl = MemCache + ":" + Ttl;

        public const string Data = nameof(Data);
        public const string PostgreSQL = nameof(PostgreSQL);
        public const string DataPostgreSQL = Data + ":" + PostgreSQL;

        public const string Cors = nameof(Cors);
        public const string Portal = nameof(Portal);
        public const string CorsPortal = Cors + ":" + Portal;

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
        public const string JwtExpires = Jwt + ":" + Expires;
        public const string JwtRefreshExpires = Jwt + ":" + RefreshExpires;
        public const string JwtType = Jwt + ":" + Type;
        public const string JwtIssuer = Jwt + ":" + Issuer;
        public const string JwtAudience = Jwt + ":" + Audience;
        public const string JwtPrivateKey = Jwt + ":" + PrivateKey;
        public const string JwtPublicKey = Jwt + ":" + PublicKey;

        public const string Hash = nameof(Hash);
        public const string Iterations = nameof(Iterations);
        public const string SaltSize = nameof(SaltSize);
        public const string KeySize = nameof(KeySize);
        public const string HashIterations = Hash + ":" + Iterations;
        public const string HashSaltSize = Hash + ":" + SaltSize;
        public const string HashKeySize = Hash + ":" + KeySize;

        public const string OpenApi = nameof(OpenApi);
        public const string Version = nameof(Version);
        public const string Title = nameof(Title);
        public const string Description = nameof(Description);
        public const string Contact = nameof(Contact);
        public const string Name = nameof(Name);
        public const string Email = nameof(Email);
        public const string Url = nameof(Url);
        public const string License = nameof(License);
        public const string OpenApiVersion = OpenApi + ":" + Version;
        public const string OpenApiTitle = OpenApi + ":" + Title;
        public const string OpenApiDescription = OpenApi + ":" + Description;
        public const string OpenApiContactName = OpenApi + ":" + Contact + ":" + Name;
        public const string OpenApiContactEmail = OpenApi + ":" + Contact + ":" + Email;
        public const string OpenApiContactUrl = OpenApi + ":" + Contact + ":" + Url;
        public const string OpenApiLicenseName = OpenApi + ":" + License + ":" + Name;
        public const string OpenApiLicenseUrl = OpenApi + ":" + License + ":" + Url;

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

        public static class Entity
        {
            public static string User = nameof(data.User).ToSnakeCase();
            public static string UserProfile = nameof(data.UserProfile).ToSnakeCase();
            public static string Client = nameof(data.Client).ToSnakeCase();
            public static string UserRefreshToken = nameof(data.UserRefreshToken).ToSnakeCase();
            public static string ClientRefreshToken = nameof(data.ClientRefreshToken).ToSnakeCase();
            public static string Role = nameof(data.Role).ToSnakeCase();
            public static string Claim = "claim";
        }
    }
}