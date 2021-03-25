using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;

namespace aiof.auth.data
{
    public class UpdatePasswordRequest : IUpdatePasswordRequest
    {
        [Required]
        [MaxLength(100)]
        public string OldPassword { get; set; }

        [Required]
        [MaxLength(100)]
        public string NewPassword { get; set; }
    }

    public class UpdatePasswordUnauthenticatedRequest : IUpdatePasswordUnauthenticatedRequest
    {
        [Required]
        [EmailAddress]
        [MaxLength(200)]
        public string Email { get; set; }

        [Required]
        [MaxLength(100)]
        public string OldPassword { get; set; }

        [Required]
        [MaxLength(100)]
        public string NewPassword { get; set; }
    }
}