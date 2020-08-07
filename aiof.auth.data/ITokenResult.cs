using System;
using System.Security.Principal;

namespace aiof.auth.data
{
    public interface ITokenResult
    {
        bool IsAuthenticated { get; set; }
        string Status { get; set; }
    }
}