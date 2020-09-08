using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;

namespace aiof.auth.data
{
    public interface IAuthProblemDetail
    {
        [JsonPropertyName("code")]
        [Required]
        int? Code { get; set; }
        
        [JsonPropertyName("message")]
        [Required]
        string Message { get; set; }

        [JsonPropertyName("traceId")]
        [Required]
        string TraceId { get; set; }

        [JsonPropertyName("errors")]
        IEnumerable<string> Errors { get; set; }
    }

    public interface IAuthProblemDetailBase
    {
        [JsonPropertyName("code")]
        [Required]
        int? Code { get; set; }
        
        [JsonPropertyName("message")]
        [Required]
        string Message { get; set; }
    }
}