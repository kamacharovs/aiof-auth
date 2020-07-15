using System;
using System.Text.Json.Serialization;

namespace aiof.auth.data
{
    public class TokenRequest<T> : ITokenRequest<T>
        where T : class
    {
        [JsonPropertyName("api_key")]
        public string ApiKey { get; set; }

        [JsonIgnore]
        public T Entity { get; set; }
    }
}