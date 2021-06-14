using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;

namespace aiof.auth.data
{
    public class IntrospectTokenResult : IIntrospectTokenResult
    {
        public Dictionary<string, string> Claims { get; set; } = new Dictionary<string, string>();

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public TokenStatus Status { get; set; }
    }
}