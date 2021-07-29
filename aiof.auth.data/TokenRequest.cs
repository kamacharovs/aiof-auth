using System;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;

namespace aiof.auth.data
{

    /// <summary>
    /// Request for authentication. The combinations of requests are: ApiKey or Token or Email and Password
    /// </summary>
    public class TokenRequest : ITokenRequest
    {
        [JsonPropertyName("api_key")]
        [MaxLength(64)]
        public string ApiKey { get; set; }

        [JsonPropertyName("refresh_token")]
        [MaxLength(128)]
        public string Token { get; set; }

        [JsonPropertyName("email")]
        [MaxLength(200)]
        public string Email { get; set; }

        [JsonPropertyName("password")]
        [MaxLength(100)]
        public string Password { get; set; }

        [JsonIgnore]
        public TokenType Type
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(Email)
                    && !string.IsNullOrWhiteSpace(Password))
                {
                    return TokenType.User;
                }
                else if (!string.IsNullOrWhiteSpace(ApiKey))
                {
                    return TokenType.ApiKey;
                }
                else if (!string.IsNullOrWhiteSpace(Token))
                {
                    return TokenType.Refresh;
                }
                else
                {
                    return TokenType.NoMatch;
                }
            }
        }
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
}