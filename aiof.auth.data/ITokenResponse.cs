using System;

namespace aiof.auth.data
{
    public interface ITokenResponse
    {
        string TokenType { get; set; }
        int ExpiresIn { get; set; }
        string AccessToken { get; set; }
        string RefreshToken { get; set; }
    }
}