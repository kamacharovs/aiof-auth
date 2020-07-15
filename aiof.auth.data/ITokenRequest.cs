using System;

namespace aiof.auth.data
{
    public interface ITokenRequest<T>
        where T : class
    {
        string ApiKey { get; set; }
        string Username { get; set; }
        string Password { get; set; }
        T Entity { get; set; }
    }
}