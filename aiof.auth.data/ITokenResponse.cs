using System;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;

namespace aiof.auth.data
{
    /// <summary>
    /// Response for authentication. The required fields are TokenType, ExpiresIn and AccessToken. RefreshToken is optional
    /// </summary>
    public interface ITokenResponse
    {
        [JsonPropertyName("token_type")]
        [Required]
        string TokenType { get; set; }
        
        [JsonPropertyName("expires_in")]
        [Required]
        int ExpiresIn { get; set; }
        
        [JsonPropertyName("access_token")]
        [Required]
        string AccessToken { get; set; }

        [JsonPropertyName("refresh_token")]
        string RefreshToken { get; set; }
    }

    /// <summary>
    /// Response for authentication for User. This is used to return User's information together with the token information
    /// </summary>
    public interface ITokenUserResponse : ITokenResponse
    {
        [JsonPropertyName("user")]
        [Required]
        User User { get; set; }
    }

    /// <summary>
    /// Response to revoke a Client refresh token
    /// </summary>
    public interface IRevokeResponse
    {
        [Required]
        int ClientId { get; set; }

        [JsonPropertyName("refresh_token")]
        [Required]
        [MaxLength(128)]
        string Token { get; set; }

        [Required]
        DateTime? Revoked { get; set; }
    }
}