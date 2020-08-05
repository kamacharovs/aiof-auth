using System;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;

namespace aiof.auth.data
{
    public interface IUser
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
        /// First name
        /// </summary>
        [Required]
        [MaxLength(200)]
        string FirstName { get; set; }

        /// <summary>
        /// Last name
        /// </summary>
        [Required]
        [MaxLength(200)]
        string LastName { get; set; }

        /// <summary>
        /// Email address
        /// </summary>
        [Required]
        [EmailAddress]
        [MaxLength(200)]
        string Email { get; set; }

        /// <summary>
        /// Unique username
        /// </summary>
        [Required]
        [MaxLength(200)]
        string Username { get; set; }

        /// <summary>
        /// Password
        /// </summary>
        [JsonIgnore]
        [Required]
        [MaxLength(100)]
        string Password { get; set; }

        /// <summary>
        /// Datetime of creation
        /// </summary>
        [Required]
        DateTime Created { get; set; }
    }
}