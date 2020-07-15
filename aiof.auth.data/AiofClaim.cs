using System;
using System.Collections.Generic;

namespace aiof.auth.data
{
    public class AiofClaim : IAiofClaim, IPublicKeyId
    {
        public int Id { get; set; }
        public Guid PublicKey { get; set; }
        public string Name { get; set; }
    }

    public static class AiofClaims
    {
        public const string PublicKey = "public_key";
        public const string GivenName = "given_name";
        public const string FamilyName = "family_name";
        public const string Name = "name";
        public const string Email = "email";

        public static IEnumerable<string> All
            => new List<string>
            {
                PublicKey,
                GivenName,
                FamilyName,
                Name,
                Email
            };
    }
}