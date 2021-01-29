using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace aiof.auth.data
{
    public interface ITenant
    {
        [JsonPropertyName("user_id")]
        int UserId { get; set; }

        [JsonPropertyName("client_id")]
        int ClientId { get; set; }

        [JsonPropertyName("public_key")]
        Guid PublicKey { get; set; }

        [JsonIgnore]
        string Log { get; }
    }
}
