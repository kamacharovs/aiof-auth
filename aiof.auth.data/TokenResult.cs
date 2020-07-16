using System;
using System.Security.Claims;

namespace aiof.auth.data
{
    public enum TokenResultStatus
    {
        Valid,
        Expired,
        Error,
        NoToken
    }

    public class TokenResult : ITokenResult
    {
        public ClaimsPrincipal Principal { get; set; }
        public TokenResultStatus Status { get; set; }
    }
}