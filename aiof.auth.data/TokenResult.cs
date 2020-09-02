using System;
using System.ComponentModel.DataAnnotations;

namespace aiof.auth.data
{
    public class TokenResult : ITokenResult
    {
        [Required]
        public bool IsAuthenticated { get; set; }
        
        [Required]
        public string Status { get; set; }

        [Required]
        public string EntityType { get; set; }
    }
    
    public enum TokenResultStatus
    {
        Valid,
        Expired,
        Error,
        NoToken
    }
}