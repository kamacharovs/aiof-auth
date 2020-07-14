using System;
using System.Text.Json.Serialization;

namespace aiof.auth.data
{
    public class TokenResponse : ITokenResponse
    {
        [JsonPropertyName("token_type")]
        public string TokenType { get; set; } = "Bearer";

        [JsonPropertyName("expires_in")]
        public int ExpiresIn { get; set; }

        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; }
    }
}