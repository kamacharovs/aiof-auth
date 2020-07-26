using System;

namespace aiof.auth.data
{
    public static class Keys
    {
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
    }
}