using System;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;

namespace aiof.auth.data
{
    public class User : IUser, 
        IPublicKeyId
    {
        [JsonIgnore] public int Id { get; set; }
        [JsonIgnore] public Guid PublicKey { get; set; } = Guid.NewGuid();
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }
        [JsonIgnore] public string Password { get; set; }
        public DateTime Created { get; set; } = DateTime.UtcNow;
    }

    public class UserDto
    {
        /// <summary>
        /// First name
        /// </summary>
        [Required]
        [MaxLength(200)]
        public string FirstName { get; set; }

        /// <summary>
        /// Last name
        /// </summary>
        [Required]
        [MaxLength(200)]
        public string LastName { get; set; }

        /// <summary>
        /// Email address
        /// </summary>
        [Required]
        [EmailAddress]
        [MaxLength(200)]
        public string Email { get; set; }

        /// <summary>
        /// Unique username
        /// </summary>
        [Required]
        [MaxLength(200)]
        public string Username { get; set; }

        /// <summary>
        /// Password
        /// </summary>
        [JsonIgnore]
        [Required]
        [MaxLength(100)]
        public string Password { get; set; }
    }
}
