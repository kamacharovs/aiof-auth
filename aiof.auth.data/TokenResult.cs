using System;

namespace aiof.auth.data
{
    public class TokenResult : ITokenResult
    {
        public bool IsAuthenticated { get; set; }
        public string Status { get; set; }
    }
    
    public enum TokenResultStatus
    {
        Valid,
        Expired,
        Error,
        NoToken
    }
}