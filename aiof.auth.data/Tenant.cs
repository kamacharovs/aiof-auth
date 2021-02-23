using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

using Microsoft.AspNetCore.Http;

namespace aiof.auth.data
{
    public class Tenant : ITenant
    {
        [JsonIgnore]
        private readonly IHttpContextAccessor _httpContextAccessor;

        [JsonPropertyName("user_id")]
        public int UserId { get; set; }

        [JsonPropertyName("client_id")]
        public int ClientId { get; set; }

        [JsonPropertyName("public_key")]
        public Guid PublicKey { get; set; }

        [JsonPropertyName("claims")]
        public Dictionary<string, string> Claims { get; set; } = new Dictionary<string, string>();

        [JsonIgnore]
        public string Log
        {
            get
            {
                return JsonSerializer.Serialize(this);
            }
        }

        public Tenant(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));

            var user = _httpContextAccessor.HttpContext?.User;

            int userId, clientId;
            Guid publicKey;

            int.TryParse(user?.FindFirst(AiofClaims.UserId)?.Value, out userId);
            int.TryParse(user?.FindFirst(AiofClaims.ClientId)?.Value, out clientId);
            Guid.TryParse(user?.FindFirst(AiofClaims.PublicKey)?.Value, out publicKey);

            UserId = userId;
            ClientId = clientId;
            PublicKey = publicKey;

            // Get the claims
            foreach (var claim in user.Claims)
            {
                if (JwtSecurityTokenHandler.DefaultOutboundClaimTypeMap.ContainsKey(claim.Type))
                    Claims.Add(JwtSecurityTokenHandler.DefaultOutboundClaimTypeMap[claim.Type], claim.Value);
                else
                    Claims.Add(claim.Type, claim.Value);
            }
        }
    }
}
