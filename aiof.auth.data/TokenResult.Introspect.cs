using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;

namespace aiof.auth.data
{
    public class IntrospectTokenResult : IIntrospectTokenResult
    {
        [Required]
        public Dictionary<string, string> Claims { get; set; } = new Dictionary<string, string>();

        [Required]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public TokenStatus Status { get; set; }
    }
}