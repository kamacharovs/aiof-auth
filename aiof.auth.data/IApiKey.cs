using System;
using System.ComponentModel.DataAnnotations;

namespace aiof.auth.data
{
    public interface IApiKey
    {
        [Required]
        [MaxLength(100)]
        string PrimaryApiKey { get; set; }

        [Required]
        [MaxLength(100)]
        string SecondaryApiKey { get; set; }
    }
}