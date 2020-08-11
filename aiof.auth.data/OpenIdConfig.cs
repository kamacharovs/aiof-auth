using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace aiof.auth.data
{
    /// <summary>
    /// OpenId configuration from https://openid.net/specs/openid-connect-discovery-1_0.html
    /// </summary>
    public class OpenIdConfig : IOpenIdConfig
    {
        [JsonPropertyName("issuer")]
        public string Issuer { get; set; }

        [JsonPropertyName("token_endpoint")]
        public string TokenEndpoint { get; set; }

        [JsonPropertyName("token_refresh_endpoint")]
        public string TokenRefreshEndpoint { get; set; }

        [JsonPropertyName("jwks_uri")]
        public string JsonWebKeyEndpoint{ get; set; }

        [JsonPropertyName("response_types_supported")]
        public IEnumerable<string> ResponseTypes { get; } = new List<string> 
        { 
            "code token" 
        };

        [JsonPropertyName("subject_types_supported")]
        public IEnumerable<string> SubjectTypesSupported { get; } = new List<string> 
        { 
            "public", 
            "pairwise" 
        };

        [JsonPropertyName("id_token_signing_alg_values_supported")]
        public IEnumerable<string> SigningAlgorithmsSupported { get; } = new List<string> 
        { 
            "HS256",
            "RS256"
        };

        [JsonPropertyName("claim_types_supported")]
        public IEnumerable<string> ClaimTypesSupported { get; } = new List<string> 
        { 
            "normal" 
        };

        [JsonPropertyName("claims_supported")]
        public IEnumerable<string> ClaimsSupported { get; } = AiofClaims.All;
    }

    public static class OpenIdConfigConstants
    {
        public static string Use => AiofClaims.Sig;
        public static string Alg => "RS256";
    }
}
