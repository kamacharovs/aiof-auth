using System;
using System.Security.Claims;

namespace aiof.auth.data
{
    public interface ITokenResult
    {
        ClaimsPrincipal Principal { get; set; }
        string Status { get; set; }
    }
}