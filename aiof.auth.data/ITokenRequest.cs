using System;

namespace aiof.auth.data
{
    public interface ITokenRequest
    {
        string ApiKey { get; set; }
        string Token { get; set; }
        string Email { get; set; }
        string Password { get; set; }
        TokenType Type { get; set; }
    }

    public interface IRevokeRequest
    {
        string Token { get; set; }
    }
    public interface IRevokeUserRequest : IRevokeRequest
    {
        int UserId { get; set; }
    }

    public interface IRevokeClientRequest : IRevokeRequest
    {
        int ClientId { get; set; }
    }

    public interface  IValidationRequest 
    {
        string AccessToken { get; set; }
    }
}