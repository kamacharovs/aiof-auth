using System;
using System.ComponentModel.DataAnnotations;

namespace aiof.auth.data
{
    public interface ITokenResult
    {
        [Required]
        bool IsAuthenticated { get; set; }
        
        [Required]
        string Status { get; set; }
    }
}