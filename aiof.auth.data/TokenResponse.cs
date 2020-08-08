using System;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;

namespace aiof.auth.data
{
    public class TokenResponse : ITokenResponse
    {
        [JsonPropertyName("token_type")]
        [Required]
        public string TokenType { get; set; } = "Bearer";

        [JsonPropertyName("expires_in")]
        [Required]
        public int ExpiresIn { get; set; }

        [JsonPropertyName("access_token")]
        [Required]
        public string AccessToken { get; set; }

        [JsonPropertyName("refresh_token")]
        public string RefreshToken { get; set; }
    }

    /// <summary>
    /// Response to revoke a Client refresh token
    /// </summary>
    public class RevokeResponse : IRevokeResponse
    {
        [Required]
        public int ClientId { get; set; }

        [JsonPropertyName("refresh_token")]
        [Required]
        [MaxLength(128)]
        public string Token { get; set; }

        [Required]
        public DateTime? Revoked { get; set; }
    }
}