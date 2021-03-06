using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;

namespace aiof.auth.data
{
    public class AuthProblemDetail : IAuthProblemDetail
    {
        [JsonPropertyName("code")]
        [Required]
        public int? Code { get; set; }
        
        [JsonPropertyName("message")]
        [Required]
        public string Message { get; set; }

        [JsonPropertyName("traceId")]
        [Required]
        public string TraceId { get; set; }

        [JsonPropertyName("errors")]
        public IEnumerable<string> Errors { get; set; }
    }

    public class AuthProblemDetailBase : IAuthProblemDetailBase
    {
        [JsonPropertyName("code")]
        [Required]
        public int? Code { get; set; }
        
        [JsonPropertyName("message")]
        [Required]
        public string Message { get; set; }
    }
}