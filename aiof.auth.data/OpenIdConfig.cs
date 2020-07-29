using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace aiof.auth.data
{
    /// <summary>
    /// OpenId configuration from https://openid.net/specs/openid-connect-discovery-1_0.html
    /// </summary>
    public class OpenIdConfig
    {
        [JsonPropertyName("issuer")]
        public string Issuer { get; set; }

        [JsonPropertyName("token_endpoint")]
        public string TokenEndpoint { get; set; }

        [JsonPropertyName("token_refresh_endpoint")]
        public string TokenRefreshEndpoint { get; set; }

        [JsonPropertyName("response_types_supported")]
        public IEnumerable<string> ResponseTypes { get; } = new List<string> { "code token" };

        [JsonPropertyName("subject_types_supported")]
        public IEnumerable<string> SubjectTypesSupported { get; } = new List<string> { "public", "pairwise" };

        [JsonPropertyName("token_endpoint_auth_signing_alg_values_supported")]
        public IEnumerable<string> SigningAlgorithmsSupported { get; } = new List<string> { "RS256" };

        [JsonPropertyName("claim_types_supported")]
        public IEnumerable<string> ClaimTypesSupported { get; } = new List<string> { "normal" };

        [JsonPropertyName("claims_supported")]
        public IEnumerable<string> ClaimsSupported { get; } = AiofClaims.All;
    }
}
