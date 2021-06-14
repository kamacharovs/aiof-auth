using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;

namespace aiof.auth.data
{
    public class Client : IClient,
        IPublicKeyId, IApiKey, IEnable
    {
        public int Id { get; set; }
        public Guid PublicKey { get; set; } = Guid.NewGuid();
        public string Name { get; set; }
        public string Slug { get; set; }
        public bool Enabled { get; set; }
        public string PrimaryApiKey { get; set; }
        public string SecondaryApiKey { get; set; }
        public int RoleId { get; set; }
        public Role Role { get; set; }
        public DateTime Created { get; set; } = DateTime.UtcNow;

        [JsonIgnore]
        public ICollection<ClientRefreshToken> RefreshTokens { get; set; } = new List<ClientRefreshToken>();
    }

    public class ClientDto
    {
        [Required]
        [MaxLength(200)]
        public string Name { get; set; }

        public bool Enabled { get; set; } = true;
        public int? RoleId { get; set; }
    }
}