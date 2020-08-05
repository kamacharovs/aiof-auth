using System;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;

namespace aiof.auth.data
{
    public interface IClient
    {
        /// <summary>
        /// Unique identifier in database
        /// </summary>
        [JsonIgnore]
        [Required]
        int Id { get; set; }

        /// <summary>
        /// Globally unique identifier
        /// </summary>
        [JsonIgnore]
        [Required]
        Guid PublicKey { get; set; }

        /// <summary>
        /// Name
        /// </summary>
        [Required]
        [MaxLength(200)]
        string Name { get; set; }

        /// <summary>
        /// Slug
        /// </summary>
        [Required]
        [MaxLength(50)]
        string Slug { get; set; }

        /// <summary>
        /// Enabled or not
        /// </summary>
        [Required]
        bool Enabled { get; set; }

        /// <summary>
        /// Primary api key
        /// </summary>
        [Required]
        [MaxLength(100)]
        string PrimaryApiKey { get; set; }

        /// <summary>
        /// Secondary api key
        /// </summary>
        [Required]
        [MaxLength(100)]
        string SecondaryApiKey { get; set; }

        /// <summary>
        /// Datetime of creation
        /// </summary>
        [Required]
        DateTime Created { get; set; }
    }
}