using System;
using System.Security.Claims;

namespace aiof.auth.data
{
    public interface ITokenResult
    {
        ClaimsPrincipal Principal { get; set; }
        TokenResultStatus Status { get; set; }
    }
}