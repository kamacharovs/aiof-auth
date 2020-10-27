using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;

namespace aiof.auth.data
{
    public class Client : IClient,
        IPublicKeyId, IApiKey, IEnable
    {
        [JsonIgnore]
        [Required]
        public int Id { get; set; }

        [JsonIgnore]
        [Required]
        public Guid PublicKey { get; set; } = Guid.NewGuid();

        [Required]
        [MaxLength(200)]
        public string Name { get; set; }

        [Required]
        [MaxLength(50)]
        public string Slug { get; set; }

        [Required]
        public bool Enabled { get; set; }

        [Required]
        [MaxLength(100)]
        public string PrimaryApiKey { get; set; }

        [Required]
        [MaxLength(100)]
        public string SecondaryApiKey { get; set; }
        
        [JsonIgnore]
        [Required]
        public int RoleId { get; set; }
        
        [Required]
        public Role Role { get; set; }

        [Required]
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