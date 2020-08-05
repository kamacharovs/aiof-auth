using System.Collections.Generic;

namespace aiof.auth.data
{
    public interface IOpenIdConfig
    {
        string Issuer { get; set; }
        string TokenEndpoint { get; set; }
        string TokenRefreshEndpoint { get; set; }
        IEnumerable<string> ResponseTypes { get; }
        IEnumerable<string> SubjectTypesSupported { get; }
        IEnumerable<string> SigningAlgorithmsSupported { get; }
        IEnumerable<string> ClaimTypesSupported { get; }
        IEnumerable<string> ClaimsSupported { get; }
    }
}