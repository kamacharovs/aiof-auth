using System;
using System.ComponentModel.DataAnnotations;

namespace aiof.auth.data
{
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