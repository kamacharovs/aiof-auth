using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;

namespace aiof.auth.data
{
    public interface IIntrospectTokenResult
    {
        [Required]
        Dictionary<string, string> Claims { get; set; }

        [Required]
        string Status { get; set; }
    }
}