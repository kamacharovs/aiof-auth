using System;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;

namespace aiof.auth.data
{
    public class Client : IClient,
        IPublicKeyId, IApiKey
    {
        [JsonIgnore] public int Id { get; set; }
        [JsonIgnore] public Guid PublicKey { get; set; } = Guid.NewGuid();
        public string Name { get; set; }
        public string Slug { get; set; }
        public bool Enabled { get; set; } = true;
        public string PrimaryApiKey { get; set; }
        public string SecondaryApiKey { get; set; }
        public DateTime Created { get; set; } = DateTime.UtcNow;
    }

    public class ClientDto
    {
        /// <summary>
        /// Name
        /// </summary>
        [Required]
        [MaxLength(200)]
        public string Name { get; set; }

        /// <summary>
        /// Slug
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string Slug { get; set; }

        /// <summary>
        /// Enabled or not
        /// </summary>
        public bool Enabled { get; set; } = true;
    }
}