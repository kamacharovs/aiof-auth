using System;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;

namespace aiof.auth.data
{
    public class TokenResult : ITokenResult
    {
        [Required]
        public bool IsAuthenticated { get; set; }
        
        [Required]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public TokenStatus Status { get; set; }

        [Required]
        public string EntityType { get; set; }
    }
}