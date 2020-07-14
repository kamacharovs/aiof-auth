using System;

namespace aiof.auth.data
{
    public class AiofClaim : IAiofClaim, IPublicKeyId
    {
        public int Id { get; set; }
        public Guid PublicKey { get; set; }
        public string Name { get; set; }
    }
}