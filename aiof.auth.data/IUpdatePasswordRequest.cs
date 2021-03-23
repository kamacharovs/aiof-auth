using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;

namespace aiof.auth.data
{
    public interface IUpdatePasswordRequest
    {
        [Required]
        [MaxLength(100)]
        string OldPassword { get; set; }

        [Required]
        [MaxLength(100)]
        string NewPassword { get; set; }
    }
}