using System;
using System.Security.Claims;

namespace aiof.auth.data
{
    public class TokenResult : ITokenResult
    {
        public ClaimsPrincipal Principal { get; set; }
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