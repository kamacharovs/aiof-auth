using System;
using System.Text.Json.Serialization;

namespace aiof.auth.data
{
    public class TokenRequest<T> : ITokenRequest<T>
        where T : class
    {
        [JsonPropertyName("api_key")]
        public string ApiKey { get; set; }

        [JsonPropertyName("refresh_token")]
        public string Token { get; set; }

        [JsonPropertyName("username")]
        public string Username { get; set; }

        [JsonPropertyName("password")]
        public string Password { get; set; }

        [JsonIgnore]
        public T Entity { get; set; }

        [JsonIgnore]
        public string EntityType => typeof(T).Name;
    }

    public class TokenRequest : ITokenRequest
    {
        [JsonPropertyName("api_key")]
        public string ApiKey { get; set; }

        [JsonPropertyName("refresh_token")]
        public string Token { get; set; }

        [JsonPropertyName("username")]
        public string Username { get; set; }

        [JsonPropertyName("password")]
        public string Password { get; set; }

        [JsonIgnore]
        public TokenRequestType Type { get; set; }
    }

    public class RevokeRequest : IRevokeRequest
    {
        public int ClientId { get; set; }
        public string Token { get; set; }
    }

    public enum TokenRequestType
    {
        Client = 1,
        Refresh = 2,
        User = 3
    }
}