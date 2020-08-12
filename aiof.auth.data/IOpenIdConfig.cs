using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;

namespace aiof.auth.data
{
    /// <summary>
    /// OpenId configuration from https://openid.net/specs/openid-connect-discovery-1_0.html
    /// </summary>
    public interface IOpenIdConfig
    {
        [JsonPropertyName("issuer")]
        [Required]
        string Issuer { get; set; }
        
        [JsonPropertyName("token_endpoint")]
        [Required]
        string TokenEndpoint { get; set; }
        
        [JsonPropertyName("token_refresh_endpoint")]
        [Required]
        string TokenRefreshEndpoint { get; set; }

        [JsonPropertyName("jwks_uri")]
        [Required]
        string JsonWebKeyEndpoint{ get; set; }
               
        [JsonPropertyName("response_types_supported")]
        [Required]
        IEnumerable<string> ResponseTypes { get; }
        
        [JsonPropertyName("subject_types_supported")]
        [Required]
        IEnumerable<string> SubjectTypesSupported { get; }
        
        [JsonPropertyName("id_token_signing_alg_values_supported")]
        [Required]
        IEnumerable<string> SigningAlgorithmsSupported { get; }
        
        [JsonPropertyName("claim_types_supported")]
        [Required]
        IEnumerable<string> ClaimTypesSupported { get; }
        
        [JsonPropertyName("claims_supported")]
        [Required]
        IEnumerable<string> ClaimsSupported { get; }
    }
}