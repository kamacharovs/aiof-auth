using System;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;

namespace aiof.auth.data
{
    public class TokenRequest<T> : ITokenRequest<T>
        where T : class
    {
        [JsonPropertyName("api_key")]
        public string ApiKey { get; set; }

        [JsonPropertyName("refresh_token")]
        public string Token { get; set; }

        [JsonPropertyName("username")]
        public string Username { get; set; }

        [JsonPropertyName("password")]
        public string Password { get; set; }

        [JsonIgnore]
        public T Entity { get; set; }

        [JsonIgnore]
        public string EntityType => typeof(T).Name;
    }

    /// <summary>
    /// Request for authentication. The combinations of requests are: ApiKey or Token or Username and Password
    /// </summary>
    public class TokenRequest : ITokenRequest
    {
        [JsonPropertyName("api_key")]
        [MaxLength(64)]
        public string ApiKey { get; set; }

        [JsonPropertyName("refresh_token")]
        [MaxLength(128)]
        public string Token { get; set; }

        [JsonPropertyName("username")]
        [MaxLength(200)]
        public string Username { get; set; }

        [JsonPropertyName("password")]
        [MaxLength(100)]
        public string Password { get; set; }

        [JsonIgnore]
        public TokenType Type { get; set; }
    }

    /// <summary>
    /// Request to revoke a Client refresh token
    /// </summary>
    public class RevokeRequest : IRevokeRequest
    {
        [Required]
        public int ClientId { get; set; }

        [JsonPropertyName("refresh_token")]
        [Required]
        [MaxLength(128)]
        public string Token { get; set; }
    }

    /// <summary>
    /// Request to validate an access token
    /// </summary>
    public class ValidationRequest : IValidationRequest
    {
        [JsonPropertyName("access_token")]
        [Required]
        public string AccessToken { get; set; }
    }

    public enum TokenType
    {
        Client = 1,
        Refresh = 2,
        User = 3
    }
}