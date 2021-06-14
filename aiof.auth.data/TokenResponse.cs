using System;
using System.Text.Json.Serialization;

namespace aiof.auth.data
{
    /// <summary>
    /// Response to JWT creation
    /// </summary>
    public class TokenResponse : ITokenResponse
    {
        [JsonPropertyName("token_type")]
        public string TokenType { get; set; } = "Bearer";

        [JsonPropertyName("expires_in")]
        public int ExpiresIn { get; set; }

        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; }

        [JsonPropertyName("refresh_token")]
        public string RefreshToken { get; set; }
    }

    /// <summary>
    /// Response for authentication for User. This is used to return User's information together with the token information
    /// </summary>
    public class TokenUserResponse : TokenResponse, ITokenUserResponse
    {
        [JsonPropertyName("user")]
        public IUser User { get; set; }
    }

    /// <summary>
    /// Response to revoke a refresh token
    /// </summary>
    public class RevokeResponse : IRevokeResponse
    {
        [JsonPropertyName("refresh_token")]
        public string Token { get; set; }

        public DateTime? Revoked { get; set; }
    }
}