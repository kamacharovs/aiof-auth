using System;

namespace aiof.auth.data
{
    public interface IPublicKeyId
    {
        int Id { get; set; }
        Guid PublicKey { get; set; }
    }
}