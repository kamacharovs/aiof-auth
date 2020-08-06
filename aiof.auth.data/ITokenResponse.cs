using System;
using System.ComponentModel.DataAnnotations;

namespace aiof.auth.data
{
    /// <summary>
    /// Response for authentication. The required fields are TokenType, ExpiresIn and AccessToken. RefreshToken is optional
    /// </summary>
    public interface ITokenResponse
    {
        [Required]
        string TokenType { get; set; }
        
        [Required]
        int ExpiresIn { get; set; }
        
        [Required]
        string AccessToken { get; set; }

        string RefreshToken { get; set; }
    }
}