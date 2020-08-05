using System;
using System.Collections.Generic;

namespace aiof.auth.data
{
    public class AiofClaim : IAiofClaim, IPublicKeyId
    {
        public int Id { get; set; }
        public Guid PublicKey { get; set; } = Guid.NewGuid();
        public string Name { get; set; }
    }

    public static class AiofClaims
    {
        public const string Sub = "sub";
        public const string Iss = "iss";

        public const string PublicKey = "public_key";
        public const string GivenName = "given_name";
        public const string FamilyName = "family_name";
        public const string Name = "name";
        public const string Email = "email";
        public const string Slug = "slug";

        public static IEnumerable<string> All
            => new List<string>
            {
                Sub,
                Iss,
                PublicKey,
                GivenName,
                FamilyName,
                Name,
                Email,
                Slug
            };
    }
}