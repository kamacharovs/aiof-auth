using System;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;

namespace aiof.auth.data
{
    public interface IClient
    {
        [JsonIgnore]
        [Required]
        int Id { get; set; }

        [JsonIgnore]
        [Required]
        Guid PublicKey { get; set; }

        [Required]
        [MaxLength(200)]
        string Name { get; set; }

        [Required]
        [MaxLength(50)]
        string Slug { get; set; }

        [Required]
        bool Enabled { get; set; }

        [Required]
        [MaxLength(100)]
        string PrimaryApiKey { get; set; }

        [Required]
        [MaxLength(100)]
        string SecondaryApiKey { get; set; }

        [Required]
        DateTime Created { get; set; }
    }
}