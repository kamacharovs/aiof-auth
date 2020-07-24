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
        string EntityType { get; }
    }

    public interface ITokenRequest
    {
        string ApiKey { get; set; }
        string Token { get; set; }
        string Username { get; set; }
        string Password { get; set; }
    }

    public interface IRevokeRequest
    {
        int ClientId { get; set; }
        string Token { get; set; }
    }
}