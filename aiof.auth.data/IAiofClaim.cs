using System;

namespace aiof.auth.data
{
    public interface IAiofClaim
    {
        int Id { get; set; }
        Guid PublicKey { get; set; }
        string Name { get; set; }
    }
}