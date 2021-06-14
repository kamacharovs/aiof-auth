using System;
using System.Text.Json.Serialization;

namespace aiof.auth.data
{
    public class TokenResult : ITokenResult
    {
        public bool IsAuthenticated { get; set; }
        
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public TokenStatus Status { get; set; }

        public string EntityType { get; set; }
    }
}